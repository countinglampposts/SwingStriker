using System.Collections;
using System.Collections.Generic;
using Swing.Character;
using Swing.Level;
using Swing.Player;
using Swing.Sound;
using UnityEngine;
using Zenject;

namespace Swing.Game
{
    public class GameDataInstaller : MonoInstaller
    {
        [SerializeField] private LevelCollection levels;
        [SerializeField] private CharacterCollection characters;
        [SerializeField] private TeamsData teams;
        [SerializeField] private SplitScreenLayouts layouts;
        [SerializeField] private GameTimeOptions timeOptions;
        [SerializeField] private SoundAssets soundAssets;

        public override void InstallBindings()
        {
            Container.BindInstance(levels);
            Container.BindInstance(characters);
            Container.BindInstance(teams);
            Container.BindInstance(layouts);
            Container.BindInstance(timeOptions);
            Container.BindInstance(soundAssets);
        }
    }
}