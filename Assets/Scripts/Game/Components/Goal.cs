using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Level
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
            signalBus.Subscribe<GoalScoredSignal>(HandleGoalScored);
            signalBus.Subscribe<BallResetSignal>(HandleRestart);
        }

        private void OnDestroy()
        {
            signalBus.Unsubscribe<GoalScoredSignal>(HandleGoalScored);
            signalBus.Unsubscribe<BallResetSignal>(HandleRestart);
        }

        private void HandleGoalScored()
        {
            locked = true;
        }

        private void HandleRestart()
        {
            locked = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(!locked && collision.gameObject == gameBall.gameObject)
            {
                Debug.Log("Goal!!!!");
                signalBus.Fire<GoalScoredSignal>();
            }
        }
    }
}