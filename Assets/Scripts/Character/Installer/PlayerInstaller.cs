using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Characters
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField]
        private Camera playerCamera;
        [SerializeField]
        private BodyRoot root;
        [SerializeField]
        private CharacterSettings settings;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<GrapplingFiredSignal>();
            Container.DeclareSignal<GrapplingReleasedSignal>();

            Container.BindInstance(settings).AsSingle();

            Container.Bind<Camera>().FromInstance(playerCamera);
            Container.Bind<BodyRoot>().FromInstance(root);
        }
    }
}