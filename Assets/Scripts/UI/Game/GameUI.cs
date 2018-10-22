using System;
using Swing.Player;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Linq;

namespace Swing.Game
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private Text scoreTextUI;
        [SerializeField] private Text timerTextUI;
        [SerializeField] private Text centerTextUI;
        [SerializeField] private GameObject root;

        [Inject] SignalBus signalBus;
        [Inject] GameState gameState;
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
                                   .TakeUntil(signalBus.GetStream<GameEndsSignal>())
                                   .Subscribe(__ => centerTextUI.enabled = false);
                         Observable.Interval(TimeSpan.FromSeconds(1))
                                   .TakeUntilDestroy(this)
                                   .TakeUntil(signalBus.GetStream<GameEndsSignal>())
                                   .TakeWhile(interval => interval <= 5)
                                   .Skip(1)
                                   .Subscribe(interval => centerTextUI.text = (4 - interval).ToString());
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