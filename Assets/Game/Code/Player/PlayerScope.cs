using System.Collections.Generic;
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

        List<LifetimeScope> _childScopes;

        protected override void Configure(IContainerBuilder builder)
        {
            _childScopes = new List<LifetimeScope>();

            builder.RegisterComponent(_input);
            builder.RegisterComponent(_controller);
            builder.RegisterComponent(_mainTransform);

            builder.RegisterEntryPoint<PlayerMoveService>(Lifetime.Singleton);

            builder.RegisterBuildCallback(AfterConfigure);
            builder.RegisterDisposeCallback(OnDispose);
        }

        private void AfterConfigure(IObjectResolver resolver)
        {
            var cameraScope = Find<PlayerCameraScope>();
            using (EnqueueParent(this))
            {
                cameraScope.Build();
            }
            _childScopes.Add(cameraScope);

            _input.Enable();
        }

        private void OnDispose(IObjectResolver resolver)
        {
            foreach (var child in _childScopes)
                child.DisposeCore();
        }
    }
}
