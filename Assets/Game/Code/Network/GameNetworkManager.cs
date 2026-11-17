using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Network
{
    [DisallowMultipleComponent]
    public class GameNetworkManager : MonoBehaviour
    {
        [SerializeField] NetworkManager networkManager;
        [SerializeField] NetworkObject playerPrefab;

#if UNITY_EDITOR
        [SerializeField] UnityEditor.SceneAsset gameplayScene;
        private void OnValidate()
        {
            if (gameplayScene != null)
                gameplaySceneName = gameplayScene.name;
        }
#endif
        [SerializeField] string gameplaySceneName;

        bool _isSubscribed;

        private void Awake()
        {
            //allow keeping NetworkManager on StartScreen scene without additional Startup scene
            //todo: additional scene?
            if (NetworkManager.Singleton != null && NetworkManager.Singleton != networkManager)
                Destroy(gameObject);
        }

        public void StartHost()
        {
            if (networkManager.StartHost())
            {
                SubscribeHost();
                networkManager.SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
            }
        }

        public void StartClient()
        {
            if (networkManager.StartClient())
            {
                SubscribeClient();
            }
        }

        private void SubscribeHost()
        {
            networkManager.SceneManager.OnSceneEvent += OnSceneEventHost;

            if (!_isSubscribed)
            {
                networkManager.OnClientConnectedCallback += OnClientConnectedHost;
                networkManager.OnClientStopped += OnClientStopped;
                _isSubscribed = true;
            }
        }

        private void SubscribeClient()
        {
            if (!_isSubscribed)
            {
                networkManager.OnClientStopped += OnClientStopped;
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
            var playerGO = Instantiate(playerPrefab);
            playerGO.SpawnAsPlayerObject(clientId);
        }

        private void OnClientStopped(bool isHost)
        {
            SceneManager.LoadScene(0);
        }
    }
}
