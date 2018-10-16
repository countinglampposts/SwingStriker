using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swing.Level
{
    public class Wind : MonoBehaviour
    {
        [SerializeField] Vector2 force;

        List<Rigidbody2D> collidedObjects = new List<Rigidbody2D>();

        private void OnTriggerEnter2D(Collider2D collider)
        {
            var rigidbody = collider.GetComponent<Rigidbody2D>();
            if (rigidbody != null) collidedObjects.Add(rigidbody);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            var rigidbody = collider.GetComponent<Rigidbody2D>();
            if (rigidbody != null) collidedObjects.Remove(rigidbody);
        }

        private void FixedUpdate()
        {
            foreach(var rigidbody in collidedObjects){
                rigidbody.AddForce(transform.TransformDirection(force),ForceMode2D.Force);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(force));
        }
    }
}