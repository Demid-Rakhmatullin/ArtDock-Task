using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Player
{
    class PlayerScope : LifetimeScope
    {
        [SerializeField] PlayerInputDispatcher _input;
        [SerializeField] CharacterController _controller;
        [SerializeField] Transform _mainTransform;

        protected override void Configure(IContainerBuilder builder)
        {
            Cursor.lockState = CursorLockMode.Locked;

            builder.RegisterComponent(_input);
            builder.RegisterComponent(_controller);
            builder.RegisterComponent(_mainTransform);

            builder.RegisterEntryPoint<PlayerMoveService>(Lifetime.Scoped);
            builder.RegisterBuildCallback(AfterConfigure);
        }

        private void AfterConfigure(IObjectResolver resolver)
        {
            var cameraScope = Find<PlayerCameraScope>();
            using (EnqueueParent(this))
            {
                cameraScope.Build();
            }

            _input.Enable();
        }
    }
}
