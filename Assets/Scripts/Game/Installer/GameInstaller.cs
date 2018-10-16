using Zenject;
using System.Linq;
using UnityEngine;
using Swing.Level;
using Swing.Player;
using System.Collections.Generic;
using UniRx;
using Swing.Character;
using System;

namespace Swing.Game
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField]
        private LevelInstaller levelPrefab;
        [SerializeField]
        private PlayerData[] playersData;
        [SerializeField]
        private TeamData[] teams;
        [SerializeField]
        private SplitScreenLayout[] layouts;

        public override void InstallBindings()
        {
            var levelContext = Container.CreateSubContainer();
            levelContext.BindInstance(teams);
            var level = levelContext.InstantiatePrefab(levelPrefab).GetComponent<LevelInstaller>();

            var layout = layouts.First(a => a.settings.Length == playersData.Length);
            var spawned = new List<Tuple<PlayerData, GameObject>>();

            for (int a = 0; a < layout.settings.Length; a++)
            {
                var playerData = playersData[a];
                var instance = SpawnPlayer(playerData, layout.settings[a], level);

                spawned.Add(new Tuple<PlayerData, GameObject>(playerData, instance));
            }
            level.ResolvePlayerSpawn(spawned);
        }

        private GameObject SpawnPlayer(PlayerData playerData, CameraSettings cameraSettings, LevelInstaller level){
            var playerContext = Container.CreateSubContainer();
            playerContext.DeclareSignal<ResetPlayerSignal>();
            playerContext.BindInstance(teams.First(element => element.id == playerData.team));
            playerContext.BindInstance(cameraSettings);
            var instance = playerContext.InstantiatePrefab(playerData.prefab);

            playerContext.Resolve<SignalBus>()
                         .GetStream<ResetPlayerSignal>()
                         .Subscribe(_ =>
                         {
                             foreach(var a in instance.GetComponentsInChildren<ICharacterDriver>()){
                                 a.Disable();
                             }
                             var oldInstance = instance;
                             Observable.Timer(TimeSpan.FromSeconds(3f))
                                       .Subscribe(__=>Destroy(oldInstance));

                             instance = playerContext.InstantiatePrefab(playerData.prefab);
                             level.ResolvePlayerSpawn(new List<Tuple<PlayerData, GameObject>> { new Tuple<PlayerData, GameObject>(playerData, instance) });
                         });

            return instance;
        }
    }
}