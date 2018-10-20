using System.Collections;
using System.Collections.Generic;
using Swing.Player;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;

namespace Swing.Character
{
    public class CriticalJoint : MonoBehaviour
    {
        [Inject] private SignalBus signalBus;
        [Inject] private CharacterState state;

        private void OnJointBreak2D(Joint2D joint)
        {
            if(state.localPlayerControl.Value) signalBus.Fire<PlayerKilledSignal>();
        }
    }
}