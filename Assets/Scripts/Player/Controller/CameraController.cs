using UnityEngine;
using Zenject;
using UniRx;
using Swing.Character;
using Swing.Game;

namespace Swing.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Camera movedCamera;

        [Inject] BodyRoot root;
        [Inject] CameraSettings cameraSettings;
        [Inject] CharacterState state;

        private void Start()
        {
            movedCamera.rect = cameraSettings.viewportRect;
            movedCamera.cullingMask = cameraSettings.mask.value;

            state.isCorpse
                 .Where(isCorpse => isCorpse)
                 .First()
                 .Subscribe(_ => movedCamera.enabled = false);

            Observable.EveryUpdate()
                  .TakeUntilDestroy(this)
                  .Where(_ => state.localPlayerControl.Value)
                  .Subscribe(_ =>
                  {
                      Vector3 cameraPosition = transform.position;
                      Vector3 rootPosition = root.rootBodyPart.transform.position;

                      cameraPosition.x = rootPosition.x;
                      cameraPosition.y = rootPosition.y;
                      cameraPosition.z = -10;

                      transform.position = cameraPosition;

                      var rootRigidbody = root.rootBodyPart.GetComponent<Rigidbody2D>();
                      movedCamera.orthographicSize = Mathf.Lerp(movedCamera.orthographicSize, 5 + rootRigidbody.velocity.magnitude/3f , 3f * Time.deltaTime);
                  });
        }
    }
}