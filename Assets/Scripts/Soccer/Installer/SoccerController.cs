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
    public class GameEndsSignal { }

    public class GoalScoredSignal
    {
        public int team;
    }

    public class BallResetSignal { }

    public class SoccerController : MonoInstaller
    {
        [Inject] private LevelAsset levelAsset;
        [Inject] private PlayerData[] playersData;
        [Inject] private GameTime gameTime;
        [Inject] private GameState gameState;

        private SoccerState soccerState = new SoccerState();

        public override void InstallBindings()
        {
            Container.DeclareSignal<GameEndsSignal>();
            Container.DeclareSignal<GoalScoredSignal>();
            Container.DeclareSignal<BallResetSignal>();

            Container.BindInstance(soccerState);
        }

        private void Start()
        {
            InitGameState();

            // Init the level
            var spawnPoints = Container.InstantiatePrefab(levelAsset.prefab).GetComponent<SpawnPointGroup>();

            // Init the players
            var spawned = new List<Tuple<PlayerData, GameObject>>();

            for (int a = 0; a < playersData.Length; a++)
            {
                var playerData = playersData[a];
                var instance = ProjectUtils.InitializePlayer(playerData, spawnPoints,gameState,Container);

                spawned.Add(new Tuple<PlayerData, GameObject>(playerData, instance));
            }
            spawnPoints.ResolvePlayerSpawn(spawned);
        }

        private void InitGameState(){
            var signalBus = Container.Resolve<SignalBus>();
            // Init the state

            // Init the timer logic
            soccerState.secondsRemaining.Value = gameTime.seconds;
            Observable.Interval(TimeSpan.FromSeconds(1))
                      .TakeUntil(signalBus.GetStream<GameEndsSignal>())
                      .Subscribe(_ => soccerState.secondsRemaining.Value--);
            soccerState.secondsRemaining
                 .TakeUntilDestroy(this)
                 .Where(timeRemaining => timeRemaining <= 0)
                 .Where(_ => soccerState.scores.HasMax(score => score.Value))
                 .Subscribe(_ => signalBus.Fire<GameEndsSignal>());
            signalBus.GetStream<GameEndsSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ =>
                     {
                         Observable.Timer(TimeSpan.FromSeconds(10))
                                   .TakeUntilDestroy(this)
                                   .Subscribe(__ => Application.LoadLevel(Application.loadedLevel));
                     });

            // Debug Commands
            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(KeyCode.E))
                      .First()
                      .Subscribe(_ => soccerState.secondsRemaining.Value = 1);
            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(KeyCode.R))
                      .First()
                      .Subscribe(_ => Application.LoadLevel(Application.loadedLevel));

            // Init the score keeping
            foreach (var a in playersData.Select(player => player.team.id).Distinct())
            {
                soccerState.scores.Add(a, 0);
            }
            signalBus.GetStream<GoalScoredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(signal =>
                     {
                         int teamID = signal.team;
                         if (!soccerState.scores.ContainsKey(teamID)) soccerState.scores.Add(teamID, 0);
                         soccerState.scores[teamID]++;
                     });
        }
    }
}