using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Game.Network
{
    [DisallowMultipleComponent]
    class SharedDataSteam : NetworkBehaviour
    {
        public NetworkVariable<Dictionary<ulong, ulong>> ClientToSteamMap;
    }
}
