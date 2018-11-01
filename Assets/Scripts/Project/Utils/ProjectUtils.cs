using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System;
using System.Linq;

namespace Swing
{
    public static class ProjectUtils 
    {
        public static IObservable<T> GetStream<T>(this SignalBus signalBus){
            return Observable.FromEvent<T>(
                action => signalBus.Subscribe<T>(action),
                action => signalBus.Unsubscribe<T>(action));
        }

        public static bool HasMax<T>(this IEnumerable<T> sequence, Func<T, int> getComparable)
        {
            bool foundMaxValue = false;
            bool tied = false;
            T maxValue = default(T); // Immediately overwritten anyway

            foreach (T value in sequence)
            {
                if (getComparable(value) == getComparable(maxValue)) tied = true;
                if (getComparable(value) > getComparable(maxValue) || !foundMaxValue)
                {
                    maxValue = value;
                    foundMaxValue = true;
                    tied = false;
                }
            }
            return !tied;
        }

        public static T MaxValueOrDefault<T>(this IEnumerable<T> sequence, Func<T, int> getComparable)
        {
            bool foundMaxValue = false;
            T maxValue = default(T); // Immediately overwritten anyway

            foreach (T value in sequence)
            {
                if (getComparable(value) > getComparable(maxValue) || !foundMaxValue)
                {
                    maxValue = value;
                    foundMaxValue = true;
                }
            }
            return maxValue;
        }

        public static Rect CreateRectFromContainingPoints(IEnumerable<Vector2> points, float buffer = 0){
            var xMin = points.Min(p => p.x);
            var xMax = points.Max(p => p.x);
            var yMin = points.Min(p => p.y);
            var yMax = points.Max(p => p.y);
            return Rect.MinMaxRect(xMin - buffer, yMin - buffer, xMax + buffer, yMax + buffer);
        }

        public static Rect CreateEncapsulatingRect(this Rect rect, float aspectRatio){
            var returnedRect = new Rect();

            if(rect.width/rect.height < aspectRatio){
                // use height
                returnedRect.height = rect.height;
                returnedRect.width = rect.height * aspectRatio;
            }else{
                // use width
                returnedRect.height = rect.width / aspectRatio;
                returnedRect.width = rect.width;
            }

            returnedRect.center = rect.center;

            return returnedRect;
        }

        public static CompositeDisposable CreateComposite(this IDisposable disposable, IDisposable other){
            CompositeDisposable disposables = new CompositeDisposable();
            disposable.AddTo(disposables);
            other.AddTo(disposables);
            return disposables;
        }
    }
}