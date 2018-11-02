using System.Collections;
using System.Collections.Generic;
using Swing.Game;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
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
            var pauseStream = gameState.isPaused.TakeUntilDestroy(this);

            pauseStream
                .Subscribe(isPaused => { 
                    uiRoot.SetActive(isPaused);
                    EventSystem.current.sendNavigationEvents = isPaused;
            });

            pauseStream
                .Where(isPaused => isPaused)
                .Subscribe(_ => EventSystem.current.SetSelectedGameObject(returnButton.gameObject));

            returnButton.onClick.AsObservable()
                      .TakeUntilDestroy(this)
                      .Subscribe(_ => gameState.isPaused.Value = false);
            quitButton.onClick.AsObservable()
                      .TakeUntilDestroy(this)
                      .Subscribe(_ => ProjectUtils.ReloadLevel());
        }
    }
}