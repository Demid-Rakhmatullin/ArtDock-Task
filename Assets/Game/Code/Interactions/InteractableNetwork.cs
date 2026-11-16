using Unity.Netcode;
using UnityEngine;

namespace Game.Interactions
{
    [DisallowMultipleComponent]
    class InteractableNetwork : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (IsServer && !gameObject.TryGetComponent<InteractableServerData>(out _))
                gameObject.AddComponent<InteractableServerData>();
        }
    }
}
