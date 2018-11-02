using System.Collections;
using System.Collections.Generic;
using Swing.Level;
using Swing.Player;
using Swing.Sound;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Swing.Game
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameCameraController cameraControllerPrefab;
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        [SerializeField] private bool autoLaunch;

        [Inject] LevelAsset levelAsset;

        private void Start()
        {
            if(autoLaunch)Container.Resolve<PlayerLifeController>().InitializePlayer(Container.Resolve<PlayerData>());
        }

        public override void InstallBindings()
        {
            Container.BindInstance(audioMixerGroup);
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

            Container.Bind<SpawnPointGroup>()
                     .FromComponentsInNewPrefab(levelAsset.prefab)
                     .AsSingle()
                     .NonLazy();
            Container.Bind<PlayerLifeController>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}