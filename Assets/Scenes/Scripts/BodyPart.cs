using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{

    public class BodyPart : MonoBehaviour
    {
        [SerializeField]
        public Vector3 pivot;
        [SerializeField]
        public BodyPart[] children;

        public void OnDrawGizmos()
        {
            Vector3 pivotPosition = transform.TransformPoint(pivot);
            float iconSize = .1f;
            Gizmos.DrawLine(pivotPosition + Vector3.up * iconSize, pivotPosition - Vector3.up * iconSize);
            Gizmos.DrawLine(pivotPosition + Vector3.back * iconSize, pivotPosition - Vector3.back * iconSize);
            Gizmos.DrawLine(pivotPosition + Vector3.right * iconSize, pivotPosition - Vector3.right * iconSize);
            
            Gizmos.color = Color.green;
            if (children != null)
            {
                foreach (BodyPart child in children)
                {
                    if (child != null) Gizmos.DrawLine(transform.position, child.transform.position);
                }
            }
            Gizmos.color = Color.white;
        }
    }
}