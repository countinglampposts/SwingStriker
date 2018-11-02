using System;
using System.Collections;
using System.Collections.Generic;
using Swing.Character;
using Swing.Level;
using Swing.Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Game
{
    public class BasicGameController : MonoBehaviour
    {
        [Inject] private LevelAsset levelAsset;
        [Inject] DiContainer container;
        [Inject] PlayerData playerData;
        [Inject] GameState gameState;

        private void Start()
        {
            // Init the level
            var spawnPoints = container.InstantiatePrefab(levelAsset.prefab).GetComponent<SpawnPointGroup>();

            ProjectUtils.InitializePlayer(playerData, spawnPoints, gameState, container);
        }
    }
}