using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Swing.Level;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

namespace Swing.UI
{
    public class SinglePlayerUI : MonoBehaviour
    {
        [SerializeField]
        public AssetScroller levelScroller;
        [SerializeField]
        public Button button;

        [Inject] LevelCollection[] levels;

        private void Start()
        {
            var levelCollection = levels.First(l => l.id == "SP");
            levelScroller.Init(levelCollection);
            button.onClick.AsObservable()
                  .TakeUntilDestroy(this)
                  .Subscribe(_ =>
                  {
                      var level = levelCollection.levels[levelScroller.CurrentIndex()];
                  });
        }
    }
}