using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Network
{
    class NetworkClientsService : INetworkClientsService
    {
        readonly IConnectionService _connection;
        readonly GameNetworkManager _networkManager;
        readonly IClientDataProvider _dataProvider;
        readonly List<NetworkClientData> _clients;
        readonly Dictionary<ulong, int> _clientsIndexes;

        public IReadOnlyList<NetworkClientData> List => _clients;

        public event Action OnClientsUpdated;

        public NetworkClientsService(IConnectionService connection, GameNetworkManager networkManager,
            GameNetworkConfig config, IClientDataProvider dataProvider)
        {
            _connection = connection;
            _networkManager = networkManager;
            _dataProvider = dataProvider;
            _clients = new List<NetworkClientData>(config.MaxPlayersCount);
            _clientsIndexes = new Dictionary<ulong, int>(config.MaxPlayersCount);

            //consider network services has the same lifetime
            _connection.OnConnected += OnConnected;
            _connection.OnOtherClientConnected += OnOtherConnected;
            _connection.OnOtherClientDisconnected += OnOtherDisconnected;
            _connection.OnDisconnected += OnDisconnected;
        }

        private async void OnConnected(ulong clientId)
        {
            foreach (ulong id in _networkManager.ConnectedClientsIds)
                await AddClientData(id);
        }

        private async void OnOtherConnected(ulong clientId)
        {
            await AddClientData(clientId);
            OnClientsUpdated?.Invoke();
        }

        private void OnOtherDisconnected(ulong clientId)
        {
            _clients.RemoveAt(_clientsIndexes[clientId]);
            _clientsIndexes.Remove(clientId);
            OnClientsUpdated?.Invoke();
        }

        private void OnDisconnected(bool isHost)
        {
            _clients.Clear();
            _clientsIndexes.Clear();
        }

        private async Awaitable AddClientData(ulong clientId)
        {
            //todo: Object pooling?
            var clientData = new NetworkClientData(clientId);

            await _dataProvider.UpdateData(clientData);

            _clientsIndexes.Add(clientId, _clients.Count);
            _clients.Add(clientData);
        }
    }
}
