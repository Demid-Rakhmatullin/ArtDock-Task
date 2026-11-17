using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [DisallowMultipleComponent]
    class StartScreenView : MonoBehaviour
    {
        [field: SerializeField]
        public Button CreateBtn { get; private set; }

        [field: SerializeField]
        public Button JoinBtn { get; private set; }

        [field: SerializeField]
        public Button ExitBtn { get; private set; }
    }
}
