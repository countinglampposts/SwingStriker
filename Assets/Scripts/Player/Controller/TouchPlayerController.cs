using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Diagnostics;
using System;
using Swing.Character;
using System.Linq;

namespace Swing.Player
{
    public class TouchPlayerController : MonoBehaviour
    {
        [Inject] Camera playerCamera;
        [Inject] SignalBus signalBus;
        [Inject] CameraSettings cameraSettings;
        [Inject] BodyRoot bodyRoot;

        public void Start()
        {
            Observable.EveryUpdate()
                      .Subscribe(_ =>
                      {
                        var touches = Input.touches.Where(touch => touch.phase == TouchPhase.Began && playerCamera.pixelRect.Contains(touch.position));
                        
                        foreach(var touch in touches){
                              signalBus.Fire(new GrapplingFiredSignal() { direction = playerCamera.ScreenToWorldPoint(touch.position) - bodyRoot.rootBodyPart.transform.position });
                              Observable.EveryUpdate()
                                        .Where(__ => Input.GetTouch(touch.fingerId).phase == TouchPhase.Ended)
                                        .First()
                                        .Subscribe(__ => signalBus.Fire<GrapplingReleasedSignal>());
                        }
                      });
        }
    }
}