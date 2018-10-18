using System.Collections;
using System.Collections.Generic;
using Swing.Character;
using Swing.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using InControl;
using System;
using UniRx;

namespace Swing.UI
{
    public class CharacterSelectUI : MonoBehaviour
    {
        [System.Serializable]
        public class SignInScreen
        {
            public GameObject root;
            public Button nextButton;
        }
        [System.Serializable]
        public class TeamSelectScreen
        {
            public GameObject root;
            public AssetScroller assetScroller;
            public Button backButton;
            public Button nextButton;
        }
        [System.Serializable]
        public class CharacterSelectScreen
        {
            public GameObject root;
            public AssetScroller assetScroller;
            public Button backButton;
            public Button nextButton;
        }
        [System.Serializable]
        public class ReadyScreen
        {
            public GameObject root;
            public Button backButton;
        }
        [SerializeField]
        private SignInScreen signInScreen;
        [SerializeField]
        private TeamSelectScreen teamSelectScreen;
        [SerializeField]
        private CharacterSelectScreen characterSelectScreen;
        [SerializeField]
        private ReadyScreen readyScreen;

        [Inject] private CharacterCollection characters;
        [Inject] private TeamsData teams;

        public bool isClaimed { get; private set; }
        public bool isSelecting { get; private set; }
        public bool isReady { get; private set; }

        private CompositeDisposable disposables = new CompositeDisposable();

        private Guid deviceID;

        private void Start()
        {
            characterSelectScreen.assetScroller.Init(characters);
            teamSelectScreen.assetScroller.Init(teams);

            ResetUIElements();
        }

        private void ResetUIElements()
        {
            signInScreen.root.SetActive(true);
            characterSelectScreen.root.SetActive(false);
            teamSelectScreen.root.SetActive(false);
            readyScreen.root.SetActive(false);
        }

        public void BindToDevice(Guid deviceID)
        {
            if (isClaimed) Debug.LogError("Trying to set a new deviceID for claimed CharacterSelectUI");

            isClaimed = true;

            this.deviceID = deviceID;

            UIUtils.AddGamepadButtonPressToButton(signInScreen.nextButton, 0, deviceID)
                   .AddTo(disposables);
            signInScreen.nextButton.onClick.AsObservable()
                        .Subscribe(_ =>
                        {
                            signInScreen.root.SetActive(false);
                            characterSelectScreen.root.SetActive(true);
                            isSelecting = true;
                        })
                   .AddTo(disposables);

            // Init Character Screen
            UIUtils.AddGamepadButtonPressToButton(characterSelectScreen.nextButton, 0, deviceID)
                   .AddTo(disposables);
            characterSelectScreen.nextButton.onClick.AsObservable()
                        .Subscribe(_ =>
                        {
                            characterSelectScreen.root.SetActive(false);
                            teamSelectScreen.root.SetActive(true);
                        })
                   .AddTo(disposables);

            UIUtils.AddGamepadButtonPressToButton(characterSelectScreen.backButton, 1, deviceID)
                   .AddTo(disposables);
            characterSelectScreen.backButton.onClick.AsObservable()
                        .Subscribe(_ =>
                        {
                            characterSelectScreen.root.SetActive(false);
                            signInScreen.root.SetActive(true);
                            isSelecting = false;
                        })
                   .AddTo(disposables);
            characterSelectScreen.assetScroller.BindToDevice(deviceID)
                   .AddTo(disposables);



            // Init team screen 
            UIUtils.AddGamepadButtonPressToButton(teamSelectScreen.nextButton, 0, deviceID)
                   .AddTo(disposables);
            teamSelectScreen.nextButton.onClick.AsObservable()
                        .Subscribe(_ =>
                        {
                            teamSelectScreen.root.SetActive(false);
                            readyScreen.root.SetActive(true);
                            isReady = true;
                        })
                   .AddTo(disposables);

            UIUtils.AddGamepadButtonPressToButton(teamSelectScreen.backButton, 1, deviceID)
                   .AddTo(disposables);
            teamSelectScreen.backButton.onClick.AsObservable()
                        .Subscribe(_ =>
                        {
                            teamSelectScreen.root.SetActive(false);
                            characterSelectScreen.root.SetActive(true);
                        })
                   .AddTo(disposables);

            teamSelectScreen.assetScroller.BindToDevice(deviceID)
                   .AddTo(disposables);



            // Init ready screen
            UIUtils.AddGamepadButtonPressToButton(readyScreen.backButton, 1, deviceID)
                   .AddTo(disposables);
            readyScreen.backButton.onClick.AsObservable()
                        .Subscribe(_ =>
                        {
                            readyScreen.root.SetActive(false);
                            teamSelectScreen.root.SetActive(true);
                            isReady = false;
                        })
                   .AddTo(disposables);

            Observable.FromEvent<InputDevice>(d => InputManager.OnDeviceDetached += d, d => InputManager.OnDeviceDetached += d)
                      .Where(device => device.GUID == deviceID)
                      .Subscribe(_ => {
                          isReady = false;
                          isSelecting = false;
                          isClaimed = false;

                          disposables.Clear();

                          ResetUIElements();
                      })
                      .AddTo(disposables);
        }

        public PlayerData GetPlayerData(){
            return new PlayerData
            {
                character = characters.characters[characterSelectScreen.assetScroller.CurrentIndex()],
                team = teams.teams[teamSelectScreen.assetScroller.CurrentIndex()],
                deviceID = deviceID
            };
        }

        private void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}