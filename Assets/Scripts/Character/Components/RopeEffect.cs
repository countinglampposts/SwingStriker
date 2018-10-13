using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

namespace Character
{
    [RequireComponent(typeof(Joint2D))]
    public class RopeEffect : MonoBehaviour
    {
        [Inject] private CharacterSettings settings;

        private void Start()
        {
            var lineRenderer = Instantiate(settings.ropeEffect.gameObject,transform).GetComponent<LineRenderer>();

            var joint = GetComponent<SpringJoint2D>();
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Subscribe(_ => {
                            lineRenderer.SetPositions(new Vector3[]{
                            joint.transform.TransformPoint(joint.anchor),
                            joint.connectedBody.transform.TransformPoint(joint.connectedAnchor)
                        });
             });
        }
    }
}