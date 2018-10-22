using System.Collections;
using System.Collections.Generic;
using Swing.Player;
using UnityEngine;
using Zenject;

namespace Swing.Character
{
    public class CharacterInstaller : MonoInstaller
    {
        [SerializeField]
        private CameraController playerCameraPrefab;
        [SerializeField]
        private BodyRoot root;
        [SerializeField]
        private CharacterSettings settings;

        public override void InstallBindings()
        {
            Container.DeclareSignal<GrapplingFiredSignal>();
            Container.DeclareSignal<GrapplingReleasedSignal>();
            Container.DeclareSignal<SuicideSignal>();
            Container.DeclareSignal<RumbleTriggeredSignal>();

            Container.BindInstance(settings)
                     .AsSingle();

            Container.Bind<BodyRoot>()
                     .FromInstance(root)
                     .AsSingle();

            // This is to activate split screen view
            /*var instance = Container.InstantiatePrefab(playerCameraPrefab);
            Container.BindInstance(instance.GetComponentInChildren<Camera>());*/
        }
    }
}