using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Network
{
    class ConnectionServiceBasic : IConnectionService
    {
        readonly GameNetworkManager _manager;
        readonly GameNetworkConfig _config;

        bool _isSubscribed;

        public ConnectionServiceBasic(GameNetworkManager manager, GameNetworkConfig config)
        {
            _manager = manager;
            _config = config;
        }

        public void StartHost()
        {
            if (_manager.StartHost())
            {
                SubscribeHost();
                _manager.SceneManager.LoadScene(_config.GameplaySceneName, LoadSceneMode.Single);
            }
        }

        public void StartClient()
        {
            if (_manager.StartClient())
            {
                SubscribeClient();
            }
        }

        public void Shutdown()
        {
            _manager.Shutdown();
        }

        private void SubscribeHost()
        {
            _manager.SceneManager.OnSceneEvent += OnSceneEventHost;

            if (!_isSubscribed)
            {
                _manager.OnClientConnectedCallback += OnClientConnectedHost;
                _manager.OnClientStopped += OnClientStopped;
                _isSubscribed = true;
            }
        }

        private void SubscribeClient()
        {
            if (!_isSubscribed)
            {
                _manager.OnClientStopped += OnClientStopped;
                _isSubscribed = true;
            }
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
            if (clientId != NetworkManager.ServerClientId)
                SpawnPlayer(clientId);
        }

        private void SpawnPlayer(ulong clientId)
        {
            var playerGO = Object.Instantiate(_config.PlayerPrefab);
            playerGO.SpawnAsPlayerObject(clientId);
        }

        private void OnClientStopped(bool isHost)
        {
            SceneManager.LoadScene(0);
        }
    }
}
