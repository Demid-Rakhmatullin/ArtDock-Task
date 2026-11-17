using Game.Interactions;
using VContainer;
using VContainer.Unity;

namespace Game.Spawn
{
    class SpawnerServiceClient : ITickable
    {
        readonly ISpawnDispatcher _dispatcher;
        readonly ISpawnerNetworkClient _network;
        readonly GrabData _grabData;

        public SpawnerServiceClient(ISpawnDispatcher dipatcher, ISpawnerNetworkClient network, 
            IObjectResolver resolver)
        {
            _dispatcher = dipatcher;
            _network = network;
            resolver.TryResolve(out _grabData); //designed as optional dependency for this service
        }

        void ITickable.Tick()
        {
            if (_dispatcher.Spawn)
                _network.SpawnRpc();

            if (_dispatcher.Despawn &&
                _grabData != null &&
                _grabData.grabbed != null)
            {
                _network.DespawnRpc(_grabData.grabbed.gameObject);
                _grabData.grabbed = null;
            }
        }
    }
}
