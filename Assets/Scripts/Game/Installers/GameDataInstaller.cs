using System.Collections;
using System.Collections.Generic;
using Swing.Character;
using Swing.Level;
using Swing.Player;
using Swing.Sound;
using UnityEngine;
using Zenject;

namespace Swing.Game.Soccer
{
    public class GameDataInstaller : MonoInstaller
    {
        [SerializeField] private CharacterCollection characters;
        [SerializeField] private TeamsData teams;
        [SerializeField] private GameTimeOptions timeOptions;
        [SerializeField] private SoundAssets soundAssets;
        [SerializeField] private LevelCollection[] levels;

        public override void InstallBindings()
        {
            Container.BindInstance(levels);
            Container.BindInstance(characters);
            Container.BindInstance(teams);
            Container.BindInstance(timeOptions);
            Container.BindInstance(soundAssets);
        }
    }
}