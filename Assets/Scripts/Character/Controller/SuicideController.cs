using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using Swing.Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Character
{
    public class SuicideSignal{}
    public class SuicideController : MonoBehaviour
    {
        [Inject] SignalBus signalBus;
        [Inject] PlayerData playerData;
        [Inject] CharacterState state;

        private void Start()
        {

            Observable.Timer(TimeSpan.FromSeconds(15))
                      .TakeUntilDestroy(this)
                      .Subscribe(_ =>
                      {
                          Func<bool> buttonsPressed = () => {
                              var device = InputManager.Devices.FirstOrDefault(selectedDevice => selectedDevice.GUID == playerData.deviceID);
                              bool deviceButtonPressed = false;
                              if (device != null) deviceButtonPressed = device.Action1.WasPressed;
                              return deviceButtonPressed || Input.GetKeyDown(KeyCode.S);
                          };
                          
                          Observable.EveryUpdate()
                                    .TakeUntilDestroy(this)
                                    .Select(__ => buttonsPressed())
                                    .DistinctUntilChanged()
                                    .Where(pressed => pressed)
                                    .Where(__ => state.localPlayerControl.Value)
                                    .Subscribe(__ =>
                                    {
                                        signalBus.Fire<SuicideSignal>();
                                        signalBus.Fire<PlayerKilledSignal>();
                                    });
                      });
        }
    }
}