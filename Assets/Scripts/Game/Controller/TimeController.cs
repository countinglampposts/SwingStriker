using System;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Swing.Game
{
    public struct TimeSlowSignal{
        public float slowTime;
    }

    /// <summary>
    /// This owns: 
    ///  - Time.timescale 
    ///  - "GamePitch" property of the game AudioMixer 
    /// It listens to:
    ///  - TimeSlowSignal to slow time
    ///  - GameState.isPaused to pause time
    /// It also listens to the pause key press
    /// </summary>
    public class TimeController : IInitializable, IDisposable
    {
        [Inject] private AudioMixer gameMixer;
        [Inject] private GameState state;
        [Inject] private SignalBus signalBus;

        private CompositeDisposable disposables = new CompositeDisposable();

        public void Initialize()
        {
            Observable.EveryUpdate()
                      .Where(_ => InputManager.ActiveDevice.CommandWasPressed || Input.GetKeyDown(KeyCode.Escape))
                      .Subscribe(_ => state.isPaused.Value = !state.isPaused.Value)
                      .AddTo(disposables);

            float goalTimeScale = 1f;
            float endSlowdownTime = 0f;
            state.isPaused
                 .Subscribe(_ => Time.timeScale = goalTimeScale)
                 .AddTo(disposables);
            Observable.EveryUpdate()
                      .Select(_ => (state.isPaused.Value) ? 0 : Mathf.SmoothStep(Time.timeScale, (Time.time < endSlowdownTime) ? goalTimeScale : 1, 8 * Time.deltaTime))
                      .Subscribe(timeScale => {
                          gameMixer.SetFloat("GamePitch", timeScale);
                          Time.timeScale = timeScale; 
                      })
                      .AddTo(disposables);
            signalBus.GetStream<TimeSlowSignal>()
                     .Subscribe(signal => {
                         goalTimeScale = .4f;
                         endSlowdownTime = Time.time + signal.slowTime;
                     })
                     .AddTo(disposables);

            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(KeyCode.T))
                      .Subscribe(_ => signalBus.Fire(new TimeSlowSignal { slowTime = 2f }))
                      .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}