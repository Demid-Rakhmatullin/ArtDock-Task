using System;
using System.Collections.Generic;

namespace Game.Network
{
    public interface INetworkClientsService
    {
        IReadOnlyList<NetworkClientData> List { get; }

        event Action OnClientsUpdated; //todo: some kind of ReactiveCollection? (UniRx/R3)
    }
}
