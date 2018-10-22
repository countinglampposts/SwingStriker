using System.Collections;
using System.Collections.Generic;
using UniRx;
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
                      .Select(_ => ProjectUtils.CreateRectFromContainingPoints(cameraState.pointsOfInterest.Select(pt => (Vector2)pt.position).ToArray()))
                      .Subscribe(rect => {
                          Vector3 position = rect.center;
                          position.z = -10;
                          goalPosition = position;
                          goalSize = Mathf.Max(minSize, rect.width / 2);
            });

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Subscribe(_ =>
                      {
                          controlledRoot.position = Vector3.Lerp(controlledRoot.position, goalPosition, 5 * Time.deltaTime);
                          controlledCamera.orthographicSize = Mathf.Lerp(controlledCamera.orthographicSize, goalSize, 5 * Time.deltaTime);
                      });
        }
    }
}