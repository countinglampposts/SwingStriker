using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Swing.Character
{
    public struct ReplayData{
        public List<TransformFrame> frames;
    }

    public struct TransformFrame{
        public Vector2 position;
        public float rotation;
    }

    public class ReplayController : MonoBehaviour
    {
        ReplayData replayData;
        CompositeDisposable disposables = new CompositeDisposable();

        private void Start()
        {
            StartRecording();
        }

        public void StartRecording(){
            replayData = new ReplayData
            {
                frames = new List<TransformFrame>()
            };
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Subscribe(_=>
            {
                replayData.frames.Add(new TransformFrame
                {
                    position = transform.position,
                    rotation = transform.rotation.eulerAngles.z
                });
            }).AddTo(disposables);
        }

        public void ReplayRecording()
        {
            disposables.Clear();

            Physics2D.autoSimulation = false;
            int counter = 0;
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .TakeWhile(_ => counter < replayData.frames.Count)
                      .Subscribe(_ =>
                      {
                          transform.position = replayData.frames[counter].position;
                          transform.rotation = Quaternion.Euler(0, 0, replayData.frames[counter].rotation);
                          counter++;
                      });
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Play")) ReplayRecording();
        }
    }
}
