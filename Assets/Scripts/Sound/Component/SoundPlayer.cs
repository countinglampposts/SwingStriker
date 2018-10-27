using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Swing.Sound
{
    public class SoundPlayer 
    {
        [Inject] AudioMixerGroup audioMixerGroup;
        [Inject] SoundAssets assets;

        public void PlayAudioAtPosition(string clipId, Vector3 position)
        {
            GameObject createdObject = new GameObject("AudioSource");
            createdObject.transform.position = position;
            var audioSource = createdObject.AddComponent<AudioSource>();
            PlayAudio(clipId, audioSource, createdObject);
        }

        public void PlayAudioOnObject(string clipId, GameObject playedFrom)
        {
            var audioSource = playedFrom.AddComponent<AudioSource>();
            PlayAudio(clipId, audioSource, audioSource);
        }

        private void PlayAudio(string clipId, AudioSource audioSource, Object destroyedObject)
        {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.PlayOneShot(assets.sounds.FirstOrDefault(asset => asset.id == clipId).clip);

            Observable.EveryUpdate()
                      .TakeUntilDestroy(audioSource)
                      .First(_ => !audioSource.isPlaying)
                      .Subscribe(_ => GameObject.Destroy(destroyedObject));
        }
    }
}