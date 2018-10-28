using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using Swing.Game;
using Swing.Sound;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Swing.Character
{
    public class GrapplingFiredSignal{}

    public class GrapplingReleasedSignal{}

    public class GrapplingHook : MonoBehaviour
    {
        [Inject] private SignalBus signalBus;
        [Inject] private CharacterSettings settings;
        [Inject] private DiContainer container;
        [Inject] private CharacterState characterState;
        [Inject] private GameCameraState gameCameraState;
        [Inject] private SoundPlayer soundPlayer;

        private List<UnityEngine.Object> currentRope = new List<UnityEngine.Object>();

        private void Start()
        {
            signalBus.GetStream<GrapplingFiredSignal>()
                     .TakeUntilDestroy(this)
                     .Where(_ => characterState.localPlayerControl.Value)
                     .Subscribe(_ =>
                     {
                         DestroyRope();

                         var hit = Physics2D.Raycast(transform.position, characterState.aimDirection.Value, settings.grapplingDistance, settings.grapplingMask.value);

                         if (hit.transform != null)
                         {
                             Vector3 hitPosition = hit.point;
                             float distance = Vector2.Distance(hitPosition, transform.position);

                             float time = distance / settings.grapplingHookSpeed;

                             var ropeEffect = container.InstantiateComponent<RopeEffect>( gameObject);

                             currentRope.Add(ropeEffect.Init(transform, hitPosition));
                             currentRope.Add(ropeEffect);

                             Observable.Timer(TimeSpan.FromSeconds(time))
                                       .TakeUntil(signalBus.GetStream<GrapplingReleasedSignal>())
                                       .TakeUntilDestroy(this)
                                       .Subscribe(__ =>
                                       {
                                           var anchor = new GameObject("GrapplingAnchor");
                                           anchor.transform.position = hitPosition;

                                           /*gameCameraState.pointsOfInterest.Add(anchor.transform);
                                           anchor.OnDestroyAsObservable()
                                                 .Merge(gameObject.OnDestroyAsObservable())
                                                 .Merge(characterState.isCorpse.Where(isCorpse => isCorpse).Select(__ => Unit.Default))
                                                 .First()
                                                 .Subscribe(__ => gameCameraState.pointsOfInterest.Remove(anchor.transform));*/

                                           var anchorRigidbody = anchor.AddComponent<Rigidbody2D>();
                                           anchorRigidbody.isKinematic = true;

                                           var joint = anchor.gameObject.AddComponent<SpringJoint2D>();
                                           joint.autoConfigureDistance = false;
                                           joint.autoConfigureConnectedAnchor = false;
                                           joint.anchor = anchor.transform.InverseTransformPoint(hitPosition);
                                           joint.distance = Vector2.Distance(hitPosition, transform.position) * settings.initialGrapplingDistanceRatio;
                                           joint.dampingRatio = 1f;
                                           joint.frequency = settings.ropeSpringFrequency;
                                           joint.enableCollision = true;

                                           var rigidbody = GetComponent<Rigidbody2D>();
                                           joint.connectedBody = rigidbody;

                                           currentRope.Add(anchor.gameObject);

                                           var climbSoundDisposable = soundPlayer.PlaySound("Climbing", transform);

                                           var endStream = this.OnDestroyAsObservable()
                                                               .Merge(anchor.OnDestroyAsObservable())
                                                               .Merge(signalBus.GetStream<GrapplingReleasedSignal>().Select(___=>Unit.Default));
                                           endStream
                                               .First()
                                               .Subscribe(___ => climbSoundDisposable.Dispose());

                                           // Add Climbing
                                           Observable.EveryUpdate()
                                                     .TakeUntil(endStream)
                                                     .Where(___ => characterState.localPlayerControl.Value)
                                                     .Select(___ => characterState.aimDirection.Value)
                                                     .Subscribe(direction =>
                                                     {
                                                         var ropeDirection = (anchor.transform.position - transform.position).normalized;
                                                         var aimDirection = characterState.aimDirection.Value.normalized;
                                                         var ropeDot = Vector2.Dot(aimDirection, ropeDirection);

                                                         var left = Vector2.Perpendicular(ropeDirection);
                                                         rigidbody.AddForce(settings.swingForce * left * Vector2.Dot(left, aimDirection));

                                                         var right = -Vector2.Perpendicular(ropeDirection);
                                                         rigidbody.AddForce(settings.swingForce * right * Vector2.Dot(right, aimDirection));

                                                         var distanceDelta = ropeDot * settings.ropeClimbSpeed * Time.deltaTime;

                                                         joint.distance -= distanceDelta;
                                                         if (joint.distance < .1f) joint.frequency += distanceDelta * .5f;
                                                     });
                                       });
                         }
            });

            signalBus.GetStream<GrapplingReleasedSignal>()
                     .TakeUntilDestroy(this)
                     .Where(_ => currentRope != null)
                     .Subscribe(_ => DestroyRope());
        }

        private void OnDestroy()
        {
            if (currentRope != null) DestroyRope();
        }

        private void DestroyRope(){
            currentRope.ForEach(Destroy);
            currentRope.Clear();
        }
    }
}