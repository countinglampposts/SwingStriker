using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swing.Character;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Swing.Sound
{
    public class GrapplingSoundEmitter : MonoBehaviour
    {
        [Inject] private SignalBus signalBus;
        [Inject] private SoundPlayer soundPlayer;

        private void Start()
        {
            signalBus.GetStream<GrapplingFiredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => soundPlayer.PlaySound("Grappling",transform));
        }
    }
}