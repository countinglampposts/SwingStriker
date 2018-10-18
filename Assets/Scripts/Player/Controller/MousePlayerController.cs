using UnityEngine;
using Zenject;
using UniRx;
using Swing.Character;
using Swing.Game;

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
            state.localPlayerControl
                 .TakeUntilDestroy(this)
                 .Subscribe(localControl => enabled = localControl);

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => Input.GetKeyDown(KeyCode.Mouse0))
                      .Where(_ => playerCamera.pixelRect.Contains(Input.mousePosition))
                      .Where(_ => enabled)
                      .Subscribe(_ => signalBus.Fire(new GrapplingFiredSignal() { direction = playerCamera.ScreenToWorldPoint(Input.mousePosition) - bodyRoot.rootBodyPart.transform.position}));

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => Input.GetKeyUp(KeyCode.Mouse0))
                      .Where(_ => playerCamera.pixelRect.Contains(Input.mousePosition))
                      .Where(_ => enabled)
                      .Subscribe(_ => signalBus.Fire(new GrapplingReleasedSignal()));
        }

        public void Disable()
        {
            enabled = false;
        }
    }
}