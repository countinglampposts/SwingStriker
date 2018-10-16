using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Swing.Level
{
    [System.Serializable]
    public struct GameBall{
        public GameObject gameObject;
        public Transform restartPoint;
    }

    [System.Serializable]
    public struct SpawnPoint{
        public int team;
        public Transform spawnPoint;
    }

    public class LevelInstaller : MonoInstaller
    {
        [SerializeField]
        public SpawnPoint[] spawnPoints;
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

        public void OnDrawGizmos()
        {
            foreach(var a in spawnPoints){
                if(a.spawnPoint != null) Gizmos.DrawWireSphere(a.spawnPoint.position, .5f);
            }
        }
    }
}