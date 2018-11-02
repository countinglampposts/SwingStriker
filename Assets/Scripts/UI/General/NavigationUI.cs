using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Swing.UI
{
    public class NavigationUI : MonoBehaviour
    {
        [System.Serializable]
        public class NavigationButton{
            public Button button;
            public GameObject uiRoot;
        }

        [SerializeField]
        private GameObject root;
        [SerializeField]
        private NavigationButton[] buttons;

        private void Start()
        {
            foreach(var b in buttons){
                var button = b;
                button.button.onClick.AsObservable()
                 .TakeUntilDestroy(this)
                 .Subscribe(_ =>
                 {
                     button.uiRoot.SetActive(true);
                     root.SetActive(false);
                 });
            }
        }
    }
}