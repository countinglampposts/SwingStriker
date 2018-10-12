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
                gameObject.AddComponent<Rigidbody2D>();

                if (parent != null)
                {
                    var joint = gameObject.AddComponent<HingeJoint2D>();
                    joint.anchor = child.pivot;
                    joint.enableCollision = true;
                }
            };

            Action<BodyPart, BodyPart> rigChildComponents = (BodyPart parent, BodyPart child) =>
            {
                if (parent != null)
                {
                    var childJoint = child.GetComponent<Joint2D>();
                    childJoint.connectedBody = parent.GetComponent<Rigidbody2D>();
                }
            };

            applyRecursive(null, body, addChildComponents);
            applyRecursive(null, body, rigChildComponents);
        }
    }
}