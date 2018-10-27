using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swing.Character;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class GrapplingSoundEmitter : MonoBehaviour
    {
        [Inject] private SoundAssets sounds;
        [Inject] private SignalBus signalBus;

        private void Start()
        {
            var audioSource = GetComponent<AudioSource>();

            signalBus.GetStream<GrapplingFiredSignal>()
                     .TakeUntilDestroy(this)
                     .Subscribe(_ => audioSource.PlayOneShot(sounds.sounds.FirstOrDefault(sound => sound.id == "Grappling").clip));
        }
    }
}