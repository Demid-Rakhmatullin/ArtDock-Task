using System;
using Unity.Netcode;

namespace Game.Interactions
{
    public interface IGrabNetworkServer
    {
        ulong OwnerClientId { get; }

        Action <NetworkObject> GrabServer { set; }

        Action<NetworkObject> GrabReleaseServer { set; }

        Action<NetworkObject> GrabThrowServer { set; }

        void GrabRequestAuthorizedRpc(NetworkObjectReference grabbingRef);
    }
}
