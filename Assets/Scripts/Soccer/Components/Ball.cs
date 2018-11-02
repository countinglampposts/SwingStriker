using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using System;

namespace Swing.Game.Soccer
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Ball : MonoBehaviour
    {
        [Inject] SignalBus signalBus;
        [Inject] Transform resetPoint;
        [Inject] GameCameraState gameCameraState;

        [SerializeField] LayerMask pathCastingMask;
        [SerializeField] LayerMask explosionMask;
        [SerializeField] ParticleSystem goalExplosion;

        public void Start()
        {
            var rigidBody = GetComponent<Rigidbody2D>();
            var collider = GetComponent<Collider2D>();

            gameCameraState.pointsOfInterest.Add(transform);
            gameObject.OnDestroyAsObservable()
                      .First()
                      .Subscribe(_ => gameCameraState.pointsOfInterest.Remove(transform));

            signalBus.GetStream<GoalScoredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ =>
                     {
                         Instantiate(goalExplosion, transform.position, transform.rotation).AutoDestruct();
                         ProjectUtils.AddExplosionForce(1000, transform.position, 20, explosionMask.value);
                         gameObject.SetActive(false);

                         gameCameraState.pointsOfInterest.Add(resetPoint);
                         Observable.Timer(TimeSpan.FromSeconds(5))
                                .TakeUntilDestroy(this)
                                .Subscribe(__ =>
                                {
                                    gameObject.SetActive(true);
                                    gameCameraState.pointsOfInterest.Remove(resetPoint);
                                    Reset();
                                });
                     });

            // Add the toggling of the slowdown effect
            bool slowingEffectActive = true;
            signalBus.GetStream<GoalScoredSignal>()
                     .Select(_ => true)
                     .Merge(signalBus.GetStream<BallResetSignal>().Select(_ => false))
                     .TakeUntilDestroy(this)
                     .Subscribe(isBetweenScoreAndReset => slowingEffectActive = !isBetweenScoreAndReset);

            // Init the slowdown effect
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => slowingEffectActive)
                      .Select(_ => Physics2D.CircleCast(transform.position, collider.bounds.extents.x, rigidBody.velocity, rigidBody.velocity.magnitude, pathCastingMask.value))
                      .Where(hit => hit.collider != null)
                      // HACK
                      .Where(hit => hit.collider.gameObject.GetComponent<Goal>() != null)
                      .Subscribe(hit => signalBus.Fire(new TimeSlowSignal { slowTime = .1f }));

            Reset();
        }

        private void Reset()
        {
            transform.position = resetPoint.position;
            var rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0;
            signalBus.Fire<BallResetSignal>();
        }
    }
}