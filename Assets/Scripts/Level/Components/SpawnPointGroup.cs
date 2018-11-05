using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swing.Game;
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

        [Inject]
        public PlayerCharacterFactory playerCharacterFactory;

        public void SpawnPlayerAtStart(PlayerData spawnedPlayerData)
        {
            var spawned = playerCharacterFactory.SpawnPlayerCharacter(spawnedPlayerData);
            spawned.transform.position = spawnPoints.First(sp => sp.isStart).transform.position;
        }

        public void ResolvePlayerSpawn(params PlayerData[] playerDatas)
        {
            // Init the players
            var spawned = new List<Tuple<PlayerData, GameObject>>();

            for (int a = 0; a < playerDatas.Length; a++)
            {
                var playerData = playerDatas[a];
                var instance = playerCharacterFactory.SpawnPlayerCharacter(playerData);

                spawned.Add(new Tuple<PlayerData, GameObject>(playerData, instance));
            }

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