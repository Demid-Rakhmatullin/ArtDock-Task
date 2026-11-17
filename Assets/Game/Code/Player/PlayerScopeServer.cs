using Game.Interactions;
using Game.Spawn;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using static Game.Spawn.SpawnKeys;

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
                .As<IGrabNetworkServer, ISpawnerNetworkServer>();
            builder.RegisterComponent(_object.SpawnPosition)
                .Keyed(SPAWN_POINT);
            builder.RegisterInstance(_object.SpawnerConfig);

            builder.RegisterEntryPoint<GrabServiceServer>(Lifetime.Singleton);
            builder.RegisterEntryPoint<SpawnerServiceServer>(Lifetime.Singleton);
        }
    }
}
