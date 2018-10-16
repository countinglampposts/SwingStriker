using UnityEngine;
using Zenject;
using UniRx;
using Swing.Character;
using System.Linq;
using Swing.Game;

namespace Swing.Player
{
    public class TouchPlayerController : MonoBehaviour, ICharacterDriver
    {
        [Inject] Camera playerCamera;
        [Inject] SignalBus signalBus;
        [Inject] BodyRoot bodyRoot;

        private bool locked = false;

        public void Update()
        {
            var touches = Input.touches.Where(touch => touch.phase == TouchPhase.Began && playerCamera.pixelRect.Contains(touch.position));

            foreach(var touch in touches){
                  signalBus.Fire(new GrapplingFiredSignal() { direction = playerCamera.ScreenToWorldPoint(touch.position) - bodyRoot.rootBodyPart.transform.position });
                  locked = true;
                  Observable.EveryUpdate()
                            .Where(__ =>Input.touches.First(selectedTouch => selectedTouch.fingerId == touch.fingerId).phase == TouchPhase.Ended)
                            .First()
                            .Subscribe(__ => {
                                locked = false;
                                signalBus.Fire<GrapplingReleasedSignal>(); 
                            });
            }
        }

        public void Disable()
        {
            enabled = false;
        }
    }
}