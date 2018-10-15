using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

namespace Swing
{
    public static class ProjectUtils 
    {
        public static IObservable<T> GetStream<T>(this SignalBus signalBus){
            return Observable.FromEvent<T>(action => signalBus.Subscribe<T>(action), action => signalBus.Unsubscribe<T>(action));
        }

    }
}