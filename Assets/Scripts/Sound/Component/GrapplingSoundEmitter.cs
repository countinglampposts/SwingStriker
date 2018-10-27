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
        [Inject] private SoundAssets sounds;
        [Inject] private SignalBus signalBus;
        [Inject] private AudioMixerGroup audioMixerGroup;

        private void Start()
        {
            signalBus.GetStream<GrapplingFiredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => AudioUtils.PlayAudioAtPosition(transform.position, sounds.sounds.FirstOrDefault(sound => sound.id == "Grappling").clip, audioMixerGroup));
        }
    }
}