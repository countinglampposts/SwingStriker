using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swing.Player;
using UniRx;
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
            Container.DeclareSignal<GoalScoredSignal>();
            Container.DeclareSignal<BallResetSignal>();
            Container.DeclareSignal<RestartLevelSignal>();

            Container.BindInterfacesAndSelfTo<LevelController>()
                     .AsSingle()
                     .NonLazy();

            Container.BindInstance(ball);
        }

        public void ResolvePlayerSpawn(List<Tuple<PlayerData,GameObject>> spawned){
            List<SpawnPoint> availiblePoint = new List<SpawnPoint>(spawnPoints);
            foreach(var spawnPoint in spawnPoints){
                var hit = Physics2D.CircleCast(spawnPoint.spawnPoint.position, .5f, Vector2.up,0f);
                if (hit.collider != null){
                    availiblePoint.Remove(spawnPoint);
                }
            }
            foreach(var player in spawned){  
                var data = player.Item1;
                var gameObject = player.Item2;
                var spawnPoint = availiblePoint.First(element => element.team == data.team);
                gameObject.transform.position = spawnPoint.spawnPoint.position;
                availiblePoint.Remove(spawnPoint);
            }
        }

        private void OnDrawGizmos()
        {
            foreach(var a in spawnPoints){
                if(a.spawnPoint != null) Gizmos.DrawWireSphere(a.spawnPoint.position, .5f);
            }
        }
    }
}