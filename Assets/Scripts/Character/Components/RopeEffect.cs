using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using UnityEngine.Audio;
using Swing.Sound;
using System.Linq;
using UniRx.Diagnostics;

namespace Swing.Character
{
    [RequireComponent(typeof(Joint2D))]
    public class RopeEffect : MonoBehaviour
    {
        [Inject] private CharacterSettings settings;
        [Inject] private SignalBus signalBus;
        [Inject] private SoundPlayer soundPlayer;

        public Object Init(Transform startTransform, Vector3 endPoint)
        {
            var effectObject = Instantiate(settings.ropeEffect.gameObject, transform);
            var lineRenderer = effectObject.GetComponent<LineRenderer>();
            var launchTime = Time.time;
            var collisionEffectTriggered = false;

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => lineRenderer != null)
                      .Subscribe(_ => {
                          var point = startTransform.position;
                          var distance = Vector2.Distance(point, endPoint);
                          var lerp = settings.grapplingHookSpeed * (Time.time - launchTime) / distance;
                          var drawnEndPoint = Vector2.Lerp(point, endPoint, lerp);
                          if (lerp >= 1 && !collisionEffectTriggered)
                          {
                              soundPlayer.PlayAudioAtPosition("Grappled", endPoint);
                              signalBus.Fire(new RumbleTriggeredSignal { magnitude = 1.5f });
                              collisionEffectTriggered = true;
                          }

                          lineRenderer.SetPositions(new Vector3[]{point, drawnEndPoint });
             });

            return effectObject;
        }
    }
}