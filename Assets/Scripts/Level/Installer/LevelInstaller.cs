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
    public struct SpawnPoint{
        public int team;
        public Transform spawnPoint;
    }

    public class LevelInstaller : MonoInstaller
    {
        [SerializeField]
        private GameObject ballPrefab;
        [SerializeField]
        private Transform ballReset;
        [SerializeField]
        public SpawnPoint[] spawnPoints;

        public override void InstallBindings()
        {
            Container.Bind<Ball>()
                     .FromComponentInNewPrefab(ballPrefab)
                     .AsSingle()
                     .WithArguments(ballReset)
                     .NonLazy();
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
                var spawnPoint = availiblePoint.First(element => element.team == data.team.id);
                gameObject.transform.position = spawnPoint.spawnPoint.position;
                availiblePoint.Remove(spawnPoint);
            }
        }

        private void OnDrawGizmos()
        {
            foreach(var a in spawnPoints){
                if(a.spawnPoint != null) Gizmos.DrawWireSphere(a.spawnPoint.position, .5f);
            }
            if(ballReset != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(ballReset.position, .5f);
                Gizmos.color = Color.white;
            }
        }
    }
}