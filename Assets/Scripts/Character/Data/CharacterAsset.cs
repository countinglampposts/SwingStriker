using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swing.Character
{
    [CreateAssetMenu(menuName = "Swing/Character")]
    public class CharacterAsset : ScriptableObject, IDisplayAsset
    {
        public string displayName;
        public GameObject prefab;

        string IDisplayAsset.displayName
        {
            get
            {
                return displayName;
            }
        }
    }
}