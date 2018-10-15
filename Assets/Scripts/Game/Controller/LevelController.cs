using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System;

namespace Level
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

            signalBus.Subscribe<GoalScoredSignal>(GoalScoredHandler);
            signalBus.Subscribe<RestartLevelSignal>(RestartLevel);
        }

        public void Dispose()
        {
            disposables.Dispose();
            signalBus.Unsubscribe<GoalScoredSignal>(GoalScoredHandler);
            signalBus.Unsubscribe<RestartLevelSignal>(RestartLevel);
        }

        private void RestartLevel(){
            Application.LoadLevel(Application.loadedLevel);
        }

        private void GoalScoredHandler()
        {
            Observable.Timer(TimeSpan.FromSeconds(5))
                      .Subscribe(_ =>
                      {
                          gameBall.gameObject.transform.position = gameBall.restartPoint.position;
                          signalBus.Fire<BallResetSignal>();
                      });
        }
    }
}