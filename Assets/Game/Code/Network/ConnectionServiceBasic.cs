using Game.Common;
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Game.Network
{
    class ConnectionServiceBasic : IConnectionService
    {
        readonly GameNetworkManager _manager;
        readonly GameNetworkConfig _config;
        readonly SystemMessagesService _messagesService;

        public event Action<ulong> OnConnected;
        public event Action OnDisconnecting;
        public event Action<bool> OnDisconnected;
        public event Action<ulong> OnOtherClientConnected;
        public event Action<ulong> OnOtherClientDisconnected;

        public ConnectionServiceBasic(GameNetworkManager manager, GameNetworkConfig config,
            SystemMessagesService messagesService)
        {
            _manager = manager;
            _config = config;
            _messagesService = messagesService;
        }

        public Task<bool> StartHost()
        {
            SubscribeEarlyHost();

            if (_manager.StartHost())
            {
                SubscribeAll();
                SubscribeHost();
                _manager.SceneManager.LoadScene(_config.GameplaySceneName, LoadSceneMode.Single);

                return Task.FromResult(true);
            }

            UnsubscribeEarlyHost();
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

        public void Shutdown()
            => _manager.Shutdown();

        private void SubscribeAll()
        {
            _manager.OnConnectionEvent += OnConnectionAll;
            _manager.OnPreShutdown += OnPreShutdownAll;
            _manager.OnClientStopped += OnClientStoppedAll;
        }

        private void UnsubscribeAll()
        {
            _manager.OnConnectionEvent -= OnConnectionAll;
            _manager.OnPreShutdown -= OnPreShutdownAll;
            _manager.OnClientStopped -= OnClientStoppedAll;
        }

        private void SubscribeEarlyHost()
        {
            _manager.ConnectionApprovalCallback = ApproveClientOnHost;
        }

        private void UnsubscribeEarlyHost()
        {
            _manager.ConnectionApprovalCallback = null;
        }

        private void SubscribeHost()
        {
            //one-time subscriptions
            _manager.SceneManager.OnSceneEvent += OnSceneEventHost;

            _manager.OnConnectionEvent += OnConnectionHost;
        }

        private void UnsubscribeHost()
        {
            if (!_manager)
                return;

            UnsubscribeEarlyHost();
            _manager.OnConnectionEvent -= OnConnectionHost;
        }

        private void SubscribeClient()
        {
            _manager.OnConnectionEvent += OnConnectionClient;
        }

        private void UnsubscribeClient()
        {
            if (!_manager)
                return;

            _manager.OnConnectionEvent -= OnConnectionClient;
        }

        private void OnSceneEventHost(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadEventCompleted:
                    SpawnPlayer(sceneEvent.ClientId);
                    OnConnected?.Invoke(sceneEvent.ClientId);
                    break;
            }
        }

        private void ApproveClientOnHost(NetworkManager.ConnectionApprovalRequest request, 
            NetworkManager.ConnectionApprovalResponse response)
        {
            if (_manager.ConnectedClientsIds.Count + 1 > _config.MaxPlayersCount)
            {
                response.Reason = "Too many players";
                response.Approved = false;
                return;
            }

            response.Approved = true;
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

        private void OnClientStoppedAll(bool isHost)
        {
            OnDisconnected?.Invoke(isHost);

            if (isHost)
                UnsubscribeHost();
            else
                UnsubscribeClient();
            UnsubscribeAll();

            if (SceneManager.GetActiveScene().buildIndex != 0)
                SceneManager.LoadScene(0);
        }

        private void SpawnPlayer(ulong clientId)
        {
            var playerGO = UnityEngine.Object.Instantiate(_config.PlayerPrefab);
            playerGO.SpawnAsPlayerObject(clientId);
        }
    }
}
