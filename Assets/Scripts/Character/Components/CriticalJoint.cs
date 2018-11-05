using UnityEngine;
using Zenject;
using Swing.Game;

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