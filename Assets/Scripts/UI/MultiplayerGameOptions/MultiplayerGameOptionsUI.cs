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
using UniRx;

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

        [Inject] LevelCollection levels;
        [Inject] LevelTimeOptions levelTimeOptions;
        [Inject] DiContainer container;

        private void Start()
        {
            levelSelectMenu.scroller.Init(levels);
            levelSelectMenu.scroller.BindToAllDevices();

            UIUtils.BindToAllDevices(levelSelectMenu.nextButton,0);
            levelSelectMenu.nextButton.onClick.AsObservable()
                          .TakeUntilDestroy(this)
                          .Subscribe(_ =>
                          {
                              var level = levels.levels[levelSelectMenu.scroller.CurrentIndex()];
                              container.Unbind(level.GetType());
                              container.BindInstance(level);

                              levelSelectMenu.root.SetActive(false);
                              timeSelectMenu.root.SetActive(true);
                          });

            timeSelectMenu.scroller.Init(levelTimeOptions);
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
                              var levelTime = levelTimeOptions.levelTimes[timeSelectMenu.scroller.CurrentIndex()];
                              container.Unbind(levelTime.GetType());
                              container.BindInstance(levelTime);

                              gameObject.SetActive(false);
                              nextUI.SetActive(true);
                          });
        }
    }
}