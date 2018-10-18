using InControl;
using UnityEngine;
using Zenject;
using Swing.Character;
using System.Linq;

namespace Swing.Player
{
    public class GamepadPlayerController : MonoBehaviour
    {
        [Inject] SignalBus signalBus;
        [Inject] PlayerData playerData;
        [Inject] CharacterState state;

        private void Update()
        {
            var inputDevice = InputManager.Devices.FirstOrDefault(device => device.GUID == playerData.deviceID);
            if (state.localPlayerControl.Value == true && inputDevice != null)
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