using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;

namespace Characters
{
    public class GrapplingFiredSignal
    {
        public Vector3 position;
    }

    public class GrapplingReleasedSignal
    {

    }

    public class GrapplingHook : MonoBehaviour
    {
        [SerializeField]
        private LayerMask mask;
        [SerializeField]
        private GameObject launcher;

        [Inject] private SignalBus signalBus;

        private GameObject currentRope;

        public void Start()
        {
            signalBus.Subscribe<GrapplingFiredSignal>(HandleGrapplingHookFired);
            signalBus.Subscribe<GrapplingReleasedSignal>(HandleGrapplingReleasedSignal);
        }

        public void OnDestroy()
        {
            signalBus.Unsubscribe<GrapplingFiredSignal>(HandleGrapplingHookFired);
            signalBus.Unsubscribe<GrapplingReleasedSignal>(HandleGrapplingReleasedSignal);
        }

        private void HandleGrapplingHookFired(GrapplingFiredSignal signal)
        {
            Debug.Log("Fire");
            currentRope = ConnectRope(launcher.GetComponent<Rigidbody2D>(), signal.position - launcher.transform.position, mask.value);
        }

        private void HandleGrapplingReleasedSignal(){
            if (currentRope != null) Destroy(currentRope);
        }

        private static GameObject ConnectRope(Rigidbody2D launcher, Vector3 direction, int mask)
        {
            var hit = Physics2D.Raycast(launcher.position, direction, Mathf.Infinity, mask);

            Vector3 hitPosition = hit.point;

            var anchor = new GameObject("GrapplingAnchor");
            anchor.transform.position = hitPosition;
            var anchorRigidbody = anchor.AddComponent<Rigidbody2D>();
            anchorRigidbody.isKinematic = true;

            var joint = anchor.gameObject.AddComponent<SpringJoint2D>();
            joint.anchor = anchor.transform.InverseTransformPoint(hitPosition);
            joint.autoConfigureDistance = false;
            joint.distance = 0;
            joint.dampingRatio = 1;
            joint.frequency = 1f;
            joint.enableCollision = true;

            joint.connectedBody = launcher;

            return anchor;
        }

        public void OnDrawGizmos()
        {
            if(currentRope != null){
                Vector3 position = currentRope.transform.position;
                float iconSize = .1f;
                Gizmos.DrawLine(position + Vector3.up * iconSize, position - Vector3.up * iconSize);
                Gizmos.DrawLine(position + Vector3.back * iconSize, position - Vector3.back * iconSize);
                Gizmos.DrawLine(position + Vector3.right * iconSize, position - Vector3.right * iconSize);

                Gizmos.DrawLine(currentRope.transform.position, transform.position);
            }
        }
    }
}