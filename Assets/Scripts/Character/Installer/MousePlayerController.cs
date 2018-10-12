using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Diagnostics;
using System;

namespace Characters
{
    public class MousePlayerController : MonoBehaviour
    {
        [Inject] SignalBus signalBus;

        public void Start()
        {
            Debug.Log("Hello?");
            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(KeyCode.Mouse0))
                      .TakeUntilDestroy(this)
                      .Debug("fire!!!!")
                      .Subscribe(_ => signalBus.Fire(new GrapplingFiredSignal() { position = GetMousePosition() }));

            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyUp(KeyCode.Mouse0))
                      .TakeUntilDestroy(this)
                      .Subscribe(_ => signalBus.Fire(new GrapplingReleasedSignal()));
        }

        private static Vector3 GetMousePosition()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float point;
            if ((new Plane(Vector3.zero, Vector3.up, Vector3.right)).Raycast(ray, out point))
            {
                return ray.GetPoint(point);
            }
            Debug.LogError("Mouse not colliding with plane");
            return Vector3.zero;
        }
    }
}