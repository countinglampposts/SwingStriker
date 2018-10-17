using System.Collections;
using System.Collections.Generic;
using Swing.Level;
using UnityEngine;
using Zenject;

namespace Swing.UI
{

    public class MultiplayerGameOptionsUI : MonoBehaviour
    {
        [SerializeField]
        private AssetScroller levelScroller;
        [SerializeField]
        private AssetScroller timeScroller;

        [Inject] LevelCollection levels;
        [Inject] LevelTimeOptions levelTimeOptions;
        [Inject] DiContainer container;

        private void Start()
        {
            Debug.Log(levels);
            Debug.Log(levelTimeOptions);
            levelScroller.Init(levels);
            timeScroller.Init(levelTimeOptions);
        }
    }
}