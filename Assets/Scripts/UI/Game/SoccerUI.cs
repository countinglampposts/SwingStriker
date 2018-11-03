using System;
using Swing.Player;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Linq;
using Swing.Game;
using Swing.Game.Soccer;

namespace Swing.UI
{
    public class SoccerUI : MonoBehaviour
    {
        [SerializeField] private Text scoreTextUI;
        [SerializeField] private Text timerTextUI;
        [SerializeField] private Text centerTextUI;
        [SerializeField] private GameObject root;

        [Inject] SignalBus signalBus;
        [Inject] GameState gameState;
        [Inject] SoccerState soccerState;
        [Inject] TeamsData teamsData;

        private void Awake()
        {
            centerTextUI.enabled = false;
        }

        private void Start()
        {
            gameState.isPaused
                     .TakeUntilDestroy(this)
                     .Subscribe(isPaused => root.SetActive(!isPaused));

            signalBus.GetStream<GoalScoredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_=> {
                         centerTextUI.text = "Goal!";
                         centerTextUI.enabled = true;
                         Observable.Timer(TimeSpan.FromSeconds(5))
                                   .TakeUntilDestroy(this)
                                   .TakeUntil(signalBus.GetStream<GameEndSignal>())
                                   .Subscribe(__ => centerTextUI.enabled = false);
                         Observable.Interval(TimeSpan.FromSeconds(1))
                                   .TakeUntilDestroy(this)
                                   .TakeUntil(signalBus.GetStream<GameEndSignal>())
                                   .TakeWhile(interval => interval <= 5)
                                   .Skip(1)
                                   .Subscribe(interval => centerTextUI.text = (4 - interval).ToString());
                     });
            signalBus.GetStream<GameEndSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => {
                         var winningTeam = teamsData.teams.First(selectedTeam => selectedTeam.id == soccerState.scores.MaxValueOrDefault(score => score.Value).Key);
                         centerTextUI.text = winningTeam.displayName + " wins!";
                         centerTextUI.enabled = true;
                     });
            soccerState.scores.ObserveReplace()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ =>
                     {
                         string scoreText = string.Empty;
                         foreach (int teamId in soccerState.scores.Keys)
                         {
                             var team = teamsData.teams.FirstOrDefault(selectedTeam => selectedTeam.id == teamId);
                             scoreText += team.name + ": " + soccerState.scores[teamId] + " - ";
                         }
                         scoreTextUI.text = scoreText;
                     });
            soccerState.secondsRemaining
                     .TakeUntilDestroy(this)
                     .Subscribe(remainingTime => timerTextUI.text = remainingTime.ToString());
        }
    }
}