using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using InControl;
using UniRx.Diagnostics;
using System.Linq;
using System;

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

        public void InitController(Guid deviceID)
        {
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => enabled)
                      .Select(_ => InputManager.Devices.First(device => device.GUID == deviceID))
                      .Where(device => device != null)
                      .Select(device => device.RightStickX < -.9f || device.LeftStickX < -.9f || device.DPadX < -.9f)
                      .DistinctUntilChanged()
                      .Where(isTriggered => isTriggered)
                      .Subscribe(_ => leftButton.onClick.Invoke());

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => enabled)
                      .Select(_ => InputManager.Devices.First(device => device.GUID == deviceID))
                      .Where(device => device != null)
                      .Select(device => device.RightStickX > .9f || device.LeftStickX > .9f || device.DPadX > .9f)
                      .DistinctUntilChanged()
                      .Where(isTriggered => isTriggered)
                      .Subscribe(_ => rightButton.onClick.Invoke());
        }

        public int CurrentIndex(){
            return usedIndex;
        }
    }
}