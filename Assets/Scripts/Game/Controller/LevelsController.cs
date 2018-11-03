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
        [Inject] DiContainer container;

        public DiContainer levelSubcontainer { get; private set; }

        private void Start()
        {
            levelSubcontainer = container.CreateSubContainer();
        }

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }

        public IDisposable LaunchLevel()
        {
            var levelAsset = levelSubcontainer.Resolve<LevelAsset>();
            var currentLevelPrefab = levelSubcontainer.InstantiatePrefab(levelAsset.gameTypePrefab);

            return Disposable.Create(() =>
            {
                levelSubcontainer.UnbindAll();
                Destroy(currentLevelPrefab);
            });
        }
    }
}