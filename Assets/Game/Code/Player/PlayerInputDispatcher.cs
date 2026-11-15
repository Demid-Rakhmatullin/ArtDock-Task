using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    class PlayerInputDispatcher : MonoBehaviour
    {
        [SerializeField] PlayerInput _playerInput;

        public Vector2 Move { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }
        public Vector2 Look { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }

        public void Enable()
        {
            _playerInput.enabled = true;
        }

        public void OnMove(InputValue value)     
            => Move = value.Get<Vector2>();

        public void OnLook(InputValue value)
            => Look = value.Get<Vector2>();

    }
}
