using System.Collections;
using System.Collections.Generic;
using Swing.Game.Soccer;
using Swing.Level;
using Swing.Player;
using Swing.Sound;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Swing.Game
{
    /// <summary>
    /// This installs all basic game functionality and dependencies
    /// This includes:
    ///  - Audio functionality
    ///  - Camera control
    ///  - Gamestate
    ///  - Time control
    ///  - Player spawning
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameCameraController cameraControllerPrefab;
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        [SerializeField] private AudioMixer gameMixer;

        [Inject] LevelAsset levelAsset;

        public override void InstallBindings()
        {
            Container.BindInstance(audioMixerGroup);
            Container.BindInstance(gameMixer);
            Container.Bind<SoundPlayer>()
                     .AsSingle();

            Container.BindInstance(new GameCameraState());
            Container.Bind<Camera>()
                     .FromComponentInNewPrefab(cameraControllerPrefab)
                     .AsSingle()
                     .NonLazy();

            Container.Bind<GameState>()
                     .AsSingle()
                     .NonLazy();

            Container.DeclareSignal<TimeSlowSignal>();
            Container.BindInterfacesAndSelfTo<TimeController>()
                     .AsSingle()
                     .NonLazy();

            Container.Bind<PlayerCharacterFactory>()
                     .AsSingle()
                     .NonLazy();
            Container.Bind<SpawnPointGroup>()
                     .FromComponentInNewPrefab(levelAsset.prefab)
                     .AsSingle()
                     .NonLazy();

            Container.BindInterfacesAndSelfTo<DebugCommandController>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}