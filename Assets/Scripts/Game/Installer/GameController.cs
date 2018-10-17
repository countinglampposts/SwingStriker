using Zenject;
using System.Linq;
using UnityEngine;
using Swing.Level;
using Swing.Player;
using System.Collections.Generic;
using UniRx;
using Swing.Character;
using System;
using InControl;

namespace Swing.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private LevelAsset levelAsset;
        [SerializeField]
        private PlayerData[] playersData;

        [Inject]
        private TeamsData teams;
        [Inject]
        private SplitScreenLayouts layouts;

        [Inject] DiContainer container;

        private void Start()
        {
            var levelContext = container.CreateSubContainer();
            levelContext.BindInstance(teams);
            var level = levelContext.InstantiatePrefab(levelAsset.prefab).GetComponent<LevelInstaller>();

            var layout = layouts.layouts.First(a => a.settings.Length == playersData.Length);
            var spawned = new List<Tuple<PlayerData, GameObject>>();

            for (int a = 0; a < layout.settings.Length; a++)
            {
                var playerData = playersData[a];
                var instance = SpawnPlayer(playerData, layout.settings[a], InputManager.Devices[a], level);

                spawned.Add(new Tuple<PlayerData, GameObject>(playerData, instance));
            }
            level.ResolvePlayerSpawn(spawned);
        }

        private GameObject SpawnPlayer(PlayerData playerData, CameraSettings cameraSettings, InputDevice inputDevice, LevelInstaller level){
            var playerContext = container.CreateSubContainer();
            var characterState = new CharacterState();
            playerContext.BindInstance(characterState);
            playerContext.DeclareSignal<ResetPlayerSignal>();
            playerContext.BindInstance(teams.teams.First(element => element.id == playerData.team));
            playerContext.BindInstance(cameraSettings);
            playerContext.BindInstance(inputDevice);
            var instance = playerContext.InstantiatePrefab(playerData.prefab);

            playerContext.Resolve<SignalBus>()
                         .GetStream<ResetPlayerSignal>()
                         .Subscribe(_ =>
                         {
                             characterState.localPlayerControl.Value = false;

                             var oldInstance = instance;
                             Observable.Timer(TimeSpan.FromSeconds(3f))
                                       .Subscribe(__ => {
                                           instance = playerContext.InstantiatePrefab(playerData.prefab);
                                           characterState.localPlayerControl.Value = true;

                                           level.ResolvePlayerSpawn(new List<Tuple<PlayerData, GameObject>> { new Tuple<PlayerData, GameObject>(playerData, instance) });

                                           Destroy(oldInstance); 
                                        });

                         });

            return instance;
        }
    }
}