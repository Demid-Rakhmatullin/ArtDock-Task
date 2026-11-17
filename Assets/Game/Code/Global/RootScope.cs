using Game.Network;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Game.Global
{
    [DisallowMultipleComponent]
    class RootScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(FindAnyObjectByType<NetworkManager>());
            builder.RegisterComponent(FindAnyObjectByType<GameNetworkManager>());
            builder.RegisterInstance(InputSystem.actions); //project-wide input asset
        }
    }
}
