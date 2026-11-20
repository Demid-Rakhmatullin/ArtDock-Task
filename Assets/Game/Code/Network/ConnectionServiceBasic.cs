using Game.Common;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Network
{
    using ApprovalRequest = NetworkManager.ConnectionApprovalRequest;

    class ConnectionServiceBasic : IConnectionService
    {
        readonly GameNetworkManager _manager;
        readonly GameNetworkConfig _config;
        readonly SystemMessagesService _messagesService;

        bool _gameplayStarted; //todo: try to keep ConnectionService abstracted of gameplay?

        Func<ApprovalRequest, (bool, string)> _outerApprove;
        Func<ApprovalRequest, (bool, string)> IConnectionService.OuterApprove { set => _outerApprove = value; }

        public event Action<ulong> OnConnected;
        public event Action OnDisconnecting;
        public event Action<bool> OnDisconnected;
        public event Action<ulong> OnOtherClientConnected;
        public event Action<ulong> OnOtherClientDisconnected;
        public event Action OnSceneChangeStarted;
        public event Action OnSceneChangeFinished;

        public bool IsHost => _manager.IsHost;

        public ConnectionServiceBasic(GameNetworkManager manager, GameNetworkConfig config,
            SystemMessagesService messagesService)
        {
            _manager = manager;
            _config = config;
            _messagesService = messagesService;
        }

        public Task<bool> StartHost()
        {
            SubscribeBeforeConnectionHost();

            if (_manager.StartHost())
            {
                SubscribeAll();
                SubscribeLobbyHost();
                SubscribeHost();
                _manager.SceneManager.LoadScene(_config.LobbySceneName, LoadSceneMode.Single);

                return Task.FromResult(true);
            }

            UnsubscribeBeforeConnectionHost();
            return Task.FromResult(false);
        }

        public bool StartClient()
        {
            if (_manager.StartClient())
            {
                SubscribeAll();
                SubscribeClient();
                return true;
            }

            return false;
        }

        public void StartGameplay()
        {
            _gameplayStarted = true;
            SubscribeGameplayHost();
            _manager.SceneManager.LoadScene(_config.GameplaySceneName, LoadSceneMode.Single);
        }

        public void Shutdown()
            => _manager.Shutdown();

        private void SubscribeAll()
        {
            _manager.SceneManager.OnSceneEvent += OnSceneEventAll; //auto unsubscribe on disconnect

            _manager.OnConnectionEvent += OnConnectionAll;
            _manager.OnPreShutdown += OnPreShutdownAll;
            _manager.OnClientStopped += OnStoppedAll;
        }

        private void UnsubscribeAll()
        {
            _manager.OnConnectionEvent -= OnConnectionAll;
            _manager.OnPreShutdown -= OnPreShutdownAll;
            _manager.OnClientStopped -= OnStoppedAll;
        }

        private void SubscribeBeforeConnectionHost()
        {
            _manager.ConnectionApprovalCallback = ApproveClientOnHost;
        }

        private void UnsubscribeBeforeConnectionHost()
        {
            _manager.ConnectionApprovalCallback = null;
        }

        private void SubscribeHost()
        {
            _manager.OnConnectionEvent += OnConnectionHost;
            _manager.OnClientStopped += OnStoppedHost;
        }

        private void SubscribeLobbyHost()
        {
            _manager.SceneManager.OnSceneEvent += OnLobbySceneEventHost;
        }

        private void UnsubscribeLobbyHost()
        {
            _manager.SceneManager.OnSceneEvent -= OnLobbySceneEventHost;
        }

        private void SubscribeGameplayHost()
        {
            _manager.SceneManager.OnSceneEvent += OnGameplaySceneEventHost;
        }

        private void UnsubscribeGameplayHost()
        {
            _manager.SceneManager.OnSceneEvent -= OnGameplaySceneEventHost;
        }

        private void UnsubscribeHost()
        {
            if (!_manager)
                return;

            UnsubscribeBeforeConnectionHost();
            _manager.OnConnectionEvent -= OnConnectionHost;
            _manager.OnClientStopped -= OnStoppedHost;
        }

        private void SubscribeClient()
        {
            _manager.OnConnectionEvent += OnConnectionClient;
            _manager.OnClientStopped += OnStoppedClient;
        }

        private void UnsubscribeClient()
        {
            if (!_manager)
                return;

            _manager.OnConnectionEvent -= OnConnectionClient;
            _manager.OnClientStopped -= OnStoppedClient;
        }

        private void OnSceneEventAll(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Load:
                    OnSceneChangeStarted?.Invoke();
                    break;
                case SceneEventType.LoadEventCompleted:
                    OnSceneChangeFinished?.Invoke();
                    break;
            }
        }

        private void OnLobbySceneEventHost(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadEventCompleted:
                    UnsubscribeLobbyHost();
                    OnConnected?.Invoke(sceneEvent.ClientId);
                    break;
            }
        }

        private void OnGameplaySceneEventHost(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadEventCompleted:
                    UnsubscribeGameplayHost();
                    foreach (var clientId in _manager.ConnectedClientsIds)
                        SpawnPlayer(clientId);                    
                    break;
            }
        }

        private void ApproveClientOnHost(ApprovalRequest request, 
            NetworkManager.ConnectionApprovalResponse response)
        {
            if (_manager.ConnectedClientsIds.Count + 1 > _config.MaxPlayersCount)
            {
                UnapproveResponse(response, "Too many players");
                return;
            }

            if (_outerApprove != null)
            {
                (bool result, string message) = _outerApprove(request);
                if (!result)
                {
                    UnapproveResponse(response, message);
                    return;
                }
            }

            response.Approved = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnapproveResponse(NetworkManager.ConnectionApprovalResponse response, 
            string message)
        {
            response.Reason = message;
            response.Approved = false;
        }

        private void OnConnectionAll(NetworkManager manager, ConnectionEventData data)
        {
            switch (data.EventType)
            {
                case ConnectionEvent.PeerConnected:
                    OnOtherClientConnected?.Invoke(data.ClientId);
                    break;
                case ConnectionEvent.PeerDisconnected:
                    OnOtherClientDisconnected?.Invoke(data.ClientId);
                    break;
            }
        }

        private void OnConnectionHost(NetworkManager manager, ConnectionEventData data)
        {
            switch (data.EventType)
            {
                case ConnectionEvent.ClientConnected:
                    if (_gameplayStarted)
                        SpawnPlayer(data.ClientId);
                    break;
            }
        }

        private void OnConnectionClient(NetworkManager manager, ConnectionEventData data)
        {
            switch (data.EventType)
            {
                case ConnectionEvent.ClientConnected:
                    OnConnected?.Invoke(data.ClientId); //on host this event is called later, because at this point first scene isn't initialized yet when starting host
                    break;
                case ConnectionEvent.ClientDisconnected:
                    if (!string.IsNullOrEmpty(_manager.DisconnectReason))
                        _messagesService.DisplayMessage(_manager.DisconnectReason);
                    break;
            }
        }

        private void OnPreShutdownAll()
            => OnDisconnecting?.Invoke();

        private void OnStoppedAll(bool isHost)
        {
            OnDisconnected?.Invoke(isHost);
            UnsubscribeAll();

            if (SceneManager.GetActiveScene().buildIndex != 0)
                SceneManager.LoadScene(0);
        }

        private void OnStoppedHost(bool isHost)
        {
            _gameplayStarted = false;
            UnsubscribeHost();
        }

        private void OnStoppedClient(bool isHost)
        {
            UnsubscribeClient();
        }

        private void SpawnPlayer(ulong clientId)
        {
            var playerGO = UnityEngine.Object.Instantiate(_config.PlayerPrefab);
            playerGO.SpawnAsPlayerObject(clientId);
        }
    }
}
