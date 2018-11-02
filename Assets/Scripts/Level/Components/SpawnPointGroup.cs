using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swing.Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Level
{
    public class SpawnPointGroup : MonoBehaviour
    {
        [SerializeField]
        public SpawnPoint[] spawnPoints;

        public void ResolvePlayerSpawn(GameObject spawned)
        {
            spawned.transform.position = spawnPoints.First(sp => sp.isStart).transform.position;
        }

        public void ResolvePlayerSpawn(List<Tuple<PlayerData, GameObject>> spawned)
        {
            List<SpawnPoint> availiblePoint = new List<SpawnPoint>(spawnPoints);
            foreach (var spawnPoint in spawnPoints)
            {
                var hit = Physics2D.CircleCast(spawnPoint.transform.position, .5f, Vector2.up, 0f);
                if (hit.collider != null)
                {
                    availiblePoint.Remove(spawnPoint);
                }
            }
            foreach (var player in spawned)
            {
                var data = player.Item1;
                var gameObject = player.Item2;
                var spawnPoint = availiblePoint.First(element => element.team == data.team.id);
                gameObject.transform.position = spawnPoint.transform.position;
                availiblePoint.Remove(spawnPoint);
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var a in spawnPoints)
            {
                if (a != null) Gizmos.DrawWireSphere(a.transform.position, .5f);
            }
        }
    }
}