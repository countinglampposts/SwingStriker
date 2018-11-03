using System.Collections;
using System.Collections.Generic;
using Swing.Level;
using Swing.Player;
using UnityEngine;
using Zenject;

namespace Swing.Game
{
    public class LevelsController : MonoBehaviour
    {
        [SerializeField]
        private GameObject singlePlayerPrefab;
        [SerializeField]
        private GameObject soccerGamePrefab;

        private GameObject currentLevelPrefab;

        public void LaunchLevel(DiContainer container, LevelAsset levelAsset, LevelCollection levelCollection, PlayerData[] playerData)
        {
            Destroy(currentLevelPrefab);

            var levelSubcontainer = container.CreateSubContainer();
            levelSubcontainer.BindInstance(levelAsset);
            levelSubcontainer.BindInstance(levelCollection);
            levelSubcontainer.BindInstance(playerData);
            currentLevelPrefab = levelSubcontainer.InstantiatePrefab(singlePlayerPrefab);
        }
    }
}