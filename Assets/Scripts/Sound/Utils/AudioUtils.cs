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
        public static void PlayAudioOnObject(GameObject playedFrom, AudioClip clip, AudioMixerGroup audioMixerGroup = null){
            var audioSource = playedFrom.AddComponent<AudioSource>();

            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.PlayOneShot(clip);

            var audioFinishedStream = Observable.EveryUpdate()
                      .Where(_ => audioSource != null && !audioSource.isPlaying);

            Observable.EveryUpdate()
                      .TakeUntil(audioSource.OnDestroyAsObservable())
                      .TakeUntil(audioFinishedStream)
                      .Subscribe(_ => GameObject.Destroy(audioSource));
        }
    }
}