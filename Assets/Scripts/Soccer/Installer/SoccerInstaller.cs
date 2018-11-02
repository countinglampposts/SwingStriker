using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swing.Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Game.Soccer
{
    public class SoccerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<GameEndsSignal>();
            Container.DeclareSignal<GoalScoredSignal>();
            Container.DeclareSignal<BallResetSignal>();

            Container.Bind<SoccerState>()
                     .AsSingle()
                     .NonLazy();

            Container.BindInterfacesAndSelfTo<SoccerController>()
                     .AsSingle()
                     .NonLazy();
        }
    }
}