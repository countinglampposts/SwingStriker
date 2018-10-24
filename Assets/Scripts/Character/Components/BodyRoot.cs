using System;
using UnityEngine;
using Zenject;

namespace Swing.Character
{
    public class BodyRoot : MonoBehaviour
    {
        [SerializeField]
        public BodyPart rootBodyPart;

        [Inject] CharacterSettings settings;
        [Inject] DiContainer container;

        private void Start()
        {
            Rig(rootBodyPart);
        }

        private void Rig(BodyPart body)
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
                child.GetComponent<Collider2D>().sharedMaterial = settings.physicsMaterial;
                var gameObject = child.gameObject;
                var rigidBody = gameObject.AddComponent<Rigidbody2D>();
                rigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;
                //rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rigidBody.mass = settings.mass;
                rigidBody.drag = rigidBody.angularDrag = settings.drag;
                rigidBody.gravityScale = settings.gravity;

                if (parent != null)
                {
                    var joint = gameObject.AddComponent<HingeJoint2D>();
                    joint.anchor = child.pivot;
                    joint.enableCollision = true;
                    joint.breakForce = settings.breakforce;

                    container.Inject(gameObject.AddComponent<Bloodsplosion>());
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