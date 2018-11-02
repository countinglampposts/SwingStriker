using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swing.Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Game.Soccer
{

    [System.Serializable]
    public struct SpawnPoint{
        public int team;
        public Transform spawnPoint;
    }

    public class SoccerLevelInstaller : MonoInstaller
    {
        [SerializeField]
        private GameObject ballPrefab;
        [SerializeField]
        private Transform ballReset;

        public override void InstallBindings()
        {
            Container.Bind<Ball>()
                     .FromComponentInNewPrefab(ballPrefab)
                     .AsSingle()
                     .WithArguments(ballReset)
                     .NonLazy();
        }

        private void OnDrawGizmos()
        {
            if(ballReset != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(ballReset.position, .5f);
                Gizmos.color = Color.white;
            }
        }
    }
}