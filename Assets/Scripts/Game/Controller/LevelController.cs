using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System;

namespace Swing.Level
{
    public class RestartLevelSignal { };

    public class LevelController :IInitializable, IDisposable
    {
        [Inject] SignalBus signalBus;
        [Inject] GameBall gameBall;

        CompositeDisposable disposables = new CompositeDisposable();

        private void OnGUI()
        {
            GUILayout.Label("Press R to restart");
        }

        public void Initialize()
        {
            Observable.EveryUpdate()
                      .Where(_=> Input.GetKey(KeyCode.R))
                      .Subscribe(_=> signalBus.Fire<RestartLevelSignal>())
                      .AddTo(disposables);

            signalBus.GetStream<GoalScoredSignal>()
                     .Subscribe(_ =>
                     {
                        UniRx.Observable.Timer(TimeSpan.FromSeconds(5))
                               .Subscribe(__ =>
                               {
                                   gameBall.gameObject.transform.position = gameBall.restartPoint.position;
                                   signalBus.Fire<BallResetSignal>();
                               });
                     })
                     .AddTo(disposables);

            signalBus.GetStream<RestartLevelSignal>()
                     .Subscribe(_=> Application.LoadLevel(Application.loadedLevel))
                     .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}