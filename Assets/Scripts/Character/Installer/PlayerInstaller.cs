using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Characters
{

    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<GrapplingFiredSignal>();
            Container.DeclareSignal<GrapplingReleasedSignal>();

            Container.Bind<MousePlayerController>().AsSingle().NonLazy();
        }
    }
}