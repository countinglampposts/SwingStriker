using System.Collections;
using System.Collections.Generic;
using Swing.Game;
using Swing.Player;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace Swing.Character
{
    public class CharacterPointOfInterest : MonoBehaviour
    {
        [Inject] CharacterState state;
        [Inject] GameCameraState cameraState;

        private void Start()
        {
            cameraState.pointsOfInterest.Add(transform);
            state.isCorpse
                 .Where(isCorpse => isCorpse)
                 .Select(_ => Unit.Default)
                 .Merge(gameObject.OnDestroyAsObservable())
                 .First()
                 .Subscribe(_ => cameraState.pointsOfInterest.Remove(transform));
        }
    }
}