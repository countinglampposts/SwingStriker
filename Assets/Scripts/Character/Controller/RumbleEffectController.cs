using System.Collections;
using System.Collections.Generic;
using Swing.Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Character
{
    public class RumbleTriggeredSignal{
        public float magnitude;
    }
    public class RumbleEffectController : MonoBehaviour
    {
        [SerializeField] private Transform rumbledTransform;

        [Inject] private SignalBus signalBus;

        private void Start()
        {
            signalBus.GetStream<PlayerKilledSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => signalBus.Fire(new RumbleTriggeredSignal { magnitude = 3f }));

            float currentRumble = 0;
            float maxRumbleMagnitude = .05f;
            float rumbleFrquency = 60;// in rotations per second
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Subscribe(__ => currentRumble = Mathf.Lerp(currentRumble, 0, 5 * Time.deltaTime));
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Subscribe(__ =>
                      {
                          rumbledTransform.localPosition = new Vector2(Mathf.Cos(Time.time * rumbleFrquency), Mathf.Sin(Time.time * rumbleFrquency) * 3) * currentRumble;
                      });

            signalBus.GetStream<RumbleTriggeredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(signal => currentRumble = Mathf.Min(maxRumbleMagnitude, signal.magnitude + maxRumbleMagnitude));
        }
    }
}