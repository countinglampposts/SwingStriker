using System.Collections;
using System.Collections.Generic;
using Swing.Player;
using UnityEngine;
using Zenject;

namespace Swing.Character
{
    public class CriticalJoint : MonoBehaviour, ICharacterDriver
    {
        [Inject] private SignalBus signalBus;

        private void OnJointBreak2D(Joint2D joint)
        {
            if(enabled) signalBus.Fire<ResetPlayerSignal>();
        }

        public void Disable()
        {
            enabled = false;
        }
    }
}