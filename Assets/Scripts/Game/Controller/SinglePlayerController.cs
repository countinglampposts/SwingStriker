using System;
using System.Collections;
using System.Collections.Generic;
using Swing.Level;
using Swing.Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Game
{
    public class SinglePlayerController : IDisposable, IInitializable
    {
        [Inject] SignalBus signalBus;
        [Inject] SpawnPointGroup spawnPointGroup;
        [Inject] PlayerData[] playerData;
        [Inject] LevelsController levelsController;

        CompositeDisposable disposables = new CompositeDisposable();

        public void Initialize()
        {
            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(KeyCode.W))
                      .Subscribe(_ => signalBus.Fire<LevelWonSignal>())
                      .AddTo(disposables);
            Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(KeyCode.L))
                      .Subscribe(_ => signalBus.Fire<LevelLostSignal>())
                      .AddTo(disposables);

            signalBus.GetStream<LevelWonSignal>()
                     .Subscribe(_=>levelsController.LaunchNextLevel())
                     .AddTo(disposables); //go to next level
            signalBus.GetStream<LevelLostSignal>()
                     .Subscribe(_=>levelsController.RestartLevel())
                     .AddTo(disposables); //reload the level

            spawnPointGroup.SpawnPlayerAtStart(playerData[0]);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

    }
}