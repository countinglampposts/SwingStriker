using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swing.Level;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using Swing.Player;
using System;
using InControl;
using Swing.Game;

namespace Swing.UI
{
    public class SinglePlayerUI : MonoBehaviour
    {
        [SerializeField]
        private AssetScroller levelScroller;
        [SerializeField]
        private Button playButton;
        [SerializeField]
        private Button backButton;
        [SerializeField]
        private GameObject root;
        [SerializeField]
        private GameObject backRoot;
        [SerializeField]
        private GameObject gamePrefab;

        [Inject] LevelCollection[] levels;
        [Inject] DiContainer container;
        [Inject] LevelsController levelsController; 

        private void Start()
        {
            var levelCollection = levels.First(l => l.id == "SP");
            levelScroller.Init(levelCollection);
            playButton.onClick.AsObservable()
                  .TakeUntilDestroy(this)
                  .Subscribe(_ =>
                  {
                      var levelAsset = levelCollection.levels[levelScroller.CurrentIndex()];
                      var playerData = new PlayerData { deviceID = InputManager.ActiveDevice.GUID, character = levelAsset.defaultCharacter };

                      levelsController.levelSubcontainer.BindInstance(levelAsset);
                      levelsController.levelSubcontainer.BindInstance(new[] { playerData });
                      levelsController.levelSubcontainer.BindInstance(levelCollection);
                      levelsController.LaunchCampaign(0);

                      root.SetActive(false);
                  });

            backButton.onClick.AsObservable()
                      .TakeUntilDestroy(this)
                      .Subscribe(_ =>
                      {
                          root.SetActive(false);
                          backRoot.SetActive(true);
                      });

            UIUtils.BindToAllDevices(playButton, 0);
            UIUtils.BindToAllDevices(backButton, 1);
        }
    }
}