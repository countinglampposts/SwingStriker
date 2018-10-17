using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swing.Character
{
    [CreateAssetMenu(menuName = "Swing/Character")]
    public class CharacterAsset : ScriptableObject, IDisplayAsset
    {
        public GameObject prefab;
        public string displayName;

        string IDisplayAsset.displayName
        {
            get
            {
                return displayName;
            }
        }
    }
}