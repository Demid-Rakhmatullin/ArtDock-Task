using Game.Network;
using System;
using Unity.Netcode;

namespace Game.Interactions
{
    public interface IGrabNetworkServer : IServerBehaviour
    {
        Action<NetworkObject> GrabServer { set; }

        Action GrabReleaseServer { set; }

        Action GrabThrowServer { set; }

        void GrabRequestAuthorizedRpc(NetworkObjectReference grabbingRef);
    }
}
