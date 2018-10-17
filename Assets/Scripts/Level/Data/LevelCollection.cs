using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Swing.Level
{
    [CreateAssetMenu(menuName = "Swing/LevelCollection")]
    public class LevelCollection : ScriptableObject, IScrollableAsset
    {
        public LevelAsset[] levels;

        public IEnumerable<LevelAsset> assets
        {
            get
            {
                return levels;
            }
        }

        IEnumerable<IDisplayAsset> IScrollableAsset.assets
        {
            get
            {
                return levels.Select(asset => asset as IDisplayAsset);
            }
        }
    }
}