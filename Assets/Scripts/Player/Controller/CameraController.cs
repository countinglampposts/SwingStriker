﻿using UnityEngine;
using Zenject;
using UniRx;
using Swing.Character;
using Swing.Game;

namespace Swing.Player
{
    public class CameraController : MonoBehaviour
    {
        [Inject] BodyRoot root;
        [Inject] Camera movedCamera;
        [Inject] CameraSettings cameraSettings;
        [Inject] CharacterState state;

        private void Start()
        {
            if(cameraSettings.viewportRect.size != Vector2.zero) movedCamera.rect = cameraSettings.viewportRect;

            state.isCorpse
                 .Where(isCorpse => isCorpse)
                 .First()
                 .Subscribe(_ => movedCamera.enabled = false);

            Observable.EveryUpdate()
                  .Where(_ => state.localPlayerControl.Value)
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