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

namespace Swing.Game
{
    public class GameEndsSignal { }

    public class GoalScoredSignal
    {
        public int team;
    }

    public class BallResetSignal { }

    public class GameController : MonoInstaller
    {
        [Inject] private LevelAsset levelAsset;
        [Inject] private PlayerData[] playersData;
        [Inject] private GameTime time;
        [Inject] private SplitScreenLayouts layouts;
        [Inject] private GameTime gameTime;

        public override void InstallBindings()
        {
            Container.DeclareSignal<GameEndsSignal>();
            Container.DeclareSignal<GoalScoredSignal>();
            Container.DeclareSignal<BallResetSignal>();

            var signalBus = Container.Resolve<SignalBus>();
            // Init the state
            var state = new GameState();

            // Init the timer logic
            state.secondsRemaining.Value = gameTime.seconds;
            Observable.Interval(TimeSpan.FromSeconds(1))
                      .TakeUntil(signalBus.GetStream<GameEndsSignal>())
                      .Subscribe(_ => state.secondsRemaining.Value--);
            state.secondsRemaining
                 .TakeUntilDestroy(this)
                 .Where(timeRemaining => timeRemaining <= 0)
                 .Where(_ => state.scores.HasMax(score => score.Value))
                 .Subscribe(_ => signalBus.Fire<GameEndsSignal>());
            signalBus.GetStream<GameEndsSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ =>
                     {
                         Observable.Timer(TimeSpan.FromSeconds(10))
                                   .TakeUntilDestroy(this)
                                   .Subscribe(__ => Application.LoadLevel(Application.loadedLevel));
                     });
            Observable.EveryUpdate()
                      .Where(_=> Input.GetKeyDown(KeyCode.E))
                      .First()
                      .Subscribe(_ => state.secondsRemaining.Value = 1);

            // Init the score keeping
            foreach (var a in playersData.Select(player => player.team.id).Distinct()){
                state.scores.Add(a, 0);
            }
            signalBus.GetStream<GoalScoredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(signal => {
                        int teamID = signal.team;
                        if (!state.scores.ContainsKey(teamID)) state.scores.Add(teamID, 0);
                        state.scores[teamID]++; 
            });

            Container.BindInstance(state);

            // Init the level
            var levelContext = Container.CreateSubContainer();
            var level = levelContext.InstantiatePrefab(levelAsset.prefab).GetComponent<LevelInstaller>();

            // Init the players
            var layout = layouts.layouts.First(a => a.settings.Length == playersData.Length);
            var spawned = new List<Tuple<PlayerData, GameObject>>();

            for (int a = 0; a < playersData.Length; a++)
            {
                var playerData = playersData[a];
                var instance = InitializePlayer(playerData, layout.settings[a], level);

                spawned.Add(new Tuple<PlayerData, GameObject>(playerData, instance));
            }
            level.ResolvePlayerSpawn(spawned);
        }

        private GameObject InitializePlayer(PlayerData playerData, CameraSettings cameraSettings, LevelInstaller level){
            DiContainer playerContext = null;
            CharacterState characterState = null;
            GameObject instance = null;

            Action MakeNewPlayer = null;
            MakeNewPlayer = () =>
            {
                playerContext = Container.CreateSubContainer();
                characterState = new CharacterState();
                playerContext.DeclareSignal<PlayerKilledSignal>();
                playerContext.BindInstance(characterState);
                playerContext.BindInstance(playerData);
                playerContext.BindInstance(cameraSettings);
                instance = playerContext.InstantiatePrefab(playerData.character.prefab);
                playerContext.Resolve<SignalBus>()
                         .GetStream<PlayerKilledSignal>()
                         .Subscribe(_ =>
                         {
                             characterState.localPlayerControl.Value = false;

                             var oldInstance = instance;
                             Observable.Timer(TimeSpan.FromSeconds(3f))
                                       .Subscribe(__ => {
                                           characterState.isCorpse.Value = true;
                                           Observable.Timer(TimeSpan.FromSeconds(30))
                                                     .Subscribe(___ => GameObject.Destroy(oldInstance));

                                           //----RESETS ALL VALUES-----
                                           MakeNewPlayer();

                                           level.ResolvePlayerSpawn(new List<Tuple<PlayerData, GameObject>> { new Tuple<PlayerData, GameObject>(playerData, instance) });
                                       });

                         });
            };

            MakeNewPlayer();

            return instance;
        }
    }
}