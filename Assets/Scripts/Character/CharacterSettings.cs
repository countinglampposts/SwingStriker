using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    [System.Serializable]
    public struct CharacterSettings
    {
        [SerializeField]
        public float breakforce;
        [SerializeField]
        public float ropeSpringFrequency;
    }
}