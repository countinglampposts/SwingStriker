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
        [SerializeField] private GameCameraController cameraControllerPrefab;
        [SerializeField] private AudioMixerGroup audioMixerGroup;

        [Inject] private LevelAsset levelAsset;
        [Inject] private PlayerData[] playersData;
        [Inject] private GameTime time;
        [Inject] private SplitScreenLayouts layouts;
        [Inject] private GameTime gameTime;

        private GameState gameState = new GameState();

        public override void InstallBindings()
        {
            Container.DeclareSignal<GameEndsSignal>();
            Container.DeclareSignal<GoalScoredSignal>();
            Container.DeclareSignal<BallResetSignal>();

            Container.BindInstance(audioMixerGroup);
            Container.BindInstance(gameState);
            Container.BindInstance(new GameCameraState());

            Container.Bind<Camera>()
                     .FromComponentInNewPrefab(cameraControllerPrefab)
                     .AsSingle()
                     .NonLazy();
        }

        private void Start()
        {
            InitGameState();

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

        private void InitGameState(){
            var signalBus = Container.Resolve<SignalBus>();
            // Init the state

            // Init the timer logic
            gameState.secondsRemaining.Value = gameTime.seconds;
            Observable.Interval(TimeSpan.FromSeconds(1))
                      .TakeUntil(signalBus.GetStream<GameEndsSignal>())
                      .Subscribe(_ => gameState.secondsRemaining.Value--);
            gameState.secondsRemaining
                 .TakeUntilDestroy(this)
                 .Where(timeRemaining => timeRemaining <= 0)
                 .Where(_ => gameState.scores.HasMax(score => score.Value))
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
                      .Subscribe(_ => gameState.secondsRemaining.Value = 1);
            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(KeyCode.R))
                      .First()
                      .Subscribe(_ => Application.LoadLevel(Application.loadedLevel));

            // Init the score keeping
            foreach (var a in playersData.Select(player => player.team.id).Distinct())
            {
                gameState.scores.Add(a, 0);
            }
            signalBus.GetStream<GoalScoredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(signal =>
                     {
                         int teamID = signal.team;
                         if (!gameState.scores.ContainsKey(teamID)) gameState.scores.Add(teamID, 0);
                         gameState.scores[teamID]++;
                     });
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

                var playerKilledStream = playerContext.Resolve<SignalBus>()
                                                      .GetStream<PlayerKilledSignal>();
                gameState.isPaused
                         .TakeUntilDestroy(instance)
                         .Subscribe(isPaused => characterState.localPlayerControl.Value = !isPaused);

                playerKilledStream
                         .First()
                         .TakeUntilDestroy(instance)
                         .Subscribe(_ =>
                         {
                             characterState.localPlayerControl.Value = false;

                             var oldInstance = instance;
                             Observable.Timer(TimeSpan.FromSeconds(3f))
                                       .TakeUntilDestroy(oldInstance)
                                       .Subscribe(__ => {
                                           characterState.isCorpse.Value = true;
                                           Observable.Timer(TimeSpan.FromSeconds(30))
                                                     .TakeUntilDestroy(oldInstance)
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