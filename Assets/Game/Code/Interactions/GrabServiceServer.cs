using System;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Interactions
{
    class GrabServiceServer : GrabServiceBase, IStartable, IDisposable
    {
        public static void Install(IContainerBuilder builder, Lifetime lifetime)
        {
            builder.Register<GrabDataServer>(lifetime);
            builder.RegisterEntryPoint<GrabServiceServer>(lifetime);
        }

        //todo: place this in GrabConfig
        //
        const float THROW_FORCE = 10f;

        readonly GrabDataServer _data;
        readonly IGrabNetworkServer _network;
        readonly Collider _grabber;

        public GrabServiceServer(IGrabNetworkServer network, Collider grabber, 
            GrabDataServer data)
        {
            _network = network;
            _grabber = grabber;
            _data = data;
        }

        void IStartable.Start()
        {
            //consider that GrabServerService is singletone in IGrabNetworkServer scope 
            _network.GrabServer = Grab;
            _network.GrabReleaseServer = Release;
            _network.GrabThrowServer = Throw;
        }

        private void Grab(NetworkObject grabbingObj)
        {
            if (!grabbingObj.TryGetComponent<InteractableServerData>(out var interactable) ||
                !grabbingObj.TryGetComponent<Collider>(out var grabbingCollider))
                return;

            if (interactable.isGrabbed || _data.grabbed != null)
                return;

            float castRadius = GetCastRadius(_grabber.bounds);
            float grabbingSize = new Vector2(
                    grabbingCollider.bounds.extents.x,
                    grabbingCollider.bounds.extents.z)
                .magnitude;

            if (Vector3.Distance(_grabber.transform.position, grabbingObj.transform.position)
                > GetCastDistance(castRadius) + castRadius + grabbingSize)
            {
                return;
            }

            grabbingObj.ChangeOwnership(_network.OwnerClientId);
            interactable.isGrabbed = true;
            _data.grabbed = grabbingObj;
            _network.GrabRequestAuthorizedRpc(grabbingObj);
        }

        private void Release()
            => Release(out _, out _);

        private bool Release(out NetworkObject released, out Rigidbody rb)
        {
            released = _data.grabbed;
            rb = null;

            if (!_data.grabbed)
                return false;

            if (_data.grabbed.TryGetComponent(out rb))
                rb.isKinematic = false;

            if (_data.grabbed.IsSpawned)
                _data.grabbed.RemoveOwnership();

            _data.grabbed.GetComponent<InteractableServerData>().isGrabbed = false;
            _data.grabbed = null;

            return true;
        }

        private void Throw()
        {
            if (Release(out var released, out var rb))
            {
                rb.AddForce(
                    released.transform.forward * THROW_FORCE,
                    ForceMode.Impulse
                );
            }
        }

        void IDisposable.Dispose()
            => Release();
    }
}
