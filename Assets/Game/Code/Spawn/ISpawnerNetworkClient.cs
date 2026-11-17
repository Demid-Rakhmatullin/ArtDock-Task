using Unity.Netcode;

namespace Game.Spawn
{
    public interface ISpawnerNetworkClient
    {
        void SpawnRpc();

        void DespawnRpc(NetworkObjectReference despawningRef);
    }
}
