﻿using System;
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
        public static IDisposable BindToAllDevices(Button button, int actionIndex)
        {
            return BindToDevice(button, actionIndex, Guid.Empty);
        }

        public static IDisposable BindToAllDevices(Button button, int actionIndex, Func<Guid, bool> predicate)
        {
            return BindToDevice(button, actionIndex, Guid.Empty, predicate);
        }

        public static IDisposable BindToDevice(Button button, int actionIndex, Guid deviceID, Func<Guid,bool> predicate = null)
        {
            return Observable.EveryUpdate()
                      .TakeUntilDestroy(button)
                      .Where(_ => button.gameObject.activeSelf && button.gameObject.activeInHierarchy)
                      .Select(_ => (deviceID == Guid.Empty)? InputManager.ActiveDevice : InputManager.Devices.FirstOrDefault(device => device.GUID == deviceID))
                      .Where(device => device != null)
                      .Where(device => (predicate == null) || predicate(device.GUID))
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
                      .DelayFrame(1)
                      .Subscribe(_=>button.onClick.Invoke());
        }
    }
}