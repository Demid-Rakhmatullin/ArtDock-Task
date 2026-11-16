using UnityEngine;

namespace Game.Player
{
    [DisallowMultipleComponent]
    class PlayerObject : MonoBehaviour
    {
        [field: SerializeField]
        public PlayerInputDispatcher Input { get; private set; }

        [field: SerializeField]
        public CharacterController Controller { get; private set; }

        [field: SerializeField]
        public PlayerNetwork Network { get; private set; }

        [field: SerializeField]
        public Transform MainTransform { get; private set; }

        [field: SerializeField]
        public Transform GripPosition { get; private set; }
    }
}
