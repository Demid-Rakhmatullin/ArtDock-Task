using Game.Interactions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Player
{
    [DisallowMultipleComponent]
    class PlayerScopeServer : LifetimeScope
    {
        [SerializeField] PlayerObject _object;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_object.Controller)
                .As<Collider>();
            builder.RegisterComponent(_object.Network)
                .As<IGrabNetworkServer>();

            builder.RegisterEntryPoint<GrabServiceServer>(Lifetime.Singleton);
        }
    }
}
