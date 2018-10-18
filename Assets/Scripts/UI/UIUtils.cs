using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UniRx;
using UniRx.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Swing.UI
{
    public static class UIUtils
    {
        public static void AddGamepadButtonPressToButton(Button button, int actionIndex)
        {
            AddGamepadButtonPressToButton(button, actionIndex, Guid.Empty);
        }

        public static void AddGamepadButtonPressToButton(Button button, int actionIndex, Guid deviceID)
        {
            Observable.EveryUpdate()
                      .TakeUntilDestroy(button)
                      .Where(_ => button.gameObject.activeSelf && button.gameObject.activeInHierarchy)
                      .Select(_ => (deviceID == Guid.Empty)? InputManager.ActiveDevice : InputManager.Devices.FirstOrDefault(device => device.GUID == deviceID))
                      .Where(device => device != null)
                      .Where(device => { switch (actionIndex)
                          {
                              case 0:
                                  return device.Action1.WasPressed;
                              case 1:
                                  return device.Action2.WasPressed;
                              case 2:
                                  return device.Action3.WasPressed;
                              case 3:
                                  return device.Action4.WasPressed;
                          }
                          return false;
                      })
                      .Delay(TimeSpan.FromSeconds(.1f))
                      .Subscribe(_=>button.onClick.Invoke());
        }
    }
}