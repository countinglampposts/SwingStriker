using System.Collections;
using System.Collections.Generic;
using Swing.Game;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Swing.UI
{
    public class PauseUI : MonoBehaviour
    {
        [SerializeField] private GameObject uiRoot;
        [SerializeField] private Button returnButton;
        [SerializeField] private Button quitButton;

        [Inject] GameState gameState;

        private void Start()
        {
            uiRoot.SetActive(false);
            gameState.isPaused
                     .TakeUntilDestroy(this)
                     .Subscribe(isPaused => uiRoot.SetActive(isPaused));
            returnButton.onClick.AsObservable()
                      .TakeUntilDestroy(this)
                      .Subscribe(_ => gameState.isPaused.Value = false);
            quitButton.onClick.AsObservable()
                      .TakeUntilDestroy(this)
                      .Subscribe(_ => Application.LoadLevel(Application.loadedLevel));
        }
    }
}