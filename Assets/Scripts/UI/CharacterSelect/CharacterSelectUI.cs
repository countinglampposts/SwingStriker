using System.Collections;
using System.Collections.Generic;
using Swing.Character;
using Swing.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using InControl;
using System;

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

        public bool isSelecting { get; private set; }
        public bool isReady { get; private set; }

        private Guid deviceID;

        public void Init(Guid deviceID)
        {
            this.deviceID = deviceID;

            UIUtils.AddGamepadButtonPressToButton(signInScreen.nextButton, 0, deviceID);
            signInScreen.nextButton.onClick.AddListener(() =>
            {
                signInScreen.root.SetActive(false);
                characterSelectScreen.root.SetActive(true);
                isSelecting = true;
            });



            // Init Character Screen
            UIUtils.AddGamepadButtonPressToButton(characterSelectScreen.nextButton, 0, deviceID);
            characterSelectScreen.nextButton.onClick.AddListener(() =>
            {
                characterSelectScreen.root.SetActive(false);
                teamSelectScreen.root.SetActive(true);
            });

            UIUtils.AddGamepadButtonPressToButton(characterSelectScreen.backButton, 1, deviceID);
            characterSelectScreen.backButton.onClick.AddListener(() =>
            {
                characterSelectScreen.root.SetActive(false);
                signInScreen.root.SetActive(true);
                isSelecting = false;
            });

            characterSelectScreen.assetScroller.Init(characters);
            characterSelectScreen.assetScroller.InitController(deviceID);



            // Init team screen 
            UIUtils.AddGamepadButtonPressToButton(teamSelectScreen.nextButton, 0, deviceID);
            teamSelectScreen.nextButton.onClick.AddListener(() =>
            {
                teamSelectScreen.root.SetActive(false);
                readyScreen.root.SetActive(true);
                isReady = true;
            });

            UIUtils.AddGamepadButtonPressToButton(teamSelectScreen.backButton, 1, deviceID);
            teamSelectScreen.backButton.onClick.AddListener(() =>
            {
                teamSelectScreen.root.SetActive(false);
                characterSelectScreen.root.SetActive(true);
            });

            teamSelectScreen.assetScroller.Init(teams);
            teamSelectScreen.assetScroller.InitController(deviceID);



            // Init ready screen
            UIUtils.AddGamepadButtonPressToButton(readyScreen.backButton, 1, deviceID);
            readyScreen.backButton.onClick.AddListener(() =>
            {
                readyScreen.root.SetActive(false);
                teamSelectScreen.root.SetActive(true);
                isReady = false;
            });

            signInScreen.root.SetActive(true);
            characterSelectScreen.root.SetActive(false);
            teamSelectScreen.root.SetActive(false);
            readyScreen.root.SetActive(false);
        }

        public PlayerData GetPlayerData(){
            return new PlayerData
            {
                character = characters.characters[characterSelectScreen.assetScroller.CurrentIndex()],
                team = teams.teams[teamSelectScreen.assetScroller.CurrentIndex()],
                deviceID = deviceID
            };
        }
    }
}