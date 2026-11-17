using Game.Network;
using System;
using Unity.Netcode;

namespace Game.Spawn
{
    public interface ISpawnerNetworkServer : INetworkServer
    {
        Action SpawnServer { set; }

        Action DespawnServer { set; }
    }
}
