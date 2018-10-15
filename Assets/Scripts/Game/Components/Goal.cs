using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

namespace Swing.Level
{
    public class GoalScoredSignal{
        public int team; 
    }
    public class BallResetSignal { }

    public class Goal : MonoBehaviour
    {
        [Inject] GameBall gameBall;
        [Inject] SignalBus signalBus;

        private bool locked;

        private void Start()
        {
            signalBus.GetStream<GoalScoredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => locked = true);

            signalBus.GetStream<BallResetSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => locked = false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(!locked && collision.gameObject == gameBall.gameObject)
            {
                signalBus.Fire<GoalScoredSignal>();
            }
        }
    }
}