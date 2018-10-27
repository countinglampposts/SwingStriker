using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swing.Character
{
    [System.Serializable]
    public struct CharacterSettings
    {
        [SerializeField]
        public float breakforce;
        [SerializeField]
        public float ropeSpringFrequency;
        [SerializeField]
        public float grapplingDistance;
        [SerializeField]
        public float mass;
        [SerializeField]
        public float drag;
        [SerializeField]
        public float gravity;
        [SerializeField]
        public float ropeClimbSpeed;
        [SerializeField]
        public float swingForce;
        [SerializeField]
        public float initialGrapplingDistanceRatio;
        [SerializeField]
        public float grapplingHookSpeed;
        [SerializeField]
        public GameObject bloodsplosionEffect;
        [SerializeField]
        public LineRenderer ropeEffect;
        [SerializeField]
        public LineRenderer laserEffect;
        [SerializeField]
        public PhysicsMaterial2D physicsMaterial;
        [SerializeField]
        public LayerMask grapplingMask;
    }
}