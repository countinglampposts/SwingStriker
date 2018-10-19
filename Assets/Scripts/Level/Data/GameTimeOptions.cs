using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Swing.Level
{
    [CreateAssetMenu(menuName = "Swing/LevelTimeOptions")]
    public class GameTimeOptions : ScriptableObject, IScrollableAsset
    {
        public GameTime[] levelTimes;
        public IEnumerable<IDisplayAsset> assets
        {
            get
            {
                return levelTimes.Select((time) => time as IDisplayAsset);
            }
        }
    }

    [System.Serializable]
    public struct GameTime : IDisplayAsset
    {
        public int seconds;
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