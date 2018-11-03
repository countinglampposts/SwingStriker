using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Swing.Game
{
    public class GameState
    {
        public BoolReactiveProperty isPaused = new BoolReactiveProperty(false);
    }
}
