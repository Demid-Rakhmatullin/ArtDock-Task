using Game.Interactions;
using Unity.Netcode;
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

        public SpawnerServiceServer(
            SpawnerConfig config, 
            ISpawnerNetworkServer network,
            [Key(SPAWN_POINT)] Transform spawnPoint)
        {
            _config = config;
            _network = network;
            _spawnPoint = spawnPoint;
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

        private void Despawn(NetworkObject despawningObj)
        {
            if (despawningObj.OwnerClientId != _network.OwnerClientId)
                return;

            if (!despawningObj.TryGetComponent<InteractableServerData>(out var interactable) ||
                !interactable.isGrabbed)
                return;

            despawningObj.RemoveOwnership();
            despawningObj.Despawn(!despawningObj.InScenePlaced);
        }
    }
}
