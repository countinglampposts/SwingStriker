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
    /// <summary>
    /// This is a generic way of showing all assets listed in IScrollableAsset
    /// All displayed assets need to implement IDisplayAsset
    /// This can be bound to a gamepad device or all devices
    /// </summary>
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
        private CompositeDisposable disposables = new CompositeDisposable();

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

        public IDisposable BindToAllDevices()
        {
            return BindToDevice(Guid.Empty);
        }

        public IDisposable BindToDevice(Guid deviceID)
        {
            disposables = new CompositeDisposable();
            Observable.EveryUpdate()
                      .Where(_ => gameObject.activeInHierarchy && gameObject.activeSelf)
                      .Select(_ => (deviceID == Guid.Empty) ? InputManager.ActiveDevice : InputManager.Devices.FirstOrDefault(device => device.GUID == deviceID))
                      .Where(device => device != null)
                      .Select(device => device.RightStickX < -.9f || device.LeftStickX < -.9f || device.DPadX < -.9f)
                      .DistinctUntilChanged()
                      .Where(isTriggered => isTriggered)
                      .Subscribe(_ => leftButton.onClick.Invoke())
                      .AddTo(disposables);

            Observable.EveryUpdate()
                      .Where(_ => gameObject.activeInHierarchy && gameObject.activeSelf)
                      .Select(_ => (deviceID == Guid.Empty) ? InputManager.ActiveDevice : InputManager.Devices.FirstOrDefault(device => device.GUID == deviceID))
                      .Where(device => device != null)
                      .Select(device => device.RightStickX > .9f || device.LeftStickX > .9f || device.DPadX > .9f)
                      .DistinctUntilChanged()
                      .Where(isTriggered => isTriggered)
                      .Subscribe(_ => rightButton.onClick.Invoke())
                      .AddTo(disposables);

            return disposables;
        }

        public int CurrentIndex()
        {
            return usedIndex;
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}