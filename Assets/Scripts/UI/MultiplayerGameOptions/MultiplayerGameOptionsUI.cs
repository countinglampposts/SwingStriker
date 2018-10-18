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
        private GameObject nextUI;

        [Inject] LevelCollection levels;
        [Inject] LevelTimeOptions levelTimeOptions;
        [Inject] DiContainer container;

        private void Start()
        {
            levelScroller.Init(levels);
            timeScroller.Init(levelTimeOptions);

            playButton.onClick.AddListener(() =>
            {
                var level = levels.levels[levelScroller.CurrentIndex()];
                container.Unbind(level.GetType());
                container.BindInstance(level);

                var levelTime = levelTimeOptions.levelTimes[timeScroller.CurrentIndex()];
                container.Unbind(levelTime.GetType());
                container.BindInstance(levelTime);

                gameObject.SetActive(false);
                nextUI.SetActive(true);
            });
        }
    }
}