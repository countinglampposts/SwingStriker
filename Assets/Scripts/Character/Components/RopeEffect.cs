using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;
using UnityEngine.Audio;
using Swing.Sound;
using System.Linq;

namespace Swing.Character
{
    [RequireComponent(typeof(Joint2D))]
    public class RopeEffect : MonoBehaviour
    {
        [Inject] private CharacterSettings settings;
        [Inject] private SignalBus signalBus;
        [Inject] private AudioMixerGroup audioMixerGroup;
        [Inject] private SoundAssets soundAssets;

        private void Start()
        {
            var lineRenderer = Instantiate(settings.ropeEffect.gameObject,transform).GetComponent<LineRenderer>();
            var joint = GetComponent<AnchoredJoint2D>();
            var launchTime = Time.time;
            var collisionEffectTriggered = false;

            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => enabled)
                      .Where(_ => joint.connectedBody != null)
                      .Subscribe(_ => {
                          var point = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor);
                          var otherPoint = joint.transform.TransformPoint(joint.anchor);
                          var distance = Vector2.Distance(point, otherPoint);
                          var lerp = settings.grapplingHookSpeed * (Time.time - launchTime) / distance;
                          otherPoint = Vector2.Lerp(point, otherPoint, lerp);
                          if (lerp >= 1 && !collisionEffectTriggered)
                          {
                              AudioUtils.PlayAudioOnObject(joint.gameObject, soundAssets.sounds.FirstOrDefault(sound => sound.id == "Grappled").clip, audioMixerGroup);
                              signalBus.Fire(new RumbleTriggeredSignal { magnitude = 1.5f });
                              collisionEffectTriggered = true;
                          }

                          lineRenderer.SetPositions(new Vector3[]{point,otherPoint});
             });
        }
    }
}