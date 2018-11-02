using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swing.Level;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using Swing.Player;
using System;

namespace Swing.UI
{
    public class SinglePlayerUI : MonoBehaviour
    {
        [SerializeField]
        private AssetScroller levelScroller;
        [SerializeField]
        private Button button;
        [SerializeField]
        private GameObject root;
        [SerializeField]
        private GameObject gamePrefab;

        [Inject] LevelCollection[] levels;
        [Inject] DiContainer container;

        private void Start()
        {
            var levelCollection = levels.First(l => l.id == "SP");
            levelScroller.Init(levelCollection);
            button.onClick.AsObservable()
                  .TakeUntilDestroy(this)
                  .Subscribe(_ =>
                  {
                      var levelSubcontainer = container.CreateSubContainer();
                      var levelAsset = levelCollection.levels[levelScroller.CurrentIndex()];

                      levelSubcontainer.BindInstance(levelAsset);
                      levelSubcontainer.BindInstance(new PlayerData { deviceID = Guid.Empty, character = levelAsset.defaultCharacter });
                      levelSubcontainer.InstantiatePrefab(gamePrefab);

                      root.SetActive(false);
                  });
        }
    }
}