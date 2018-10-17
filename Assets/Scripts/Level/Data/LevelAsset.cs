using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swing.Level
{
    [CreateAssetMenu(menuName = "Swing/Level")]
    public class LevelAsset : ScriptableObject, IDisplayAsset
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