using Game.Common;
using System;
using Unity.Netcode;

namespace Game.Interactions
{
    public interface IGrabNetworkServer : INetworkServer
    {        
        Action <NetworkObject> GrabServer { set; }

        Action<NetworkObject> GrabReleaseServer { set; }

        Action<NetworkObject> GrabThrowServer { set; }

        void GrabRequestAuthorizedRpc(NetworkObjectReference grabbingRef);
    }
}
