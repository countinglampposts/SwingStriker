using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swing.Player
{
    [CreateAssetMenu(menuName = "Swing/TeamsData")]
    public class TeamsData : ScriptableObject{
        public TeamData[] teams;
    }

    [System.Serializable]
    public struct TeamData
    {
        public int id;
        public Color color;
    }
}