﻿using System.Collections;
using System.Collections.Generic;
using Swing.Player;
using UnityEngine;
using Zenject;

namespace Swing.Character
{
    public class CharacterInstaller : MonoInstaller
    {
        [SerializeField]
        private Camera playerCamera;
        [SerializeField]
        private BodyRoot root;
        [SerializeField]
        private CharacterSettings settings;

        public override void InstallBindings()
        {
            Container.DeclareSignal<GrapplingFiredSignal>();
            Container.DeclareSignal<GrapplingReleasedSignal>();
            Container.DeclareSignal<SuicideSignal>();

            Container.BindInstance(settings).AsSingle();

            Container.Bind<Camera>().FromInstance(playerCamera);
            Container.Bind<BodyRoot>().FromInstance(root);
        }
    }
}