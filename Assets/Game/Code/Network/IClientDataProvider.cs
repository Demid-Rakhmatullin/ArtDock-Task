using UnityEngine;

namespace Game.Network
{
    interface IClientDataProvider
    {
        Awaitable UpdateData(NetworkClientData data);
    }
}
