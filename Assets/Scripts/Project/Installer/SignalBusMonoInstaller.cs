using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Swing
{
    public class SignalBusMonoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
        }
    }
}