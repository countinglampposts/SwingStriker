using System.Collections;
using System.Collections.Generic;
using InControl;
using UniRx;
using UnityEngine;
using System.Linq;
using Swing.Player;
using Zenject;
using Swing.Game;
using UniRx.Diagnostics;
using UnityEngine.UI;

namespace Swing.UI
{
    public class CharacterSelectSplitScreenUI : MonoBehaviour
    {
        [SerializeField]
        private CharacterSelectUI[] selectUIs;
        [SerializeField]
        private Button goButton;
        [SerializeField]
        private Button backButton;
        [SerializeField]
        private GameObject root;
        [SerializeField]
        private GameObject backRoot;
        [SerializeField]
        private GameController controller;

        [Inject] DiContainer container;

        private void Start()
        {
            // Init go button
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => gameObject.activeInHierarchy && gameObject.activeSelf)
                      .Select(_ => selectUIs.Any(ui => ui.isSelecting) && selectUIs.Where(ui => ui.isSelecting).All(ui => ui.isReady))
                      .DistinctUntilChanged()
                      .Subscribe(showButton => goButton.gameObject.SetActive(showButton));
            UIUtils.AddGamepadButtonPressToButton(goButton, 0);
            goButton.onClick.AddListener(() =>
            {
                var players = selectUIs.Where(ui => ui.isReady).Select(ui => ui.GetPlayerData()).ToArray();
                container.Unbind(players.GetType());
                container.BindInstance(players);
                container.InstantiatePrefab(controller);
                root.SetActive(false);
            });

            // Init back button
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => gameObject.activeInHierarchy && gameObject.activeSelf)
                      .Select(_ => selectUIs.All(ui => !ui.isSelecting))
                      .Subscribe(showButton => backButton.gameObject.SetActive(showButton));
            UIUtils.AddGamepadButtonPressToButton(backButton, 1);
            backButton.onClick.AddListener(() =>
            {
                root.SetActive(false);
                backRoot.SetActive(true);
            });

            for (int a = 0; a < InputManager.Devices.Count;a++){
                selectUIs[a].Init(InputManager.Devices[a].GUID);
            }
        }
    }
}