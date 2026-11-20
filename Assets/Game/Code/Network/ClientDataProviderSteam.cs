using Steamworks;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Game.Network
{
    class ClientDataProviderSteam : IClientDataProvider
    {
        readonly SharedDataSteam _dataPrefab;
        readonly GameNetworkConfig _config;

        SharedDataSteam _steamData;

        public ClientDataProviderSteam(SharedDataSteam dataPrefab, GameNetworkConfig config)
        {
            _dataPrefab = dataPrefab;
            _config = config;
        }

        public void InitializeHost()
        {
            _steamData = Object.Instantiate(_dataPrefab);
            _steamData.ClientToSteamMap = new NetworkVariable<Dictionary<ulong, ulong>>(
                new Dictionary<ulong, ulong>(_config.MaxPlayersCount));

            var netwObj = _steamData.GetComponent<NetworkObject>();
            netwObj.Spawn(false);
            
            _steamData.ClientToSteamMap.Initialize(_steamData);
        }

        public void InitializeClient()
        {
            _steamData = Object.FindAnyObjectByType<SharedDataSteam>();
        }

        public void AddClient(ulong clientId, ulong steamId)
        {
            _steamData.ClientToSteamMap.Value.Add(clientId, steamId);
            _steamData.ClientToSteamMap.CheckDirtyState();
        }

        public void RemoveClient(ulong clientId)
        {
            _steamData.ClientToSteamMap.Value.Remove(clientId);
            _steamData.ClientToSteamMap.CheckDirtyState();
        }

        public async Awaitable UpdateData(NetworkClientData data)
        {
            if (!_steamData.ClientToSteamMap.Value.TryGetValue(data.networkId, out ulong steamId))
                return;
            data.SteamId = steamId;

            Friend friend = new(steamId);
            await friend.RequestInfoAsync();

            data.Name = friend.Name;
        }
    }
}
