using UnityEngine;
using Zenject;
using UniRx;
using Swing.Character;
using System.Linq;
using Swing.Game;

namespace Swing.Player
{
    public class TouchPlayerController : MonoBehaviour
    {
        [Inject] private Camera playerCamera;
        [Inject] private SignalBus signalBus;
        [Inject] private BodyRoot bodyRoot;
        [Inject] private CharacterState state;

        private bool locked = false;

        private void Start()
        {
            state.localPlayerControl
                 .TakeUntilDestroy(this)
                 .Subscribe(localControl => enabled = localControl);
        }

        public void Update()
        {
            if (!locked)
            {
                var touches = Input.touches.Where(touch => touch.phase == TouchPhase.Began && playerCamera.pixelRect.Contains(touch.position));

                foreach (var touch in touches)
                {
                    state.aimDirection.Value = playerCamera.ScreenToWorldPoint(touch.position) - bodyRoot.rootBodyPart.transform.position;
                    signalBus.Fire<GrapplingFiredSignal>();
                    locked = true;
                    Observable.EveryUpdate()
                              .Where(__ => Input.touches.First(selectedTouch => selectedTouch.fingerId == touch.fingerId).phase == TouchPhase.Ended)
                              .First()
                              .Subscribe(__ =>
                              {
                                  locked = false;
                                  signalBus.Fire<GrapplingReleasedSignal>();
                              });
                }
            }
        }

        public void Disable()
        {
            enabled = false;
        }
    }
}