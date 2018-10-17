using InControl;
using UnityEngine;
using Zenject;
using Swing.Character;

namespace Swing.Player
{
    public class GamepadPlayerController : MonoBehaviour
    {
        [Inject] SignalBus signalBus;
        [Inject] InputDevice inputDevice;
        [Inject] CharacterState state;

        private void Update()
        {
            if (state.localPlayerControl.Value == true)
            {
                if (inputDevice.RightTrigger.WasPressed)
                {
                    var direction = new Vector2(inputDevice.RightStickX, inputDevice.RightStickY).normalized;
                    signalBus.Fire(new GrapplingFiredSignal { direction = direction });
                }

                if (inputDevice.RightTrigger.WasReleased)
                {
                    signalBus.Fire<GrapplingReleasedSignal>();
                }
            }
        }
    }
}