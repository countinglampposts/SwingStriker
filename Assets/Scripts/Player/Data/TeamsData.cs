using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Swing.Player
{
    [CreateAssetMenu(menuName = "Swing/TeamsData")]
    public class TeamsData : ScriptableObject, IScrollableAsset{
        public TeamData[] teams;

        public IEnumerable<IDisplayAsset> assets
        {
            get
            {
                return teams.Select(team => team as IDisplayAsset);
            }
        }
    }

    [System.Serializable]
    public struct TeamData : IDisplayAsset
    {
        public int id;
        public Color color;
        public string name;

        public string displayName
        {
            get
            {
                return name;
            }
        }
    }
}