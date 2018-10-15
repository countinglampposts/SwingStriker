using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Level
{
    [System.Serializable]
    public struct GameBall{
        public GameObject gameObject;
        public Transform restartPoint;
    }

    public class LevelInstaller : MonoInstaller
    {
        [SerializeField]
        private GameBall ball;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<GoalScoredSignal>();
            Container.DeclareSignal<BallResetSignal>();
            Container.DeclareSignal<RestartLevelSignal>();

            Container.BindInterfacesAndSelfTo<LevelController>()
                     .AsSingle()
                     .NonLazy();

            Container.BindInstance(ball);
        }
    }
}