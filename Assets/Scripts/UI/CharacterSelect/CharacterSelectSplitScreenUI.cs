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

namespace Swing.UI
{
    public class CharacterSelectSplitScreenUI : MonoBehaviour
    {
        [SerializeField]
        private CharacterSelectUI[] selectUIs;
        [SerializeField]
        private GameObject root;
        [SerializeField]
        private GameObject backRoot;
        [SerializeField]
        private GameController controller;

        [Inject] DiContainer container;

        private void Start()
        {
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => gameObject.activeInHierarchy && gameObject.activeSelf)
                      .Where(_ => selectUIs.Any(ui => ui.isSelecting) && selectUIs.Where(ui => ui.isSelecting).All(ui => ui.isReady))
                      .Where(_ => InputManager.ActiveDevice.Action1.WasPressed)
                      .Subscribe(_ => {
                          var players = selectUIs.Where(ui => ui.isReady).Select(ui => ui.GetPlayerData()).ToArray();
                          container.BindInstance(players)
                                   .AsSingle();
                          container.InstantiatePrefab(controller);
                          root.SetActive(false);
                      });
            Observable.EveryUpdate()
                      .TakeUntilDestroy(this)
                      .Where(_ => gameObject.activeInHierarchy && gameObject.activeSelf)
                      .Where(_ => selectUIs.All(ui => !ui.isSelecting))
                      .Where(_ => InputManager.ActiveDevice.Action2.WasPressed)
                      .Subscribe(_ =>
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