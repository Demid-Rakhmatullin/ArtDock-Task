using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using VContainer.Unity;

namespace Game.Network
{
    class ConnectionServiceDecoratorSteam : IConnectionService, IStartable, ITickable
    {
        readonly IConnectionService _inner;
        readonly GameNetworkConfig _config;
        readonly FacepunchTransport _transport;

        Lobby? _currentLobby;
        bool _waitingInvite;
        float _invitePingTimer;

        public event Action OnDisconnecting;
        public event Action<bool> OnDisconnected;

        public ConnectionServiceDecoratorSteam(IConnectionService inner, GameNetworkConfig config, 
            NetworkTransport transport)
        {
            _inner = inner;
            _config = config;
            _transport = (FacepunchTransport)transport;
        }

        void IStartable.Start()
        {
            StartWaitingInvite();
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

        private void StartWaitingInvite()
        {
            _transport.Initialize();

            if (!SteamClient.IsValid)
            {
                Debug.LogError($"Can't start Steam invitaion check. SteamClient.IsValid == false."); //todo: display message
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
                //todo: display message
                Debug.LogError($"Can't start Steam host. Transport didn't start or SteamClient.IsValid == false.");
                return false;
            }

            StopWaitingInvite();
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

        private void SubscribeHost()
        {
            _inner.OnDisconnecting += OnDisconnectingHandler;
            _inner.OnDisconnected += OnDisconnectedHandler;

            SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedHost;
        }

        private void SubscribeClient()
        {
            _inner.OnDisconnecting += OnDisconnectingHandler;
            _inner.OnDisconnected += OnDisconnectedHandler;
        }

        private void UnsubscribeHost()
        {
            if (_inner != null)
            {
                _inner.OnDisconnecting -= OnDisconnectingHandler;
                _inner.OnDisconnected -= OnDisconnectedHandler;
            }

            SteamMatchmaking.OnLobbyCreated -= OnLobbyCreatedHost;
        }

        private void UnsubscribeClient()
        {
            if (_inner != null)
            {
                _inner.OnDisconnecting -= OnDisconnectingHandler;
                _inner.OnDisconnected -= OnDisconnectedHandler;
            }
        }

        private void OnLobbyCreatedHost(Result result, Lobby lobby)
        {
            if (result != Result.OK)
            {
                Debug.LogError($"Lobby couldn't be created!, {result}"); //todo: display message
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
                Debug.LogError($"Can't start Steam connection after. Transport didn't start or SteamClient.IsValid == false."); //todo: display message
                return;
            }

            StopWaitingInvite();
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

        private void OnDisconnectedHandler(bool isHost)
        {
            OnDisconnected?.Invoke(isHost);

            if (isHost)
                UnsubscribeHost();
            else
                UnsubscribeClient();

            StartWaitingInvite();
        }
    }
}
