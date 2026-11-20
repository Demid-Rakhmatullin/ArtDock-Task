using Unity.Netcode;
using UnityEngine;

namespace Game.Network
{
    [CreateAssetMenu(fileName = "GameNetworkConfig", menuName = "Game/GameNetworkConfig")]
    public class GameNetworkConfig : ScriptableObject
    {
        [field: SerializeField]
        public NetworkObject PlayerPrefab { get; private set; }

        //todo: [Scene] attribute        
#if UNITY_EDITOR
        [SerializeField] UnityEditor.SceneAsset gameplayScene;
        [SerializeField] UnityEditor.SceneAsset lobbyScene;
        private void OnValidate()
        {
            if (gameplayScene != null)
                GameplaySceneName = gameplayScene.name;

            if (lobbyScene != null)
                LobbySceneName = lobbyScene.name;
        }
#endif
        [field: SerializeField]
        public string GameplaySceneName { get; private set; }

        [field: SerializeField]
        public string LobbySceneName { get; private set; }

        [field: SerializeField]
        public int MaxPlayersCount { get; private set; }

        [field: SerializeField]
        public bool UseSteam { get; private set; }

        [field: SerializeField]
        public float SteamInvitePingSeconds { get; private set; }
    }
}
