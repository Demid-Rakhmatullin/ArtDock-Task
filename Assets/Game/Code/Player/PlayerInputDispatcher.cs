using Game.Gameplay;
using Game.Interactions;
using Game.Spawn;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using PreserveAttribute = UnityEngine.Scripting.PreserveAttribute;

namespace Game.Player
{
    [DisallowMultipleComponent]
    class PlayerInputDispatcher : MonoBehaviour, IGrabDispatcher, ISpawnDispatcher
    {
        [SerializeField] PlayerInput playerInput;

        GameplayData _gpData; 

        public Vector2 Move { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }
        public Vector2 Look { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }
        public bool Grab { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }
        public bool Throw { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; private set; }

        //very simple way to signal single button press
        //todo: clean up values gracefully (at the end of frame?)
        //
        bool _spawn;
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

        bool _despawn;
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

        [Inject]
        public void Construct(GameplayData gpData)
        {
            _gpData = gpData;

            gpData.OnStateChanged += OnGameplayStateChange;
            OnGameplayStateChange(gpData.State);
        }

        private void OnDestroy()
        {
            if (_gpData != null)
                _gpData.OnStateChanged -= OnGameplayStateChange;

            Cursor.lockState = CursorLockMode.None;
        }

        private void OnGameplayStateChange(GameplayState state)
        {
            switch (state)
            {
                case GameplayState.Active:
                    EnableInput(); break;
                case GameplayState.Pause:
                    DisableInput(); break;
            }
        }

        private void EnableInput()
        {
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.enabled = true;
        }

        private void DisableInput()
        {
            Cursor.lockState = CursorLockMode.None;
            playerInput.enabled = false;
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
