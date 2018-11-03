using Zenject;
using System.Linq;
using UnityEngine;
using Swing.Level;
using Swing.Player;
using System.Collections.Generic;
using UniRx;
using Swing.Character;
using System;
using InControl;
using UnityEngine.Audio;
using Swing.Sound;

namespace Swing.Game.Soccer
{

    public class SoccerController : IInitializable, IDisposable
    {
        [Inject] private LevelAsset levelAsset;
        [Inject] private PlayerData[] playersData;
        [Inject] private GameTime gameTime;
        [Inject] private PlayerLifeController playerLifeController;
        [Inject] private SpawnPointGroup spawnPointGroup;
        [Inject] private SoccerState soccerState;
        [Inject] private DiContainer container;

        private CompositeDisposable disposables = new CompositeDisposable();

        public void Initialize()
        {
            InitGameState();

            // Init the players
            var spawned = new List<Tuple<PlayerData, GameObject>>();

            for (int a = 0; a < playersData.Length; a++)
            {
                var playerData = playersData[a];
                var instance = playerLifeController.InitializePlayer(playerData);

                spawned.Add(new Tuple<PlayerData, GameObject>(playerData, instance));
            }
            spawnPointGroup.ResolvePlayerSpawn(spawned);
        }

        private void InitGameState(){
            var signalBus = container.Resolve<SignalBus>();

            // Init the timer logic
            soccerState.secondsRemaining.Value = gameTime.seconds;
            Observable.Interval(TimeSpan.FromSeconds(1))
                      .TakeUntil(signalBus.GetStream<GameEndSignal>())
                      .Subscribe(_ => soccerState.secondsRemaining.Value--);
            soccerState.secondsRemaining
                       .Where(timeRemaining => timeRemaining <= 0)
                       .Where(_ => soccerState.scores.HasMax(score => score.Value))
                       .Subscribe(_ => signalBus.Fire<GameEndSignal>())
                       .AddTo(disposables);
            signalBus.GetStream<GameEndSignal>()
                     .Subscribe(_ =>
                     {
                         Observable.Timer(TimeSpan.FromSeconds(10))
                                   .Subscribe(__ => ProjectUtils.ReturnToMainMenu())
                                   .AddTo(disposables);
                     })
                     .AddTo(disposables);
            // Debug Commands
            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(KeyCode.E))
                      .First()
                      .Subscribe(_ => soccerState.secondsRemaining.Value = 1);


            // Init the score keeping
            foreach (var a in playersData.Select(player => player.team.id).Distinct())
            {
                soccerState.scores.Add(a, 0);
            }
            signalBus.GetStream<GoalScoredSignal>()
                     .Subscribe(signal =>
                     {
                         int teamID = signal.team;
                         if (!soccerState.scores.ContainsKey(teamID)) soccerState.scores.Add(teamID, 0);
                         soccerState.scores[teamID]++;
                     })
                     .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}