using Unity.Netcode;
using UnityEngine;
using VContainer.Unity;

namespace Game.Interactions
{
    class GrabServiceServer : GrabServiceBase, IStartable
    {
        //todo: place this in GrabConfig
        //
        const float THROW_FORCE = 10f;

        readonly IGrabNetworkServer _network;
        readonly Collider _grabber;

        public GrabServiceServer(IGrabNetworkServer network, Collider grabber)
        {
            _network = network;
            _grabber = grabber;
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

            if (interactable.isGrabbed &&
                _network.OwnerClientId != grabbingObj.OwnerClientId) //if client RPC somehow is lost repeat server logic
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
            _network.GrabRequestAuthorizedRpc(grabbingObj);
        }

        private void Release(NetworkObject grabbedObj)
            => Release(grabbedObj, out _);

        private bool Release(NetworkObject grabbedObj, out Rigidbody rb)
        {
            rb = null;

            if (_network.OwnerClientId != grabbedObj.OwnerClientId)
                return false;

            if (!grabbedObj.TryGetComponent<InteractableServerData>(out var interactable))
                return false;

            grabbedObj.RemoveOwnership();
            interactable.isGrabbed = false;

            if (grabbedObj.TryGetComponent(out rb))
                rb.isKinematic = false;

            return true;
        }

        private void Throw(NetworkObject grabbedObj)
        {
            if (Release(grabbedObj, out var rb))
            {
                rb.AddForce(
                    grabbedObj.transform.forward * THROW_FORCE,
                    ForceMode.Impulse
                );
            }
        }
    }
}
