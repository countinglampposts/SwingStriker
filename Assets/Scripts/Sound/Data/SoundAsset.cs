using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swing.Sound
{
    [CreateAssetMenu(menuName = "Swing/Sound Asset")]
    public class SoundAsset : ScriptableObject
    {
        public string id;
        public AudioClip clip;
    }
}