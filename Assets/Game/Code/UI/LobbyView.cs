using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [DisallowMultipleComponent]
    class LobbyView : MonoBehaviour
    {
        [field: SerializeField]
        public Button StartBtn { get; private set; }
    }
}
