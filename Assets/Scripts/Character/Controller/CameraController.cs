using UnityEngine;
using Zenject;
using System;
using UniRx;

namespace Character
{
    public class CameraController : MonoBehaviour
    {
        [Inject] BodyRoot root;
        [Inject] Camera movedCamera;

        private void Start()
        {
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Subscribe(_ =>
                      {
                          Vector3 cameraPosition = movedCamera.transform.position;
                          Vector3 rootPosition = root.rootBodyPart.transform.position;

                          cameraPosition.x = rootPosition.x;
                          cameraPosition.y = rootPosition.y;

                          movedCamera.transform.position = cameraPosition;
                      });
        }
    }
}