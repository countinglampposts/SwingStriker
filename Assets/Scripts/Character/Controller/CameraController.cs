using UnityEngine;
using Zenject;
using System;
using UniRx;

namespace Characters
{
    public class CameraController : MonoBehaviour
    {
        [Inject] BodyRoot root;
        [Inject] Camera camera;

        private void Start()
        {
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Subscribe(_ =>
                      {
                          Vector3 cameraPosition = camera.transform.position;
                          Vector3 rootPosition = root.rootBodyPart.transform.position;

                          cameraPosition.x = rootPosition.x;
                          cameraPosition.y = rootPosition.y;

                          camera.transform.position = cameraPosition;
                      });
        }
    }
}