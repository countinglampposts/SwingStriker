using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Audio;

namespace Swing.Sound
{
    public static class AudioUtils
    {
        public static void PlayAudioAtPosition(Vector3 position, AudioClip clip, AudioMixerGroup audioMixerGroup = null)
        {
            GameObject createdObject = new GameObject("AudioSource");
            createdObject.transform.position = position;
            var audioSource = createdObject.AddComponent<AudioSource>();
            PlayAudio(audioSource, createdObject, clip, audioMixerGroup);
        }

        public static void PlayAudioOnObject(GameObject playedFrom, AudioClip clip, AudioMixerGroup audioMixerGroup = null){
            var audioSource = playedFrom.AddComponent<AudioSource>();
            PlayAudio(audioSource, audioSource, clip, audioMixerGroup);
        }

        private static void PlayAudio(AudioSource audioSource, Object destroyedObject, AudioClip clip, AudioMixerGroup audioMixerGroup = null)
        {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.PlayOneShot(clip);

            var audioFinishedStream = Observable.EveryUpdate()
                      .Where(_ => audioSource != null && !audioSource.isPlaying);

            Observable.EveryUpdate()
                      .TakeUntil(audioSource.OnDestroyAsObservable())
                      .TakeUntil(audioFinishedStream)
                      .Subscribe(_ => GameObject.Destroy(destroyedObject));
        }
    }
}