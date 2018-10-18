using System;
using System.Collections;
using System.Collections.Generic;
using Swing.Character;
using UnityEngine;

namespace Swing.Player
{
    [System.Serializable]
    public struct PlayerData
    {
        public TeamData team;
        public CharacterAsset character;
        public Guid deviceID;
    }
}