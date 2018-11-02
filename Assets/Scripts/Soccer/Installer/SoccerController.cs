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
        [SerializeField] private GameCameraController cameraControllerPrefab;
        [SerializeField] private AudioMixerGroup audioMixerGroup;

        [Inject] private LevelAsset levelAsset;
        [Inject] private PlayerData[] playersData;
        [Inject] private GameTime time;
        [Inject] private SplitScreenLayouts layouts;
        [Inject] private GameTime gameTime;

        private GameState gameState = new GameState();
        private SoccerState soccerState = new SoccerState();

        public override void InstallBindings()
        {
            Container.DeclareSignal<GameEndsSignal>();
            Container.DeclareSignal<GoalScoredSignal>();
            Container.DeclareSignal<BallResetSignal>();

            Container.BindInstance(audioMixerGroup);
            Container.BindInstance(soccerState);
            Container.BindInstance(gameState);
            Container.BindInstance(new GameCameraState());

            Container.Bind<SoundPlayer>()
                     .AsTransient();

            Container.Bind<Camera>()
                     .FromComponentInNewPrefab(cameraControllerPrefab)
                     .AsSingle()
                     .NonLazy();
        }

        private void Start()
        {
            InitGameState();

            Debug.Log(levelAsset.prefab);
            // Init the level
            var level = Container.InstantiatePrefab(levelAsset.prefab).GetComponent<SpawnPointGroup>();

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

        private GameObject InitializePlayer(PlayerData playerData, CameraSettings cameraSettings, SpawnPointGroup level){
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

                // Disabled player control when paused
                gameState.isPaused
                         .TakeUntilDestroy(instance)
                         .Subscribe(isPaused => characterState.localPlayerControl.Value = !isPaused);

                // Reset the player when killed via recursion
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