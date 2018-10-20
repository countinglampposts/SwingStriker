using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using Zenject;
using UniRx.Diagnostics;
using System.Linq;

namespace Swing.Character
{
    [RequireComponent(typeof(Collider2D))]
    public class RumbleCollider : MonoBehaviour
    {
        [SerializeField] float magnitudeMultiplier;
        [SerializeField] float minMagnitude;
        [SerializeField] LayerMask mask;
        [Inject] SignalBus signalBus;
        void Start()
        {
            // TODO
            /*gameObject.OnCollisionEnter2DAsObservable()
                      .TakeUntilDestroy(this)
                      .Do(c=>Debug.Log(LayerMask.LayerToName(c.otherCollider.gameObject.layer)))
                      .Select(c => c.contacts
                                  .Where(contact => mask.value == (mask.value | (1 << contact.otherCollider.gameObject.layer)))
                                  .Sum(contact => contact.relativeVelocity.magnitude))
                      .Debug()
                      .Where(m => m > minMagnitude)
                      .Subscribe(m => signalBus.Fire(new RumbleTriggeredSignal { magnitude = magnitudeMultiplier * m }));*/
        }
    }
}