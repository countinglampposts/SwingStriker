using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    [SerializeField]
    GameObject attached;

    private void Start()
    {
        Vector3 hitPosition = attached.transform.position;

        var anchor = new GameObject("GrapplingAnchor");
        anchor.transform.position = hitPosition;

        var anchorRigidbody = anchor.AddComponent<Rigidbody2D>();
        anchorRigidbody.isKinematic = true;

        var joint = anchor.gameObject.AddComponent<SpringJoint2D>();
        joint.anchor = anchor.transform.InverseTransformPoint(hitPosition);
        joint.autoConfigureDistance = false;
        joint.distance = 0;
        joint.dampingRatio = 1;
        joint.frequency = 1;
        joint.enableCollision = true;

        var rigidbody = GetComponent<Rigidbody2D>();
        joint.connectedBody = rigidbody;
    }
}
