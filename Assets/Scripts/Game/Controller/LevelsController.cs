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
    public class LevelsController : MonoInstaller
    {
        [SerializeField] GameObject rootMenu;
        [Inject] DiContainer container;

        public DiContainer levelSubcontainer { get; private set; }

        private GameObject currentLevelInstance;
        private int currentCampaignIndex;

        private void Start()
        {
            levelSubcontainer = container.CreateSubContainer();
        }

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }

        public void LaunchLevel()
        {
            var levelAsset = levelSubcontainer.Resolve<LevelAsset>();
            currentLevelInstance = levelSubcontainer.InstantiatePrefab(levelAsset.gameTypePrefab);
        }

        public void LaunchCampaign(int index){
            currentCampaignIndex = index;
            var collection = levelSubcontainer.Resolve<LevelCollection>();
            var level = collection.levels[index];
            levelSubcontainer.Unbind(level.GetType());
            levelSubcontainer.BindInstance(level);
            LaunchLevel();
        }

        public void LaunchNextLevel()
        {
            Destroy(currentLevelInstance);
            LaunchCampaign(currentCampaignIndex + 1);
        }

        public void ReturnToMainMenu(){
            ClearLevel();
            rootMenu.SetActive(true);
        }

        public void RestartLevel()
        {
            Destroy(currentLevelInstance);
            LaunchLevel();
        }

        private void ClearLevel()
        {
            levelSubcontainer.UnbindAll();
            Destroy(currentLevelInstance);
        }
    }
}