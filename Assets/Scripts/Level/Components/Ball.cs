using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System;
using Swing.Game;

namespace Swing.Level
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Ball : MonoBehaviour
    {
        [Inject] SignalBus signalBus;
        [Inject] Transform restartPoint;

        public void Start()
        {
            Reset();

            signalBus.GetStream<GoalScoredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ =>
                     {
                         Observable.Timer(TimeSpan.FromSeconds(5))
                                .TakeUntilDestroy(this)
                                .Subscribe(__ =>
                                {
                                    Reset();
                                });
                     });
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