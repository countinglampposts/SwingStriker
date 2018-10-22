using System.Collections;
using System.Collections.Generic;
using InControl;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Game
{
    public class TimeController : MonoBehaviour
    {
        [Inject] private GameState state;
        private void Start()
        {
            state.isPaused
                .TakeUntilDestroy(this)
                .Subscribe(isPaused => Time.timeScale = (isPaused) ? 0 : 1);

            Observable.EveryUpdate()
                      .Where(_ => InputManager.ActiveDevice.CommandWasPressed || Input.GetKeyDown(KeyCode.P))
                      .Subscribe(_ => state.isPaused.Value = !state.isPaused.Value);
        }
    }
}