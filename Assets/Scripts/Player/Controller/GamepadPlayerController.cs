using InControl;
using UnityEngine;
using Zenject;
using Swing.Character;
using System.Linq;
using UniRx;

namespace Swing.Player
{
    public class GamepadPlayerController : IInitializable
    {
        [Inject] SignalBus signalBus;
        [Inject] PlayerData playerData;
        [Inject] CharacterState state;

        private CompositeDisposable disposables = new CompositeDisposable();

        public void Initialize()
        {
            bool grapplingHookExtended = false;
            Observable.EveryUpdate()
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
                      })
                      .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}