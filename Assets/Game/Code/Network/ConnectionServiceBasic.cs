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

        public event Action OnDisconnecting;
        public event Action<bool> OnDisconnected;

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
                SubscribeClient();
                return true;
            }

            return false;
        }

        public void Shutdown()
            => _manager.Shutdown();

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

            _manager.OnClientConnectedCallback += OnClientConnectedHost;
            _manager.OnPreShutdown += OnPreShutdown;
            _manager.OnClientStopped += OnClientStopped;
        }

        private void UnsubscribeHost()
        {
            if (!_manager)
                return;

            UnsubscribeEarlyHost();
            _manager.OnClientConnectedCallback -= OnClientConnectedHost;
            _manager.OnPreShutdown -= OnPreShutdown;
            _manager.OnClientStopped -= OnClientStopped;
        }

        private void SubscribeClient()
        {
            _manager.OnClientDisconnectCallback += OnDisconnectClient;
            _manager.OnPreShutdown += OnPreShutdown;
            _manager.OnClientStopped += OnClientStopped;
        }

        private void UnsubscribeClient()
        {
            if (!_manager)
                return;

            _manager.OnClientDisconnectCallback -= OnDisconnectClient;
            _manager.OnPreShutdown -= OnPreShutdown;
            _manager.OnClientStopped -= OnClientStopped;
        }

        private void OnSceneEventHost(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadEventCompleted:
                    SpawnPlayer(sceneEvent.ClientId);
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

        private void OnClientConnectedHost(ulong clientId)
        {
            SpawnPlayer(clientId);
        }

        private void SpawnPlayer(ulong clientId)
        {
            var playerGO = UnityEngine.Object.Instantiate(_config.PlayerPrefab);
            playerGO.SpawnAsPlayerObject(clientId);
        }

        private void OnDisconnectClient(ulong obj)
        {
            if (!string.IsNullOrEmpty(_manager.DisconnectReason))
                _messagesService.DisplayMessage(_manager.DisconnectReason);
        }

        private void OnPreShutdown()
            => OnDisconnecting?.Invoke();

        private void OnClientStopped(bool isHost)
        {
            OnDisconnected?.Invoke(isHost);

            if (isHost)
                UnsubscribeHost();
            else
                UnsubscribeClient();

            if (SceneManager.GetActiveScene().buildIndex != 0)
                SceneManager.LoadScene(0);
        }
    }
}
