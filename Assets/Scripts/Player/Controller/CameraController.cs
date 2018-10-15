using UnityEngine;
using Zenject;
using System;
using UniRx;
using Swing.Character;

namespace Swing.Player
{

    public class CameraController : MonoBehaviour
    {
        [Inject] BodyRoot root;
        [Inject] Camera movedCamera;
        [Inject] CameraSettings cameraSettings;

        private void Start()
        {
            movedCamera.rect = cameraSettings.viewportRect;
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