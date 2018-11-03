using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Game
{
    public class DebugCommandController : IInitializable, IDisposable
    {
        [Inject] private SignalBus signalBus;
        CompositeDisposable disposables = new CompositeDisposable();

        public void Initialize()
        {
            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(KeyCode.R))
                      .Subscribe(_ => ProjectUtils.ReturnToMainMenu())
                      .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

    }
}