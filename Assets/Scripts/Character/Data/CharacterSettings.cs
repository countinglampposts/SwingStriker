using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    [System.Serializable]
    public struct CharacterSettings
    {
        [SerializeField]
        public float breakforce;
        [SerializeField]
        public float ropeSpringFrequency;
        [SerializeField]
        public GameObject bloodsplosionEffect;
        [SerializeField]
        public LineRenderer ropeEffect;
    }
}