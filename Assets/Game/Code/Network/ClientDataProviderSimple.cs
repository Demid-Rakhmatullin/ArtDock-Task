using UnityEngine;

namespace Game.Network
{
    class ClientDataProviderSimple : IClientDataProvider
    {
        public Awaitable UpdateData(NetworkClientData data)
        {
            data.Name = $"Player {data.networkId + 1}";

            //todo: unnecessary allocation?
            var source = new AwaitableCompletionSource();
            source.SetResult();
            return source.Awaitable;
        }
    }
}
