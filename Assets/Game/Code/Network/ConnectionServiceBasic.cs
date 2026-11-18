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

        public event Action OnDisconnecting;
        public event Action<bool> OnDisconnected;

        public ConnectionServiceBasic(GameNetworkManager manager, GameNetworkConfig config)
        {
            _manager = manager;
            _config = config;
        }

        public Task<bool> StartHost()
        {
            if (_manager.StartHost())
            {
                SubscribeHost();
                _manager.SceneManager.LoadScene(_config.GameplaySceneName, LoadSceneMode.Single);

                return Task.FromResult(true);
            }

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

        private void SubscribeHost()
        {
            //one-time subscriptions
            _manager.SceneManager.OnSceneEvent += OnSceneEventHost;

            _manager.OnClientConnectedCallback += OnClientConnectedHost;
            _manager.OnPreShutdown += OnPreShutdown;
            _manager.OnClientStopped += OnClientStopped;
        }

        private void SubscribeClient()
        {
            _manager.OnPreShutdown += OnPreShutdown;
            _manager.OnClientStopped += OnClientStopped;
        }

        private void UnsubscribeHost()
        {
            if (!_manager)
                return;

            _manager.OnClientConnectedCallback -= OnClientConnectedHost;
            _manager.OnPreShutdown -= OnPreShutdown;
            _manager.OnClientStopped -= OnClientStopped;     
        }

        private void UnsubscribeClient()
        {
            if (!_manager)
                return;

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

        private void OnClientConnectedHost(ulong clientId)
        {
            if (_manager.ConnectedClientsIds.Count > _config.MaxPlayersCount)
            {
                _manager.DisconnectClient(clientId, "Too many players"); //todo: display message on client
                return;
            }

            SpawnPlayer(clientId);
        }

        private void SpawnPlayer(ulong clientId)
        {
            var playerGO = UnityEngine.Object.Instantiate(_config.PlayerPrefab);
            playerGO.SpawnAsPlayerObject(clientId);
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

            SceneManager.LoadScene(0);
        }
    }
}
