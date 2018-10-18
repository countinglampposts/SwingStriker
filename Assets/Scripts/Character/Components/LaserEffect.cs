using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

namespace Swing.Character
{
    public class LaserEffect : MonoBehaviour
    {
        [Inject] private CharacterSettings settings;
        [Inject] private CharacterState state;

        private void Start()
        {
            var lineRenderer = Instantiate(settings.laserEffect.gameObject,transform).GetComponent<LineRenderer>();

            var raycastStream = Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => enabled)
                      .Select(_ => Physics2D.Raycast(transform.position, state.aimDirection.Value, settings.grapplingDistance, settings.grapplingMask));

            raycastStream.Select(hit => hit.transform != null)
                         .CombineLatest(state.localPlayerControl, (hitting, localControl) => hitting && localControl)
                         .Subscribe(show => lineRenderer.enabled = show);

            raycastStream.Where(hit => hit.transform != null)
                      .Subscribe(hit => {
                          lineRenderer.SetPositions(new Vector3[]{
                            transform.position,
                            hit.point
                        });
             });
        }
    }
}