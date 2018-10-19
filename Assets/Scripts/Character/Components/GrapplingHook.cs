using UnityEngine;
using Zenject;
using UniRx;

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

        private GameObject currentRope;

        private void Start()
        {
            signalBus.GetStream<GrapplingFiredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ =>
                     {
                         if (currentRope != null) Destroy(currentRope);

                         var hit = Physics2D.Raycast(transform.position, characterState.aimDirection.Value, settings.grapplingDistance, settings.grapplingMask.value);

                         if (hit.transform != null)
                         {
                             Vector3 hitPosition = hit.point;

                             var anchor = new GameObject("GrapplingAnchor");
                             anchor.transform.position = hitPosition;

                             var anchorRigidbody = anchor.AddComponent<Rigidbody2D>();
                             anchorRigidbody.isKinematic = true;

                             var joint = anchor.gameObject.AddComponent<SpringJoint2D>();
                             joint.anchor = anchor.transform.InverseTransformPoint(hitPosition);
                             joint.autoConfigureDistance = false;
                             joint.distance = 0;
                             joint.dampingRatio = 1;
                             joint.frequency = settings.ropeSpringFrequency;
                             joint.enableCollision = true;

                             var rigidbody = GetComponent<Rigidbody2D>();
                             joint.connectedBody = rigidbody;

                             container.Inject(anchor.AddComponent<RopeEffect>());

                             currentRope = anchor;

                             Observable.EveryUpdate()
                                       .TakeUntilDestroy(this)
                                       .TakeUntil(signalBus.GetStream<GrapplingReleasedSignal>())
                                       .Select(__ => characterState.aimDirection.Value)
                                       .Subscribe(direction =>
                                       {
                                           var ropeDirection = (anchor.transform.position - transform.position).normalized;
                                           var aimDirection = characterState.aimDirection.Value.normalized;
                                           var ropeDot = Vector2.Dot(aimDirection, ropeDirection);
                                           joint.distance -= ropeDot * settings.ropeClimbSpeed * Time.deltaTime;

                                           var left = Vector2.Perpendicular(ropeDirection);
                                           rigidbody.AddForce(settings.swingForce * left * Vector2.Dot(left, aimDirection));

                                           var right = -Vector2.Perpendicular(ropeDirection);
                                           rigidbody.AddForce(settings.swingForce * right * Vector2.Dot(right, aimDirection));
                                       });
                         }
                     });

            signalBus.GetStream<GrapplingReleasedSignal>()
                     .TakeUntilDestroy(this)
                     .Where(_ => currentRope != null)
                     .Subscribe(_ => Destroy(currentRope));
        }


        private GameObject ConnectRope(Rigidbody2D launcher, Vector3 direction, int mask)
        {
            var hit = Physics2D.Raycast(launcher.position, direction, settings.grapplingDistance, mask);

            Vector3 hitPosition = hit.point;

            if (hit.collider == null)
                return null;

            var anchor = new GameObject("GrapplingAnchor");
            anchor.transform.position = hitPosition;
            var anchorRigidbody = anchor.AddComponent<Rigidbody2D>();
            anchorRigidbody.isKinematic = true;

            var joint = anchor.gameObject.AddComponent<SpringJoint2D>();
            joint.anchor = anchor.transform.InverseTransformPoint(hitPosition);
            joint.autoConfigureDistance = false;
            joint.distance = 0;
            joint.dampingRatio = 1;
            joint.frequency = settings.ropeSpringFrequency;
            joint.enableCollision = true;

            joint.connectedBody = launcher;

            container.Inject(anchor.AddComponent<RopeEffect>());

            return anchor;
        }

        private void OnDestroy()
        {
            if (currentRope != null) Destroy(currentRope);
        }

        private void OnDrawGizmos()
        {
            if(currentRope != null){
                Vector3 position = currentRope.transform.position;
                float iconSize = .1f;
                Gizmos.DrawLine(position + Vector3.up * iconSize, position - Vector3.up * iconSize);
                Gizmos.DrawLine(position + Vector3.back * iconSize, position - Vector3.back * iconSize);
                Gizmos.DrawLine(position + Vector3.right * iconSize, position - Vector3.right * iconSize);

                Gizmos.DrawLine(currentRope.transform.position, transform.position);
            }
        }
    }
}