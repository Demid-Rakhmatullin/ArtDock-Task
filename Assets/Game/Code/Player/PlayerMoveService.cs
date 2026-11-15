using UnityEngine;
using VContainer.Unity;

namespace Game.Player
{
    class PlayerMoveService : ITickable
    {
        //todo: place this in MoveConfig
        //
        const float MOVE_SPEED = 3f;
        const float MAX_VELOCITY = 50f;

        readonly PlayerInputDispatcher _input;
        readonly CharacterController _controller;

        float _verticalVelocity;

        public PlayerMoveService(PlayerInputDispatcher input, CharacterController controller)
        {
            _input = input;
            _controller = controller;
        }

        void ITickable.Tick()
        {
            Vector3 moveInput = new (_input.Move.x, 0f, _input.Move.y);
            Vector3 horizontalMove = _controller.transform.TransformVector(moveInput)
                * MOVE_SPEED;

            bool isGrounded = _controller.isGrounded;
            if (!isGrounded)
            {
                _verticalVelocity = Mathf.Min(
                    MAX_VELOCITY,
                    _verticalVelocity + Physics.gravity.y * Time.deltaTime);
            }
            else
            {         
                _verticalVelocity = 0f;
            }

            Vector3 finalMove = horizontalMove + new Vector3(0f, _verticalVelocity, 0f);

            _controller.Move(finalMove * Time.deltaTime);
        }
    }
}
