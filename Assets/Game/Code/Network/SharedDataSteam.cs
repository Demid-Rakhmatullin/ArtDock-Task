using System.Collections.Generic;
using Unity.Netcode;

namespace Game.Network
{
    class SharedDataSteam : NetworkBehaviour
    {
        public NetworkVariable<Dictionary<ulong, ulong>> ClientToSteamMap;
    }
}
