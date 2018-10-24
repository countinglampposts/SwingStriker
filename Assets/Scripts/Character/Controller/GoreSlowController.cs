using System;
using System.Collections;
using System.Collections.Generic;
using Swing.Game;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Character
{
    public class GoreSlowController : IInitializable, IDisposable
    {
        [Inject] private SignalBus signalBus;

        private CompositeDisposable disposables = new CompositeDisposable();

        public void Initialize()
        {
            var jointBreakStream = signalBus.GetStream<JointBrokenSignal>();

            jointBreakStream.Buffer(jointBreakStream.Throttle(TimeSpan.FromSeconds(.2)))
                .Where(xs => xs.Count >= 3)
                .Subscribe(xs => signalBus.Fire(new TimeSlowSignal { slowTime = 2f }))
                .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}