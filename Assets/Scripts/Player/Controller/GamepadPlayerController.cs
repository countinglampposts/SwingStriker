using InControl;
using UnityEngine;
using Zenject;
using Swing.Character;
using System.Linq;
using UniRx;

namespace Swing.Player
{
    public class GamepadPlayerController : MonoBehaviour
    {
        [Inject] SignalBus signalBus;
        [Inject] PlayerData playerData;
        [Inject] CharacterState state;

        private void Start()
        {
            bool grapplingHookExtended = false;
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => state.localPlayerControl.Value)
                      .Select(_ => InputManager.Devices.FirstOrDefault(device => device.GUID == playerData.deviceID))
                      .Where(device => device != null)
                      .Subscribe(device =>
                      {
                          var direction = new Vector2(device.RightStickX + device.LeftStickX, device.RightStickY + device.LeftStickY).normalized;
                          if (direction.magnitude > .2f)
                          {
                              state.aimDirection.Value = direction;
                              if (!grapplingHookExtended && (device.RightTrigger.WasPressed || device.LeftTrigger.WasPressed))
                              {
                                  grapplingHookExtended = true;
                                  signalBus.Fire<GrapplingFiredSignal>();
                              }

                              if (grapplingHookExtended && (device.RightTrigger.WasReleased || device.LeftTrigger.WasReleased))
                              {
                                  grapplingHookExtended = false;
                                  signalBus.Fire<GrapplingReleasedSignal>();
                              }
                          }
                      });
        }
    }
}