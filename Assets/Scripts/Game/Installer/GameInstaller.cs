using Zenject;
using System.Linq;
using UnityEngine;
using Swing.Level;

namespace Swing.Game
{
    [System.Serializable]
    public struct SplitScreenLayout{
        public CameraSettings[] settings;
    }

    [System.Serializable]
    public struct PlayerData
    {
        public GameObject prefab;
    }

    public class GameInstaller : MonoInstaller
    {
        [SerializeField]
        LevelInstaller level;
        [SerializeField]
        private PlayerData[] playersData;
        [SerializeField]
        private SplitScreenLayout[] layouts;

        public override void InstallBindings()
        {
            var layout = layouts.First(a => a.settings.Length == playersData.Length);
            for (int a = 0; a < layout.settings.Length;a++){
                var playerData = playersData[a];
                var subContainer = Container.CreateSubContainer();
                subContainer.BindInstance(layout.settings[a]);
                subContainer.InstantiatePrefab(playerData.prefab, level.spawnPoints[a].spawnPoint.position, Quaternion.identity, null);            
            }
        }
    }
}