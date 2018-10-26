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

    [Obsolete]
    public class KeyPlayerController : MonoBehaviour
    {
        [SerializeField] KeyMapping keyMapping;

        [Inject] BodyRoot bodyRoot;
        [Inject] SignalBus signalBus;
        [Inject] private CharacterState state;

        private void Start()
        {
            state.localPlayerControl
                 .TakeUntilDestroy(this)
                 .Subscribe(localControl => enabled = localControl);

            bool rightDown = false;
            bool leftDown = false;

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Select(_ => Input.GetKey(keyMapping.right))
                      .Where(_ => enabled)
                      .Subscribe(isDown => rightDown = isDown);
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Select(_ => Input.GetKey(keyMapping.left))
                      .Where(_ => enabled)
                      .Subscribe(isDown => leftDown = isDown);

            bool upDown = false;
            bool downDown = false;

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Select(_ => Input.GetKey(keyMapping.up))
                      .Where(_ => enabled)
                      .Subscribe(isDown => upDown = isDown);
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Select(_ => Input.GetKey(keyMapping.down))
                      .Where(_ => enabled)
                      .Subscribe(isDown => downDown = isDown);

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => enabled)
                      .Subscribe(_ =>
                      {
                          Vector3 direction = (upDown) ? Vector3.up : (downDown) ? Vector3.down : Vector3.zero;
                          direction += (leftDown) ? Vector3.left : (rightDown) ? Vector3.right : Vector3.zero;
                          direction = (direction == Vector3.zero) ? Vector3.up : direction;
                          state.aimDirection.Value = direction;
                      });

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => Input.GetKeyDown(keyMapping.fire))
                      .Where(_ => enabled)
                      .Subscribe(_ =>
                      {
                          signalBus.Fire<GrapplingFiredSignal>();
                      });

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => Input.GetKeyUp(keyMapping.fire))
                      .Where(_ => enabled)
                      .Subscribe(_ => signalBus.Fire<GrapplingReleasedSignal>());
        }

        public void Disable()
        {
            enabled = false;
        }
    }
}