using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using System;
using Swing.Game;

namespace Swing.Level
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Ball : MonoBehaviour
    {
        [Inject] SignalBus signalBus;
        [Inject] Transform restartPoint;
        [Inject] GameCameraState gameCameraState;

        [SerializeField] LayerMask pathCastingMask;

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
                         Observable.Timer(TimeSpan.FromSeconds(5))
                                .TakeUntilDestroy(this)
                                .Subscribe(__ =>
                                {
                                    signalBus.Fire(new TimeSlowSignal { slowTime = 2f });
                                    Reset();
                                });
                     });

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Select(_ => Physics2D.CircleCast(transform.position, collider.bounds.extents.x, rigidBody.velocity, rigidBody.velocity.magnitude, pathCastingMask.value))
                      .Where(hit => hit.collider != null)
                      // HACK
                      .Where(hit => hit.collider.gameObject.GetComponent<Goal>() != null)
                      .Subscribe(hit => signalBus.Fire(new TimeSlowSignal { slowTime = .5f }));

            Reset();
        }

        private void Reset()
        {
            transform.position = restartPoint.position;
            var ridigBody = GetComponent<Rigidbody2D>();
            ridigBody.velocity = Vector2.zero;
            ridigBody.angularVelocity = 0;
            signalBus.Fire<BallResetSignal>();
        }
    }
}