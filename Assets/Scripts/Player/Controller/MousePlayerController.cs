using UnityEngine;
using Zenject;
using UniRx;
using Swing.Character;
using Swing.Game;

namespace Swing.Player
{
    public class MousePlayerController : MonoBehaviour
    {
        [Inject] Camera playerCamera;
        [Inject] SignalBus signalBus;
        [Inject] BodyRoot bodyRoot;
        [Inject] private CharacterState state;

        private void Start()
        {
            state.localPlayerControl
                 .TakeUntilDestroy(this)
                 .Subscribe(localControl => enabled = localControl);

            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(KeyCode.Mouse0))
                      .Where(_ => playerCamera.pixelRect.Contains(Input.mousePosition))
                      .Where(_ => enabled)
                      .TakeUntilDestroy(this)
                      .Subscribe(_ => signalBus.Fire(new GrapplingFiredSignal() { direction = playerCamera.ScreenToWorldPoint(Input.mousePosition) - bodyRoot.rootBodyPart.transform.position}));

            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyUp(KeyCode.Mouse0))
                      .Where(_ => playerCamera.pixelRect.Contains(Input.mousePosition))
                      .Where(_ => enabled)
                      .TakeUntilDestroy(this)
                      .Subscribe(_ => signalBus.Fire(new GrapplingReleasedSignal()));
        }

        public void Disable()
        {
            enabled = false;
        }
    }
}