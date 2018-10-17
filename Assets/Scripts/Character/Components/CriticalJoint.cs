using System.Collections;
using System.Collections.Generic;
using Swing.Player;
using UnityEngine;
using Zenject;
using UniRx;

namespace Swing.Character
{
    public class CriticalJoint : MonoBehaviour
    {
        [Inject] private SignalBus signalBus;
        [Inject] private CharacterState state;

        private void Start()
        {
            state.localPlayerControl
                 .TakeUntilDestroy(this)
                 .Subscribe(localControl => enabled = localControl);
        }

        private void OnJointBreak2D(Joint2D joint)
        {
            if(enabled) signalBus.Fire<ResetPlayerSignal>();
        }
    }
}