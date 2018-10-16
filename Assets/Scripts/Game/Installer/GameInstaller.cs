using Zenject;
using System.Linq;
using UnityEngine;
using Swing.Level;
using Swing.Player;
using System.Collections.Generic;

namespace Swing.Game
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField]
        private LevelInstaller levelPrefab;
        [SerializeField]
        private PlayerData[] playersData;
        [SerializeField]
        private TeamData[] teams;
        [SerializeField]
        private SplitScreenLayout[] layouts;

        public override void InstallBindings()
        {
            var levelContext = Container.CreateSubContainer();
            levelContext.BindInstance(teams);
            var level = levelContext.InstantiatePrefab(levelPrefab).GetComponent<LevelInstaller>();

            var layout = layouts.First(a => a.settings.Length == playersData.Length);
            List<GameObject> spawned = new List<GameObject>();

            for (int a = 0; a < layout.settings.Length;a++){
                var playerData = playersData[a];
                var playerContext = Container.CreateSubContainer();
                playerContext.BindInstance(teams.First(element => element.id == playerData.team));
                playerContext.BindInstance(layout.settings[a]);
                spawned.Add(playerContext.InstantiatePrefab(playerData.prefab));
            }
            level.ResolvePlayerSpawn(spawned.ToArray());
        }
    }
}