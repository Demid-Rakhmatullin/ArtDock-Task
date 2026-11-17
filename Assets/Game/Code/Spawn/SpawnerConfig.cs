using Unity.Netcode;
using UnityEngine;

namespace Game.Spawn
{
    [CreateAssetMenu(fileName = "NewSpawnerConfig", menuName = "Game/SpawnerConfig")]
    public class SpawnerConfig : ScriptableObject
    {
        public NetworkObject[] NetworkPrefabs;
    }
}
