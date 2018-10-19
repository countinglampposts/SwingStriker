using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Swing.Game
{
    public class GameState
    {
        public IntReactiveProperty secondsRemaining = new IntReactiveProperty();
        public ReactiveDictionary<int, int> scores = new ReactiveDictionary<int, int>();
    }
}