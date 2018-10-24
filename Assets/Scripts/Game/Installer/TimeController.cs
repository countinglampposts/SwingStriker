using System.Collections;
using System.Collections.Generic;
using InControl;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Game
{
    public struct TimeSlowSignal{
        public float slowTime;
    }

    public class TimeController : MonoInstaller
    {
        [Inject] private GameState state;

        public override void InstallBindings()
        {
            Container.DeclareSignal<TimeSlowSignal>();
        }

        private void Start()
        {
            var signalBus = Container.Resolve<SignalBus>();

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => InputManager.ActiveDevice.CommandWasPressed || Input.GetKeyDown(KeyCode.P))
                      .Subscribe(_ => state.isPaused.Value = !state.isPaused.Value);

            float goalTimeScale = 1f;
            float endSlowdownTime = 0f;
            state.isPaused
                 .TakeUntilDestroy(this)
                 .Subscribe(_ => Time.timeScale = goalTimeScale);
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Select(_ => (state.isPaused.Value) ? 0 : Mathf.Lerp(Time.timeScale, (Time.time < endSlowdownTime) ? goalTimeScale : 1, 8 * Time.deltaTime))
                      .Subscribe(timeScale => Time.timeScale = timeScale);
            signalBus.GetStream<TimeSlowSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(signal => {
                         goalTimeScale = .4f;
                         endSlowdownTime = Time.time + signal.slowTime;
            });

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => Input.GetKeyDown(KeyCode.T))
                      .Subscribe(_ => signalBus.Fire(new TimeSlowSignal { slowTime = 2f }));
        }
    }
}