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
        public GameObject bloodsplosionEffect;
        [SerializeField]
        public LineRenderer ropeEffect;
    }
}