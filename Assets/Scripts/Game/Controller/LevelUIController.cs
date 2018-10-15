using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Level
{
    public class LevelUIController : MonoBehaviour
    {
        [SerializeField] private Text scoreTextUI;
        [SerializeField] private Text goalUI;

        private string score;

        private Dictionary<int, int> teamScore = new Dictionary<int, int>{
            {0,0},
            {1,0}
        };

        [Inject] SignalBus signalBus;

        private void Awake()
        {
            goalUI.enabled = false;
        }

        private void Start()
        {
            signalBus.Subscribe<GoalScoredSignal>(HandleGoalScored);

        }

        private void OnDestroy()
        {
            signalBus.Unsubscribe<GoalScoredSignal>(HandleGoalScored);
        }

        private void HandleGoalScored(GoalScoredSignal goalScored)
        {
            goalUI.enabled = true;
            Observable.Timer(TimeSpan.FromSeconds(5))
                      .Subscribe(_ => goalUI.enabled = false);

            teamScore[goalScored.team]++;

            string scoreText = string.Empty;
            foreach(int score in teamScore.Values){
                scoreText += score + " - ";
            }
            scoreTextUI.text = scoreText;
        }
    }
}