using System.Collections;
using System.Collections.Generic;
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
        }
    }
}