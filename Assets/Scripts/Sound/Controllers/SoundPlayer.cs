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

        public IDisposable PlaySound(string assetId, Vector3 position, bool loop = false)
        {
            return PlaySound(assetId, null, position);
        }

        public IDisposable PlaySound(string assetId, Transform parent, bool loop = false)
        {
            return PlaySound(assetId, parent, Vector3.zero);
        }

        public IDisposable PlaySound(string assetId, Transform parent, Vector3 localPosition, bool loop = false)
        {
            var soundAsset = assets.sounds.FirstOrDefault(asset => asset.id == assetId);

            if (soundAsset != null)
            {
                GameObject createdObject = new GameObject("AudioSource");
                createdObject.transform.parent = parent;
                createdObject.transform.localPosition = localPosition;

                var audioSource = createdObject.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = audioMixerGroup;

                Action destroyAction = () => { if (createdObject != null) GameObject.Destroy(createdObject); };
                if (loop)
                {
                    audioSource.clip = soundAsset.clip;
                    audioSource.loop = true;
                    audioSource.Play();

                    return Disposable.Create(destroyAction);
                }
                else
                {
                    audioSource.PlayOneShot(soundAsset.clip);

                    return Observable.Timer(TimeSpan.FromSeconds(soundAsset.clip.length))
                              .Subscribe(_ => destroyAction())
                              .AddTo(Disposable.Create(destroyAction));
                }

            }
            Debug.LogWarning("Could not find sound asset of id " + assetId);
            return Disposable.Empty;
        }
    }
}