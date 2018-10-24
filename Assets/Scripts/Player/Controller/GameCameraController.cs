using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using System.Linq;
using Zenject;

namespace Swing.Game
{
    public class GameCameraState{
        public List<Transform> pointsOfInterest = new List<Transform>();
    }

    public class GameCameraController : MonoBehaviour
    {
        [SerializeField] private float minSize = 8;
        [SerializeField] private Camera controlledCamera;
        [SerializeField] private Transform controlledRoot;

        [Inject] GameCameraState cameraState;

        private void Start()
        {
            var goalPosition = Vector3.zero;
            var goalSize = minSize;
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Select(_ => cameraState.pointsOfInterest
                                              .Where(pt => pt != null)
                                              .Select(pt => (Vector2)pt.position))
                      .Select(pts => ProjectUtils.CreateRectFromContainingPoints(pts))
                      .Select(rect => rect.CreateEncapsulatingRect(controlledCamera.aspect))
                      .Subscribe(rect => {
                          Vector3 position = rect.center;
                          position.z = -10;
                          goalPosition = position;
                          goalSize = Mathf.Max(minSize, rect.width/2f);
            });

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Subscribe(_ =>
                      {
                          controlledRoot.position = Vector3.Lerp(controlledRoot.position, goalPosition, 2 * Time.deltaTime);
                          controlledCamera.orthographicSize = Mathf.Lerp(controlledCamera.orthographicSize, goalSize, 3 * Time.deltaTime);
                      });
        }

        private void OnDrawGizmos()
        {
            var pts = cameraState.pointsOfInterest
                                 .Where(pt => pt != null)
                                 .Select(pt => (Vector2)pt.position);
            var rect = ProjectUtils.CreateRectFromContainingPoints(pts);
            var aRect = rect.CreateEncapsulatingRect(controlledCamera.aspect);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(rect.center, rect.size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(aRect.center, aRect.size);
            Gizmos.color = Color.white;
        }
    }
}