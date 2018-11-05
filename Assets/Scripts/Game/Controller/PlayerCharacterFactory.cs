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
    /// <summary>
    /// This creates a new character controlled by the player
    /// This handles
    ///  - Re/Spawning logic
    ///  - Creating the player subcontext
    ///  - Disabling and Enabling player control when paused
    ///  - Binding the PlayerData to the new subcontext
    /// </summary>
    public class PlayerCharacterFactory
    {
        [Inject] GameState gameState;
        [Inject] DiContainer container;

        /// <summary>
        /// Creates new Character representing the data about the player
        /// </summary>
        /// <returns>The player character's prefab</returns>
        /// <param name="playerData">Player data.</param>
        public GameObject SpawnPlayerCharacter(PlayerData playerData)
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

                // HACK: There has to be a better way to do this
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
                                           /// Keep the body alive for 30 sexonds
                                           Observable.Timer(TimeSpan.FromSeconds(30))
                                                     .TakeUntilDestroy(oldInstance)
                                                     .Subscribe(___ => GameObject.Destroy(oldInstance));
                                           
                                            MakeNewPlayer();
                                       });

                         });
            };

            MakeNewPlayer();

            return instance;
        }
    }
}