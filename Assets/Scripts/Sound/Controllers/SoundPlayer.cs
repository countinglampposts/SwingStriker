using System;
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

        public IDisposable PlaySound(string clipId, Vector3 position)
        {
            return PlaySound(clipId, null, position);
        }

        public IDisposable PlaySound(string clipId, Transform parent)
        {
            return PlaySound(clipId, parent, Vector3.zero);
        }

        public IDisposable PlaySound(string clipId, Transform parent, Vector3 position)
        {
            var soundAsset = assets.sounds.FirstOrDefault(asset => asset.id == clipId);

            if (soundAsset != null)
            {
                GameObject createdObject = new GameObject("AudioSource");
                createdObject.transform.parent = parent;
                createdObject.transform.localPosition = position;
                var audioSource = createdObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = audioMixerGroup;

                Action destroyAction = () => { if (createdObject != null) GameObject.Destroy(createdObject); };

                audioSource.PlayOneShot(soundAsset.clip);

                return Observable.Timer(TimeSpan.FromSeconds(soundAsset.clip.length))
                          .Subscribe(_ => destroyAction())
                          .AddTo(Disposable.Create(destroyAction));
            }
            return Disposable.Empty;
        }
    }
}