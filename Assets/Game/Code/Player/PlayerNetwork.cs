using Game.Gameplay;
using Game.Interactions;
using Game.Spawn;
using System;
using Unity.Netcode;
using UnityEngine;
using VContainer.Unity;

namespace Game.Player
{
    [DisallowMultipleComponent]
    class PlayerNetwork : NetworkBehaviour, 
        IGrabNetworkClient, IGrabNetworkServer,
        ISpawnerNetworkClient, ISpawnerNetworkServer
    {
        [SerializeField] LifetimeScope _clientScope;
        [SerializeField] LifetimeScope _serverScope;

        protected override void OnNetworkPostSpawn()
        {
            if (IsOwner)
            {
                var gameplayScope = FindAnyObjectByType<GameplayScope>(); //todo: use LifetimeScope class instead of concrete implementation
                using (LifetimeScope.EnqueueParent(gameplayScope))
                {
                    _clientScope.Build();
                }
            }

            if (IsServer)
                _serverScope.Build();
        }

        public override void OnNetworkPreDespawn()
        {
            if (IsOwner)
                _clientScope.DisposeCore();

            if (IsServer)
                _serverScope.DisposeCore();
        }
        
        #region IGrabNetwork

        public Action<GameObject> GrabRequestAuthorized { private get; set; }

        public Action<NetworkObject> GrabServer { private get; set; }

        public Action GrabReleaseServer { private get; set; }

        public Action GrabThrowServer { private get; set; }

        [Rpc(SendTo.Server)]
        public void GrabRequestRpc(NetworkObjectReference grabbingRef)
        {
            if (grabbingRef.TryGet(out NetworkObject grabbingObj))
                GrabServer?.Invoke(grabbingObj); //invokes GrabRequestAuthorizedRpc if success
        }
       
        [Rpc(SendTo.Owner)]
        public void GrabRequestAuthorizedRpc(NetworkObjectReference grabbedRef)
        {
            if (grabbedRef.TryGet(out NetworkObject grabbedObj))
                GrabRequestAuthorized?.Invoke(grabbedObj.gameObject);
        }

        [Rpc(SendTo.Server)]
        public void GrabReleaseRpc()
        {
            GrabReleaseServer?.Invoke();
        }

        [Rpc(SendTo.Server)]
        public void GrabThrowRpc()
        {
            GrabThrowServer?.Invoke();
        }

        #endregion

        #region ISpawnerNetwork

        public Action SpawnServer { private get; set; }
        public Action DespawnServer { private get; set; }

        [Rpc(SendTo.Server)]
        public void SpawnRpc()
        {
            SpawnServer?.Invoke();
        }

        [Rpc(SendTo.Server)]
        public void DespawnRpc()
        {
            DespawnServer?.Invoke();
        }

        #endregion
    }
}
