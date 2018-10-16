using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;
using Swing.Character;
using System;

namespace Swing.Player
{
    [System.Serializable]
    public struct KeyMapping{
        public KeyCode left;
        public KeyCode right;
        public KeyCode up;
        public KeyCode down;
        public KeyCode fire;
    }

    public class KeyPlayerController : MonoBehaviour, ICharacterDriver
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
                      .Where(_ => enabled)
                      .TakeUntilDestroy(this)
                      .Subscribe(isDown => rightDown = isDown);
            Observable.EveryUpdate()
                      .Select(_ => Input.GetKey(keyMapping.left))
                      .Where(_ => enabled)
                      .TakeUntilDestroy(this)
                      .Subscribe(isDown => leftDown = isDown);

            bool upDown = false;
            bool downDown = false;

            Observable.EveryUpdate()
                      .Select(_ => Input.GetKey(keyMapping.up))
                      .Where(_ => enabled)
                      .TakeUntilDestroy(this)
                      .Subscribe(isDown => upDown = isDown);
            Observable.EveryUpdate()
                      .Select(_ => Input.GetKey(keyMapping.down))
                      .Where(_ => enabled)
                      .TakeUntilDestroy(this)
                      .Subscribe(isDown => downDown = isDown);

            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(keyMapping.fire))
                      .Where(_ => enabled)
                      .TakeUntilDestroy(this)
                      .Subscribe(_ =>
                      {
                          Vector3 direction = (upDown) ? Vector3.up : (downDown) ? Vector3.down : Vector3.zero;
                          direction += (leftDown) ? Vector3.left : (rightDown) ? Vector3.right : Vector3.zero;
                          direction = (direction == Vector3.zero) ? Vector3.up : direction;
                          signalBus.Fire(new GrapplingFiredSignal() { direction = direction });
                      });

            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyUp(keyMapping.fire))
                      .Where(_ => enabled)
                      .TakeUntilDestroy(this)
                      .Subscribe(_ => signalBus.Fire<GrapplingReleasedSignal>());
        }

        public void Disable()
        {
            enabled = false;
        }
    }
}