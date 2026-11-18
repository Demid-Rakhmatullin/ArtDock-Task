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
        private void OnValidate()
        {
            if (gameplayScene != null)
                GameplaySceneName = gameplayScene.name;
        }
#endif
        public string GameplaySceneName { get; private set; }

        [field: SerializeField]
        public bool UseSteam { get; private set; }
    }
}
