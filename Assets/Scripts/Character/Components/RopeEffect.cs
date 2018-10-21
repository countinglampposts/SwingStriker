using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

namespace Swing.Character
{
    [RequireComponent(typeof(Joint2D))]
    public class RopeEffect : MonoBehaviour
    {
        [Inject] private CharacterSettings settings;
        [Inject] private SignalBus signalBus;

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
                          var lerp = 70f * (Time.time - launchTime) / distance;
                          otherPoint = Vector2.Lerp(point, otherPoint, lerp);
                          if (lerp >= 1 && !collisionEffectTriggered)
                          {
                              signalBus.Fire(new RumbleTriggeredSignal { magnitude = 1.5f });
                              collisionEffectTriggered = true;
                          }

                          lineRenderer.SetPositions(new Vector3[]{point,otherPoint});
             });
        }
    }
}