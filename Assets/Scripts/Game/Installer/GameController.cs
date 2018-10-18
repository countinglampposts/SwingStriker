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
        [Inject] private LevelAsset levelAsset;
        [Inject] private PlayerData[] playersData;
        [Inject] private LevelTime time;
        [Inject] private SplitScreenLayouts layouts;
        [Inject] DiContainer container;

        public void Start()
        {
            var levelContext = container.CreateSubContainer();
            var level = levelContext.InstantiatePrefab(levelAsset.prefab).GetComponent<LevelInstaller>();

            var layout = layouts.layouts.First(a => a.settings.Length == playersData.Length);
            var spawned = new List<Tuple<PlayerData, GameObject>>();

            for (int a = 0; a < layout.settings.Length; a++)
            {
                var playerData = playersData[a];
                var device = InputManager.Devices.FirstOrDefault(selectedDevice => selectedDevice.GUID == playerData.deviceID);
                var instance = SpawnPlayer(playerData, layout.settings[a], device, level);

                spawned.Add(new Tuple<PlayerData, GameObject>(playerData, instance));
            }
            level.ResolvePlayerSpawn(spawned);
        }

        private GameObject SpawnPlayer(PlayerData playerData, CameraSettings cameraSettings, InputDevice inputDevice, LevelInstaller level){
            DiContainer playerContext = null;
            CharacterState characterState = null;
            GameObject instance = null;

            Action MakeNewPlayer = null;
            MakeNewPlayer = () =>
            {
                playerContext = container.CreateSubContainer();
                characterState = new CharacterState();
                playerContext.DeclareSignal<ResetPlayerSignal>();
                playerContext.BindInstance(characterState);
                playerContext.BindInstance(playerData.team);
                playerContext.BindInstance(cameraSettings);
                if(inputDevice != null) playerContext.BindInstance(inputDevice);
                instance = playerContext.InstantiatePrefab(playerData.character.prefab);
                playerContext.Resolve<SignalBus>()
                         .GetStream<ResetPlayerSignal>()
                         .Subscribe(_ =>
                         {
                             characterState.localPlayerControl.Value = false;

                             var oldInstance = instance;
                             Observable.Timer(TimeSpan.FromSeconds(3f))
                                       .Subscribe(__ => {
                                           characterState.isCorpse.Value = true;
                                           //----RESETS ALL VALUES-----
                                           MakeNewPlayer();

                                           level.ResolvePlayerSpawn(new List<Tuple<PlayerData, GameObject>> { new Tuple<PlayerData, GameObject>(playerData, instance) });
                                           Observable.Timer(TimeSpan.FromSeconds(30))
                                                     .Subscribe(___ => GameObject.Destroy(oldInstance));
                                       });

                         });
            };

            MakeNewPlayer();

            return instance;
        }
    }
}