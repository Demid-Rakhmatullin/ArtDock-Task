using Game.Interactions;
using Game.Spawn;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

namespace Game.Player
{
    [DisallowMultipleComponent]
    class PlayerInputDispatcher : MonoBehaviour, IGrabDispatcher, ISpawnDispatcher
    {
        [SerializeField] PlayerInput _playerInput;

        public Vector2 Move { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }
        public Vector2 Look { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }
        public bool Grab { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }
        public bool Throw { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }

        //very simple way to signal single button press
        //todo: clean up values gracefully (at the end of frame?)
        //
        private bool _spawn;
        public bool Spawn 
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_spawn)
                {
                    _spawn = false;
                    return true;
                }

                return false;
            }
        }

        private bool _despawn;
        public bool Despawn
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_despawn)
                {
                    _despawn = false;
                    return true;
                }

                return false;
            }
        }

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

        [Preserve]
        private void OnSpawn(InputValue value)
            => _spawn = value.isPressed;

        [Preserve]
        private void OnDespawn(InputValue value)
            => _despawn = value.isPressed;

    }
}
