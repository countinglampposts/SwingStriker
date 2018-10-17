using InControl;
using UnityEngine;
using Zenject;
using Swing.Character;

namespace Swing.Player
{
    public class ControllerPlayerController : MonoBehaviour
    {
        [Inject] SignalBus signalBus;
        [Inject] InputDevice inputDevice;

        private void Update()
        {
            if(inputDevice.RightTrigger.WasPressed){
                var direction = new Vector2(inputDevice.RightStickX, inputDevice.RightStickY).normalized;
                signalBus.Fire(new GrapplingFiredSignal { direction = direction });
            }

            if(inputDevice.RightTrigger.WasReleased)
            {
                signalBus.Fire<GrapplingReleasedSignal>();
            }
        }
    }
}