using System;
using System.Linq;
using InControl;
using Swing.Game;
using Swing.Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Character
{
    public class SuicideSignal{}
    public class SuicideController : IInitializable, IDisposable
    {
        [Inject] SignalBus signalBus;
        [Inject] PlayerData playerData;
        [Inject] CharacterState state;

        CompositeDisposable disposables = new CompositeDisposable();

        public void Initialize()
        {
            Observable.Timer(TimeSpan.FromSeconds(15))
                      .Subscribe(_ =>
                      {
                          Func<bool> buttonsPressed = () => {
                              var device = InputManager.Devices.FirstOrDefault(selectedDevice => selectedDevice.GUID == playerData.deviceID);
                              bool deviceButtonPressed = false;
                              if (device != null) deviceButtonPressed = device.Action1.WasPressed;
                              return deviceButtonPressed || Input.GetKeyDown(KeyCode.S);
                          };
                          
                          Observable.EveryUpdate()
                                    .Select(__ => buttonsPressed())
                                    .DistinctUntilChanged()
                                    .Where(pressed => pressed)
                                    .Where(__ => state.localPlayerControl.Value)
                                    .Subscribe(__ =>
                                    {
                                        signalBus.Fire<SuicideSignal>();
                                        signalBus.Fire<PlayerKilledSignal>();
                                    })
                                    .AddTo(disposables);
                      })
                      .AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}