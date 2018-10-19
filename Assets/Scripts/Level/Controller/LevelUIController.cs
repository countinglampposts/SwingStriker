﻿using System;
using System.Collections;
using System.Collections.Generic;
using Swing.Game;
using Swing.Player;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Linq;

namespace Swing.Level
{
    public class LevelUIController : MonoBehaviour
    {
        [SerializeField] private Text scoreTextUI;
        [SerializeField] private Text timerTextUI;
        [SerializeField] private Text centerTextUI;

        [Inject] SignalBus signalBus;
        [Inject] GameState gameState;
        [Inject] TeamsData teamsData;

        private void Awake()
        {
            centerTextUI.enabled = false;
        }

        private void Start()
        {
            signalBus.GetStream<GoalScoredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_=> {
                         centerTextUI.text = "Goal!";
                         centerTextUI.enabled = true;
                         Observable.Timer(TimeSpan.FromSeconds(5))
                                   .TakeUntilDestroy(this)
                                   .TakeUntil(signalBus.GetStream<GameEndsSignal>())
                                   .Subscribe(__ => centerTextUI.enabled = false);
                     });
            signalBus.GetStream<GameEndsSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => {
                         var winningTeam = teamsData.teams.First(selectedTeam => selectedTeam.id == gameState.scores.MaxValueOrDefault(score => score.Value).Key);
                         centerTextUI.text = winningTeam.displayName + " wins!";
                         centerTextUI.enabled = true;
                     });
            gameState.scores.ObserveReplace()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ =>
                     {
                         string scoreText = string.Empty;
                         foreach (int teamId in gameState.scores.Keys)
                         {
                             var team = teamsData.teams.FirstOrDefault(selectedTeam => selectedTeam.id == teamId);
                             scoreText += team.name + ": " + gameState.scores[teamId] + " - ";
                         }
                         scoreTextUI.text = scoreText;
                     });
            gameState.secondsRemaining
                     .TakeUntilDestroy(this)
                     .Subscribe(remainingTime => timerTextUI.text = remainingTime.ToString());
        }
    }
}