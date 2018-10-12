using System;
using UnityEngine;

namespace Characters
{

    public class BodyRigger : MonoBehaviour
    {
        [SerializeField]
        private BodyPart body;

        private void Start()
        {
            Rig(body);
        }

        private static void Rig(BodyPart body)
        {
            Action<BodyPart, BodyPart, Action<BodyPart, BodyPart>> applyRecursive = null;
            applyRecursive = (BodyPart parent, BodyPart target, Action<BodyPart, BodyPart> action) =>
            {
                action(parent, target);
                foreach (var child in target.children)
                {
                    applyRecursive(target, child, action);
                }
            };

            Action<BodyPart, BodyPart> addChildComponents = (BodyPart parent, BodyPart child) =>
            {
                var gameObject = child.gameObject;
                var rigidbody = gameObject.AddComponent<Rigidbody>();
                rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

                if (parent != null)
                {
                    /*var joint = gameObject.AddComponent<SpringJoint>();
                    joint.anchor = child.pivot;
                    joint.axis = Vector3.forward;
                    joint.spring = 10000;
                    joint.damper = .0001f;
                    joint.enableCollision = true;*/

                    var joint = gameObject.AddComponent<HingeJoint>();
                    joint.anchor = child.pivot;
                    joint.axis = Vector3.forward;
                    joint.enableCollision = true;

                    joint.useSpring = true;
                    JointSpring spring = joint.spring;
                    spring.spring = 10000;
                    spring.damper = .0001f;
                    joint.spring = spring;
                }
            };

            Action<BodyPart, BodyPart> rigChildComponents = (BodyPart parent, BodyPart child) =>
            {
                if (parent != null)
                {
                    var childJoint = child.GetComponent<Joint>();
                    childJoint.connectedBody = parent.GetComponent<Rigidbody>();
                }
            };

            applyRecursive(null, body, addChildComponents);
            applyRecursive(null, body, rigChildComponents);
        }
    }
}