using System;
using Unity.Netcode;
using UnityEngine;

namespace Game.Interactions
{
    public interface IGrabNetworkClient
    {
        Action<GameObject> GrabRequestAuthorized { set; }

        void GrabRequestRpc(NetworkObjectReference grabbingObj);

        void GrabReleaseRpc();

        void GrabThrowRpc();
    }
}
