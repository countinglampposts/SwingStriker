using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Swing.UI
{
    public class AssetScroller : MonoBehaviour
    {
        [SerializeField]
        Button rightButton;
        [SerializeField]
        Button leftButton;
        [SerializeField]
        Text text;

        private ReactiveProperty<int> index = new ReactiveProperty<int>();
        private int usedIndex = 0;


        public void Init(IScrollableAsset scrollableAsset)
        {
            rightButton.onClick.AddListener(() => index.Value++);
            leftButton.onClick.AddListener(() => index.Value--);

            var assets = new List<IDisplayAsset>(scrollableAsset.assets);
            index
                .TakeUntilDestroy(this)
                .Subscribe(currentIndex =>
                {
                    usedIndex = (int)Mathf.Repeat(currentIndex, assets.Count);
                    text.text = assets[usedIndex].displayName;
                });

            index.Value = 0;
        }

        public int CurrentIndex(){
            return usedIndex;
        }
    }
}