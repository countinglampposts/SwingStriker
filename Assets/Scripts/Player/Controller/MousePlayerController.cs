using UnityEngine;
using Zenject;
using UniRx;
using Swing.Character;
using Swing.Game;
using UniRx.Diagnostics;
using System;
using System.Linq;
using InControl;
using System.Collections.Generic;

namespace Swing.Player
{
    public class MousePlayerController : MonoBehaviour
    {
        [Inject] private Camera playerCamera;
        [Inject] private SignalBus signalBus;
        [Inject] private BodyRoot bodyRoot;
        [Inject] private CharacterState state;

        private void Start()
        {
            bool mouseEnabled = true;
            List<Vector3> buffer = new List<Vector3>();
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => state.localPlayerControl.Value)
                      .Do(_ => {
                          buffer.Add(Input.mousePosition);
                          if (buffer.Count >= 30) buffer.RemoveAt(0);
                      })
                      .Select(_ => buffer)
                      .Skip(1)
                      .Select(positions => positions.Skip(1).Select((next, index) => next - positions[index]).Average(delta => delta.magnitude))
                      .Select(avg => avg > 1)
                      .DistinctUntilChanged()
                      .Subscribe(enabled => mouseEnabled = enabled);

            Action resetMouseDirection = () => state.aimDirection.Value = playerCamera.ScreenToWorldPoint(Input.mousePosition) - bodyRoot.rootBodyPart.transform.position;

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => state.localPlayerControl.Value)
                      .Where(_ => mouseEnabled)
                      .Where(_ => playerCamera.pixelRect.Contains(Input.mousePosition))
                      .Subscribe(_ => resetMouseDirection());

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => state.localPlayerControl.Value)
                      .Where(_ => Input.GetKeyDown(KeyCode.Mouse0))
                      .Where(_ => playerCamera.pixelRect.Contains(Input.mousePosition))
                      .Subscribe(_ => {
                          resetMouseDirection();
                          signalBus.Fire<GrapplingFiredSignal>(); 
                      });

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => state.localPlayerControl.Value)
                      .Where(_ => Input.GetKeyUp(KeyCode.Mouse0))
                      .Where(_ => playerCamera.pixelRect.Contains(Input.mousePosition))
                      .Subscribe(_ => {
                          resetMouseDirection();
                          signalBus.Fire<GrapplingReleasedSignal>();
                      });
        }
    }
}