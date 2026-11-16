using Game.Interactions;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

namespace Game.Player
{
    [DisallowMultipleComponent]
    class PlayerInputDispatcher : MonoBehaviour, IGrabDispatcher
    {
        [SerializeField] PlayerInput _playerInput;

        public Vector2 Move { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }
        public Vector2 Look { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }
        public bool Grab { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }
        public bool Throw { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }

        public void Enable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _playerInput.enabled = true;
        }

        [Preserve]
        private void OnMove(InputValue value) 
            => Move = value.Get<Vector2>();

        [Preserve]
        private void OnLook(InputValue value)
            => Look = value.Get<Vector2>();

        [Preserve]
        private void OnGrab(InputValue value)
            => Grab = value.isPressed;

        [Preserve]
        private void OnThrow(InputValue value)
            => Throw = value.isPressed;

    }
}
