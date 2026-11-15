using UnityEngine;
using VContainer;
using VContainer.Unity;
using static Game.Player.PlayerCameraScope;

namespace Game.Player
{
    class PlayerCameraService : ILateTickable
    {
        //todo: place this in CameraConfig
        //
        static readonly Vector3 OFFSET = new(0f, 4.3f, -9.7f);
        const float ROTATION_SPEED = 1f;

        readonly PlayerInputDispatcher _input;
        readonly Transform _cameraTransform;
        readonly Transform _playerTransform;

        public PlayerCameraService(
            PlayerInputDispatcher input,
            [Key(CAMERA_KEY)] Transform cameraTransform, 
            Transform playerTransform)
        {
            _input = input;
            _cameraTransform = cameraTransform;
            _playerTransform = playerTransform;
        }

        void ILateTickable.LateTick()
        {
            float yaw = _playerTransform.rotation.eulerAngles.y + 
                _input.Look.x * ROTATION_SPEED;
            _playerTransform.rotation = Quaternion.Euler(0f, yaw, 0f);

            Vector3 cameraPos = _playerTransform.TransformPoint(OFFSET);
            _cameraTransform.position = cameraPos;

            _cameraTransform.LookAt(_playerTransform);
        }
    }
}
