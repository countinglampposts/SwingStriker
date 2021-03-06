﻿using System.Collections;
using System.Collections.Generic;
using InControl;
using Swing.Character;
using Swing.Game;
using Swing.Level;
using Swing.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using System.Linq;

namespace Swing.UI
{

    public class MultiplayerGameOptionsUI : MonoBehaviour
    {
        [System.Serializable]
        public class LevelSelectMenu
        {
            public GameObject root;
            public AssetScroller scroller;
            public Button nextButton;
        }
        [System.Serializable]
        public class TimeSelectMenu
        {
            public GameObject root;
            public AssetScroller scroller;
            public Button nextButton;
            public Button backButton;
        }
        [SerializeField]
        private LevelSelectMenu levelSelectMenu;
        [SerializeField]
        private TimeSelectMenu timeSelectMenu;
        [SerializeField]
        private GameObject nextUI;

        [Inject] LevelCollection[] levelCollections;
        [Inject] GameTimeOptions gameTimeOptions;
        [Inject] DiContainer container;
        [Inject] LevelsController levelsController;

        private void Start()
        {
            var levels = levelCollections.First(l => l.id == "MP");
            levelSelectMenu.scroller.Init(levels);
            levelSelectMenu.scroller.BindToAllDevices();

            UIUtils.BindToAllDevices(levelSelectMenu.nextButton,0);
            levelSelectMenu.nextButton.onClick.AsObservable()
                          .TakeUntilDestroy(this)
                          .Subscribe(_ =>
                          {
                              var level = levels.levels[levelSelectMenu.scroller.CurrentIndex()];
                              levelsController.levelSubcontainer.Unbind(level.GetType());
                              levelsController.levelSubcontainer.BindInstance(level);

                              levelSelectMenu.root.SetActive(false);
                              timeSelectMenu.root.SetActive(true);
                          });

            timeSelectMenu.scroller.Init(gameTimeOptions);
            timeSelectMenu.scroller.BindToAllDevices();

            UIUtils.BindToAllDevices(timeSelectMenu.backButton, 1);
            timeSelectMenu.backButton.onClick.AsObservable()
                          .TakeUntilDestroy(this)
                          .Subscribe(_ =>
                          {
                              timeSelectMenu.root.SetActive(false);
                              levelSelectMenu.root.SetActive(true);
                          });

            UIUtils.BindToAllDevices(timeSelectMenu.nextButton,0);
            timeSelectMenu.nextButton.onClick.AsObservable()
                          .TakeUntilDestroy(this)
                          .Subscribe(_ =>
                          {
                              var gameTime = gameTimeOptions.levelTimes[timeSelectMenu.scroller.CurrentIndex()];
                              levelsController.levelSubcontainer.Unbind(gameTime.GetType());
                              levelsController.levelSubcontainer.BindInstance(gameTime);

                              gameObject.SetActive(false);
                              nextUI.SetActive(true);
                          });
        }
    }
}