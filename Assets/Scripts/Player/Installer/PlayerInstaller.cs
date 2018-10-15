using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Swing.Player
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField]
        private CameraSettings settings;

        public override void InstallBindings()
        {
            Container.BindInstance(settings);
        }
    }
}
