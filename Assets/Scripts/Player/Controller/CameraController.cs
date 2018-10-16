using UnityEngine;
using Zenject;
using UniRx;
using Swing.Character;
using Swing.Game;

namespace Swing.Player
{
    public class CameraController : MonoBehaviour//, ICharacterDriver
    {
        [Inject] BodyRoot root;
        [Inject] Camera movedCamera;
        [InjectOptional] CameraSettings cameraSettings;

        private void Start()
        {
            if(cameraSettings.viewportRect.size != Vector2.zero) movedCamera.rect = cameraSettings.viewportRect;
            Observable.EveryUpdate()
                      .Where(_ => enabled)
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

        public void Disable()
        {
            movedCamera.enabled = false;
            enabled = false;
        }
    }
}