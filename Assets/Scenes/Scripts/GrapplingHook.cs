using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class GrapplingHook : MonoBehaviour
    {
        [SerializeField]
        private LayerMask mask;

        private Object currentRope;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Mouse0)){
                if(currentRope != null) Destroy(currentRope);
                currentRope = ConnectRope(GetComponent<Rigidbody>(), GetMousePosition() - transform.position, mask.value);
            }
            if(Input.GetKeyUp(KeyCode.Mouse0)){
                //Destroy(currentRope);
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

        private static Object ConnectRope(Rigidbody launcher, Vector3 direction, int mask)
        {
            RaycastHit hit;
            if(Physics.Raycast(launcher.position, direction, out hit, Mathf.Infinity, mask))
            {
                Debug.Log(hit.collider.gameObject.name);
                Vector3 hitPosition = hit.point;

                var anchor = new GameObject("GrapplingAnchor");
                anchor.transform.position = hitPosition;
                var anchorRigidbody = anchor.AddComponent<Rigidbody>();
                anchorRigidbody.isKinematic = true;

                var joint = anchor.gameObject.AddComponent<HingeJoint>();
                joint.anchor = anchor.transform.InverseTransformPoint(hitPosition);
                joint.axis = Vector3.forward;
                //joint.spring = 100;
                //joint.damper = .0001f;
                joint.enableCollision = true;

                joint.connectedBody = launcher;

                return joint;
            }

            return null;
        }

        /*public void OnDrawGizmos()
        {
            if(currentRope != null){
                Vector3 position = currentRope.transform.position;
                float iconSize = .1f;
                Gizmos.DrawLine(position + Vector3.up * iconSize, position - Vector3.up * iconSize);
                Gizmos.DrawLine(position + Vector3.back * iconSize, position - Vector3.back * iconSize);
                Gizmos.DrawLine(position + Vector3.right * iconSize, position - Vector3.right * iconSize);
            }
        }*/
    }
}