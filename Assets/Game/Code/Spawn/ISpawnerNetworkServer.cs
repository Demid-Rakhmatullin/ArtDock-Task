using Game.Network;
using System;

namespace Game.Spawn
{
    public interface ISpawnerNetworkServer
    {
        Action SpawnServer { set; }

        Action DespawnServer { set; }
    }
}
