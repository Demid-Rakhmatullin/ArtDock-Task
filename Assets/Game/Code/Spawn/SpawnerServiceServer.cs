using Game.Interactions;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using static Game.Spawn.SpawnKeys;

namespace Game.Spawn
{
    //todo: Object Pooling
    class SpawnerServiceServer : IStartable
    {
        readonly SpawnerConfig _config;
        readonly ISpawnerNetworkServer _network;
        readonly Transform _spawnPoint;
        readonly GrabDataServer _grabData;

        public SpawnerServiceServer(
            SpawnerConfig config, 
            ISpawnerNetworkServer network,
            [Key(SPAWN_POINT)] Transform spawnPoint,
            IObjectResolver resolver)
        {
            _config = config;
            _network = network;
            _spawnPoint = spawnPoint;
            resolver.TryResolve(out _grabData); //designed as optional dependency for this service
        }

        void IStartable.Start()
        {
            //consider that GrabServerService is singletone in IGrabNetworkServer scope 
            _network.SpawnServer = Spawn;
            _network.DespawnServer = Despawn;
        }

        private void Spawn()
        {
            if (_config.NetworkPrefabs.Length == 0)
                return;

            int rand = Random.Range(0, _config.NetworkPrefabs.Length);
            var randomPrefab = _config.NetworkPrefabs[rand];

            var instance = Object.Instantiate(
                randomPrefab, 
                _spawnPoint.position, 
                randomPrefab.transform.rotation);

            instance.Spawn();
        }

        private void Despawn()
        {
            if (_grabData == null || _grabData.grabbed == null)
                return;

            var grabbed = _grabData.grabbed;
            grabbed.RemoveOwnership();
            grabbed.Despawn(!grabbed.InScenePlaced);
            _grabData.grabbed = null;
        }
    }
}
