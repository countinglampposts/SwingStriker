using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Swing.Game.Soccer
{
    public class SoccerState
    {
        public IntReactiveProperty secondsRemaining = new IntReactiveProperty();
        public ReactiveDictionary<int, int> scores = new ReactiveDictionary<int, int>();
    }
}