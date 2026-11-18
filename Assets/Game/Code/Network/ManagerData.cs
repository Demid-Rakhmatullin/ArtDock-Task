using Unity.Netcode;
using UnityEngine;

namespace Game.Network
{
    class ManagerData : MonoBehaviour
    {
        [field: SerializeField] 
        public NetworkTransport UnityTransport { get; private set; }

        [field: SerializeField] 
        public NetworkTransport SteamTransport { get; private set; }
    }
}
