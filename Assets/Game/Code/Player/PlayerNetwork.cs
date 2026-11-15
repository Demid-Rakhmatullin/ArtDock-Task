using Unity.Netcode;
using UnityEngine;

namespace Game.Player
{
    class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField] PlayerScope _scope;

        protected override void OnNetworkPostSpawn()
        {
            if (IsOwner)
                _scope.Build();
        }

        public override void OnNetworkPreDespawn()
        {
            if (IsOwner)
                _scope.DisposeCore();
        }
    }
}
