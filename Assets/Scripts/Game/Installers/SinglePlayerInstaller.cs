using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Swing.Game
{
    public class LevelWonSignal{}
    public class LevelLostSignal{}

    public class SinglePlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<LevelWonSignal>(); // Load the next level
            Container.DeclareSignal<LevelLostSignal>(); // Reload the level

            Container.BindInterfacesAndSelfTo<SinglePlayerController>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}