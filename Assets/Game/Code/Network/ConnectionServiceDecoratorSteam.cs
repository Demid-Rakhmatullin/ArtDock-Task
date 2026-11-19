using Game.Common;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using VContainer.Unity;

namespace Game.Network
{
    class ConnectionServiceDecoratorSteam : IConnectionService, IAsyncStartable, ITickable
    {
        readonly IConnectionService _inner;
        readonly GameNetworkConfig _config;
        readonly SystemMessagesService _messagesService;
        readonly FacepunchTransport _transport;

        Lobby? _currentLobby;
        bool _waitingInvite;
        float _invitePingTimer;

        public event Action<ulong> OnConnected;
        public event Action OnDisconnecting;
        public event Action<bool> OnDisconnected;
        public event Action<ulong> OnOtherClientConnected;
        public event Action<ulong> OnOtherClientDisconnected;

        public ConnectionServiceDecoratorSteam(IConnectionService inner, GameNetworkConfig config,
            SystemMessagesService messagesService, NetworkTransport transport)
        {
            _inner = inner;
            _config = config;
            _messagesService = messagesService;
            _transport = (FacepunchTransport)transport;
        }

        async Awaitable IAsyncStartable.StartAsync(CancellationToken cancellation)
        {
            //consider decorator has the same lifetime as decorated instance
            _inner.OnDisconnecting += OnDisconnectingHandler;
            _inner.OnDisconnected += OnDisconnectedHandler;
            _inner.OnConnected += OnConnectedHandler;
            _inner.OnOtherClientConnected += OnOtherClientConnectedHandler;
            _inner.OnOtherClientDisconnected += OnOtherClientDisconnectedHandler;

            await StartWaitingInvite();
        }

        void ITickable.Tick()
        {
            if (_waitingInvite)
            {
                _invitePingTimer += Time.deltaTime;
                if (_invitePingTimer > _config.SteamInvitePingSeconds)
                {
                    SteamClient.RunCallbacks();
                    _invitePingTimer = 0f;
                }
            }
        }

        private async Awaitable StartWaitingInvite()
        {
            await Awaitable.WaitForSecondsAsync(_config.SteamInvitePingSeconds);
            _transport.Initialize();

            if (!SteamClient.IsValid)
            {
                Debug.LogError("Can't start Steam invitaion check. SteamClient.IsValid == false.");
                _messagesService.DisplayMessage("Can't connect to Steam services. Please check that Steam is started.");
                return;
            }

            _waitingInvite = true;
            SteamFriends.OnGameLobbyJoinRequested += OnLobbyJoinRequestedClient;
        }

        private void StopWaitingInvite()
        {
            _waitingInvite = false;
            _invitePingTimer = 0f;
            SteamFriends.OnGameLobbyJoinRequested -= OnLobbyJoinRequestedClient;
        }

        public async Task<bool> StartHost()
        {
            if (SteamClient.IsValid) 
                _transport.Shutdown(); //clean up transport before starting host

            bool innerResult = await _inner.StartHost();      

            if (!innerResult || !SteamClient.IsValid)
            {
                Debug.LogError("Can't start Steam host. Transport didn't start or SteamClient.IsValid == false.");
                _messagesService.DisplayMessage("Can't connect to Steam services. Please check that Steam is started.");
                return false;
            }

            StopWaitingInvite();
            SubscribeAll();
            SubscribeHost();
            _currentLobby = await SteamMatchmaking.CreateLobbyAsync(_config.MaxPlayersCount);

            return true;
        }

        public bool StartClient()
        {
            SteamFriends.OpenOverlay("friends"); //just show overlay, joining implemented only via Steam UI

            return true;
        }

        public void Shutdown()
            => _inner.Shutdown();

        public void SubscribeAll()
        {   }

        public void UnsubscribeAll()
        {   }

        private void SubscribeHost()
        {
            SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedHost;
        }

        private void UnsubscribeHost()
        {
            SteamMatchmaking.OnLobbyCreated -= OnLobbyCreatedHost;
        }

        private void SubscribeClient()
        {   }

        private void UnsubscribeClient()
        {   }

        private void OnLobbyCreatedHost(Result result, Lobby lobby)
        {
            if (result != Result.OK)
            {
                Debug.LogError($"Lobby couldn't be created!, {result}");
                _messagesService.DisplayMessage("Can't create Steam lobby. Please check that your Steam account is capable to do that.");
                return;
            }

            lobby.SetFriendsOnly();
            lobby.SetData("name", "ArtDock Test Lobby");
            lobby.SetJoinable(true);
        }

        private async void OnLobbyJoinRequestedClient(Lobby lobby, SteamId id)
        {
            _transport.targetSteamId = id;
            bool innerResult = _inner.StartClient();            

            if (!innerResult || !SteamClient.IsValid)
            {
                Debug.LogError($"Can't start Steam connection after accepting invite. Transport didn't start or SteamClient.IsValid == false.");
                _messagesService.DisplayMessage("Can't connect to Steam host. Please check that your friend's host PC has stable connection.");
                return;
            }

            StopWaitingInvite();
            SubscribeAll();
            SubscribeClient();
            _currentLobby = await SteamMatchmaking.JoinLobbyAsync(lobby.Id);
        }

        private void OnDisconnectingHandler()
        {
            OnDisconnecting?.Invoke();
            
            if (SteamClient.IsValid)
                _currentLobby?.Leave();
            _currentLobby = null;
        }

        private async void OnDisconnectedHandler(bool isHost)
        {
            OnDisconnected?.Invoke(isHost);

            if (isHost)
                UnsubscribeHost();
            else
                UnsubscribeClient();
            UnsubscribeAll();

            await StartWaitingInvite();
        }

        private void OnConnectedHandler(ulong clientId) => OnConnected?.Invoke(clientId);
        private void OnOtherClientConnectedHandler(ulong clientId) => OnOtherClientConnected?.Invoke(clientId);
        private void OnOtherClientDisconnectedHandler(ulong clientId) => OnOtherClientDisconnected?.Invoke(clientId);
    }
}
