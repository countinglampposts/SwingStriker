using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class GrapplingHook : MonoBehaviour
    {
        [SerializeField]
        private LayerMask mask;

        private GameObject currentRope;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Mouse0)){
                currentRope = ConnectRope(GetComponent<Rigidbody2D>(), GetMousePosition() - transform.position, mask.value);
            }
            if(Input.GetKeyUp(KeyCode.Mouse0)){
                Destroy(currentRope);
            }
        }

        private static Vector3 GetMousePosition(){
            Debug.Log(Camera.main);
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float point;
            if((new Plane(Vector3.zero, Vector3.up, Vector3.right)).Raycast(ray,out point)){
                return ray.GetPoint(point);
            }
            Debug.LogError("Mouse not colliding with plane");
            return Vector3.zero;
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

            return null;
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