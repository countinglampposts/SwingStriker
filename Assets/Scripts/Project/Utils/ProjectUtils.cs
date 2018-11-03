using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using System;
using System.Linq;
using Swing.Player;
using Swing.Character;
using Swing.Level;
using Swing.Game;

namespace Swing
{
    public static class ProjectUtils 
    {
        public static void ReturnToMainMenu(){
            Application.LoadLevel(Application.loadedLevel);
        }

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

        public static void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, int layerMask)
        {
            var hits = Physics2D.CircleCastAll(explosionPosition, explosionRadius, Vector2.zero,0,layerMask);
            foreach(var h in hits){
                if(h.rigidbody != null){
                    h.rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
                }
            }
        }

        public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
        {
            var dir = (body.transform.position - explosionPosition);
            float wearoff = 1 - (dir.magnitude / explosionRadius);
            body.AddForce(dir.normalized * explosionForce * wearoff);
        }

        public static void AutoDestruct(this ParticleSystem ps)
        {
            Observable.EveryUpdate()
                      .TakeUntilDestroy(ps)
                      .Skip(5)
                      .Where(_ => ps.particleCount == 0)
                      .Subscribe(_ => GameObject.Destroy(ps.gameObject));
        }
    }
}