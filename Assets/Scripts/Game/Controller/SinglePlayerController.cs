using System;
using System.Collections;
using System.Collections.Generic;
using Swing.Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Game
{
    public class SinglePlayerController : IDisposable, IInitializable
    {
        [Inject] SignalBus signalBus;
        [Inject] PlayerLifeController playerLifeController;
        [Inject] PlayerData[] playerData;

        CompositeDisposable disposables = new CompositeDisposable();

        public void Initialize()
        {
            signalBus.GetStream<LevelWonSignal>(); //go to next level
            signalBus.GetStream<LevelLostSignal>(); //reload the level

            Debug.Log("Initializing thplaer");
            playerLifeController.InitializePlayer(playerData[0]);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }

    }
}