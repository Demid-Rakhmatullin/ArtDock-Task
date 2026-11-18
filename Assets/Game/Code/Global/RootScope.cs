using Game.Network;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Game.Global
{
    [DisallowMultipleComponent]
    class RootScope : LifetimeScope
    {
        [SerializeField] GameNetworkConfig networkConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(InputSystem.actions); //project-wide input asset

            var networkManager = FindAnyObjectByType<GameNetworkManager>();
            networkManager.Install(builder, networkConfig);
        }
    }
}
