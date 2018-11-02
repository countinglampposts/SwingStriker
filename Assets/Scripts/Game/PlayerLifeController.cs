using System;
using System.Collections;
using System.Collections.Generic;
using Swing.Character;
using Swing.Level;
using Swing.Player;
using UniRx;
using UnityEngine;
using Zenject;

namespace Swing.Game
{
    public class PlayerLifeController
    {
        [Inject] SpawnPointGroup spawnPoints;
        [Inject] GameState gameState;
        [Inject] DiContainer container;

        public GameObject InitializePlayer(PlayerData playerData)
        {
            DiContainer playerContext = null;
            CharacterState characterState = null;
            GameObject instance = null;

            Action MakeNewPlayer = null;
            MakeNewPlayer = () =>
            {
                playerContext = container.CreateSubContainer();
                characterState = new CharacterState();
                playerContext.DeclareSignal<PlayerKilledSignal>();
                playerContext.BindInstance(characterState);
                playerContext.BindInstance(playerData);
                instance = playerContext.InstantiatePrefab(playerData.character.prefab);

                var playerKilledStream = playerContext.Resolve<SignalBus>()
                                                      .GetStream<PlayerKilledSignal>();

            // Disabled player control when paused
            gameState.isPaused
                         .TakeUntilDestroy(instance)
                         .Subscribe(isPaused => characterState.localPlayerControl.Value = !isPaused);

            // Reset the player when killed via recursion
            playerKilledStream
                         .First()
                         .TakeUntilDestroy(instance)
                         .Subscribe(_ =>
                         {
                             characterState.localPlayerControl.Value = false;

                             var oldInstance = instance;
                             Observable.Timer(TimeSpan.FromSeconds(3f))
                                       .TakeUntilDestroy(oldInstance)
                                       .Subscribe(__ =>
                                       {
                                           characterState.isCorpse.Value = true;
                                           Observable.Timer(TimeSpan.FromSeconds(30))
                                                     .TakeUntilDestroy(oldInstance)
                                                     .Subscribe(___ => GameObject.Destroy(oldInstance));

                                       //----RESETS ALL VALUES-----
                                       MakeNewPlayer();

                                           spawnPoints.ResolvePlayerSpawn(new List<Tuple<PlayerData, GameObject>> { new Tuple<PlayerData, GameObject>(playerData, instance) });
                                       });

                         });



            };

            MakeNewPlayer();

            return instance;
        }
    }
}