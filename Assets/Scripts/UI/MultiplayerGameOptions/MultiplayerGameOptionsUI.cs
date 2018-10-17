using System.Collections;
using System.Collections.Generic;
using InControl;
using Swing.Character;
using Swing.Game;
using Swing.Level;
using Swing.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Swing.UI
{

    public class MultiplayerGameOptionsUI : MonoBehaviour
    {
        [SerializeField]
        private AssetScroller levelScroller;
        [SerializeField]
        private AssetScroller timeScroller;
        [SerializeField]
        private Button playButton;
        [SerializeField]
        private CharacterAsset characterAsset;
        [SerializeField]
        private GameController gamePrefab;

        [Inject] LevelCollection levels;
        [Inject] LevelTimeOptions levelTimeOptions;
        [Inject] DiContainer container;

        private void Start()
        {
            levelScroller.Init(levels);
            timeScroller.Init(levelTimeOptions);

            playButton.onClick.AddListener(() =>
            {
                var gameContext = container.CreateSubContainer();
                gameContext.BindInstance(levels.levels[levelScroller.CurrentIndex()]);
                gameContext.BindInstance(levelTimeOptions.levelTimes[timeScroller.CurrentIndex()]);

                List<PlayerData> playersData = new List<PlayerData>();
                for (int a = 0; a < InputManager.Devices.Count;a++){
                    playersData.Add(new PlayerData { team = a%2, character = characterAsset });
                }
                gameContext.BindInstance(playersData.ToArray());

                gameContext.InstantiatePrefab(gamePrefab);

                gameObject.SetActive(false);
            });
        }
    }
}