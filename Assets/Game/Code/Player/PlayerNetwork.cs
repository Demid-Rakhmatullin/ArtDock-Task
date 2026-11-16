using Game.Interactions;
using System;
using Unity.Netcode;
using UnityEngine;
using VContainer.Unity;

namespace Game.Player
{
    [DisallowMultipleComponent]
    class PlayerNetwork : NetworkBehaviour, IGrabNetworkClient, IGrabNetworkServer
    {
        [SerializeField] LifetimeScope _clientScope;
        [SerializeField] LifetimeScope _serverScope;

        protected override void OnNetworkPostSpawn()
        {
            if (IsOwner)
                _clientScope.Build();

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

        public Action<NetworkObject> GrabReleaseServer { private get; set; }

        public Action<NetworkObject> GrabThrowServer { private get; set; }

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
        public void GrabReleaseRpc(NetworkObjectReference grabbedRef)
        {
            if (grabbedRef.TryGet(out NetworkObject grabbedObj))
                GrabReleaseServer?.Invoke(grabbedObj);
        }

        [Rpc(SendTo.Server)]
        public void GrabThrowRpc(NetworkObjectReference grabbedRef)
        {
            if (grabbedRef.TryGet(out NetworkObject grabbedObj))
                GrabThrowServer?.Invoke(grabbedObj);
        }

        #endregion
    }
}
