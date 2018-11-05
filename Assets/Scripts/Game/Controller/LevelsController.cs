using Swing.Level;
using UnityEngine;
using Zenject;

namespace Swing.Game
{
    /// <summary>
    /// This owns the flow of various levels.
    /// To use:
    ///  Bind all level dependencies to levelSubcontainer,
    ///  then use LaunchLevel() or LaunchCampaign()
    /// Use ReturnToMainMenu() to exit the level
    /// </summary>
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

        /// <summary>
        /// Launches the level based on the LevelAsset bound to levelSubcontainer
        /// </summary>
        public void LaunchLevel()
        {
            var levelAsset = levelSubcontainer.Resolve<LevelAsset>();
            currentLevelInstance = levelSubcontainer.InstantiatePrefab(levelAsset.gameTypePrefab);
        }

        /// <summary>
        /// Launches a campaign based on the LevelCollection bound to levelSubcontainer
        /// </summary>
        /// <param name="index">The index of the level in LevelCollection to launch</param>
        public void LaunchCampaign(int index)
        {
            currentCampaignIndex = index;
            var collection = levelSubcontainer.Resolve<LevelCollection>();
            var level = collection.levels[index];
            levelSubcontainer.Unbind(level.GetType());
            levelSubcontainer.BindInstance(level);
            LaunchLevel();
        }

        /// <summary>
        /// Launches the next level in the campaign
        /// </summary>
        public void LaunchNextLevel()
        {
            Destroy(currentLevelInstance);
            LaunchCampaign(currentCampaignIndex + 1);
        }

        public void ReturnToMainMenu()
        {
            ClearSubcontainer();
            rootMenu.SetActive(true);
        }

        public void RestartLevel()
        {
            Destroy(currentLevelInstance);
            LaunchLevel();
        }

        public void ClearSubcontainer()
        {
            if (currentLevelInstance != null) Destroy(currentLevelInstance);
            levelSubcontainer.UnbindAll();
            levelSubcontainer = container.CreateSubContainer();
        }
    }
}