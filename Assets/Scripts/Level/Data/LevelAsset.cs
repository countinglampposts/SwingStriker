using System.Collections;
using System.Collections.Generic;
using Swing.Character;
using UnityEngine;

namespace Swing.Level
{
    [CreateAssetMenu(menuName = "Swing/Level")]
    public class LevelAsset : ScriptableObject, IDisplayAsset
    {
        public GameObject prefab;
        public string displayName;
        public CharacterAsset defaultCharacter;

        string IDisplayAsset.displayName
        {
            get
            {
                return displayName;
            }
        }
    }
}