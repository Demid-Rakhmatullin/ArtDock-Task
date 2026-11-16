using System;
using Unity.Netcode;
using UnityEngine;

namespace Game.Interactions
{
    public interface IGrabNetworkClient
    {
        Action<GameObject> GrabRequestAuthorized { set; }

        void GrabRequestRpc(NetworkObjectReference grabbingObj);

        void GrabReleaseRpc(NetworkObjectReference grabbingRef);

        void GrabThrowRpc(NetworkObjectReference grabbedRef);
    }
}
