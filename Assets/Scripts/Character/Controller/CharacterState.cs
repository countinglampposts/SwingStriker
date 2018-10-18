using UniRx;
using UnityEngine;

namespace Swing.Character
{
    public class CharacterState
    {
        public ReactiveProperty<bool> localPlayerControl = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> isCorpse = new ReactiveProperty<bool>(false);
        public Vector2ReactiveProperty aimDirection = new Vector2ReactiveProperty(Vector2.zero);
    }
}