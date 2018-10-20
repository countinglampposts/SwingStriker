using UnityEngine;
using Zenject;
using UniRx;
using Swing.Character;
using Swing.Game;
using UniRx.Diagnostics;
using System;
using System.Linq;
using InControl;

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
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this) 
                      .Where(_ => state.localPlayerControl.Value)
                      .Select(_ => Input.mousePosition)
                      .Buffer(60)
                      .Select(positions => positions.Skip(1).Select((next, index) => next - positions[index]).Average(delta => delta.magnitude))
                      .Select(avg => avg > 5)
                      .DistinctUntilChanged()
                      .Subscribe(enabled => mouseEnabled = enabled);

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => state.localPlayerControl.Value)
                      .Where(_ => mouseEnabled)
                      .Where(_ => playerCamera.pixelRect.Contains(Input.mousePosition))
                      .Subscribe(_ => state.aimDirection.Value = playerCamera.ScreenToWorldPoint(Input.mousePosition) - bodyRoot.rootBodyPart.transform.position);

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => state.localPlayerControl.Value)
                      .Where(_ => Input.GetKeyDown(KeyCode.Mouse0))
                      .Where(_ => playerCamera.pixelRect.Contains(Input.mousePosition))
                      .Subscribe(_ => signalBus.Fire<GrapplingFiredSignal>());

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => state.localPlayerControl.Value)
                      .Where(_ => Input.GetKeyUp(KeyCode.Mouse0))
                      .Where(_ => playerCamera.pixelRect.Contains(Input.mousePosition))
                      .Subscribe(_ => signalBus.Fire(new GrapplingReleasedSignal()));
        }
    }
}