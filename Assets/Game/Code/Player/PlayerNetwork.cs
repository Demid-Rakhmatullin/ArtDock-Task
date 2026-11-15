using Unity.Netcode;
using UnityEngine;
using VContainer.Unity;

namespace Game.Player
{
    class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField] LifetimeScope _playerScope;

        protected override void OnNetworkPostSpawn()
        {
            if (IsOwner)
                _playerScope.Build();
        }

        public override void OnNetworkPreDespawn()
        {
            if (IsOwner)
                _playerScope.DisposeCore();
        }
    }
}
