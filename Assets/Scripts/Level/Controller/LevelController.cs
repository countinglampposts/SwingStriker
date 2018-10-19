using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System;
using Swing.Game;

namespace Swing.Level
{
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
            signalBus.GetStream<GoalScoredSignal>()
                     .Subscribe(_ =>
                     {
                        Observable.Timer(TimeSpan.FromSeconds(5))
                               .Subscribe(__ =>
                               {
                                   gameBall.gameObject.transform.position = gameBall.restartPoint.position;
                                   var ridigBody = gameBall.gameObject.GetComponent<Rigidbody2D>();
                                   ridigBody.velocity = Vector2.zero;
                                   ridigBody.angularVelocity = 0;
                                   signalBus.Fire<BallResetSignal>();
                               });
                     })
                     .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}