using UniRx;

namespace Swing.Character
{
    public class CharacterState
    {
        public ReactiveProperty<bool> localPlayerControl = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> isCorpse = new ReactiveProperty<bool>(false);
    }
}