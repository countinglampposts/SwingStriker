using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;
using Character;
using System;

namespace Player
{
    [System.Serializable]
    public struct KeyMapping{
        public KeyCode left;
        public KeyCode right;
        public KeyCode fire;
    }

    public class KeyController : MonoBehaviour
    {
        [SerializeField] KeyMapping keyMapping;

        [Inject] BodyRoot bodyRoot;
        [Inject] SignalBus signalBus;

        private void Start()
        {
            bool rightDown = false;
            bool leftDown = false;

            Observable.EveryUpdate()
                      .Select(_ => Input.GetKey(keyMapping.right))
                      .TakeUntilDestroy(this)
                      .Subscribe(isDown => rightDown = isDown);
            Observable.EveryUpdate()
                      .Select(_ => Input.GetKey(keyMapping.left))
                      .TakeUntilDestroy(this)
                      .Subscribe(isDown => leftDown = isDown);

            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(keyMapping.fire))
                      .TakeUntilDestroy(this)
                      .Subscribe(_ =>
                      {
                Vector3 direction = Vector3.up;
                          direction += (leftDown) ? Vector3.left : (rightDown) ? Vector3.right : Vector3.zero;
                          signalBus.Fire(new GrapplingFiredSignal() { direction = direction });
                      });

            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyUp(keyMapping.fire))
                      .TakeUntilDestroy(this)
                      .Subscribe(_ => signalBus.Fire<GrapplingReleasedSignal>());
        }
    }
}