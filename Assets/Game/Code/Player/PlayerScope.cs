using Game.Interactions;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using static Game.Interactions.InteractionsKeys;

namespace Game.Player
{
    class PlayerScope : LifetimeScope
    {
        [SerializeField] PlayerInputDispatcher _input;
        [SerializeField] CharacterController _controller;
        [SerializeField] Transform _mainTransform;
        [SerializeField] Transform _gripPosition;

        List<LifetimeScope> _childScopes;

        protected override void Configure(IContainerBuilder builder)
        {
            _childScopes = new List<LifetimeScope>();

            builder.RegisterComponent(_mainTransform);
            builder.RegisterComponent(_input)
                .AsImplementedInterfaces();
            builder.RegisterComponent(_controller)
                .As<Collider>();
            builder.RegisterComponent(_gripPosition)
                .Keyed(GRIP_POSITION);

            builder.RegisterEntryPoint<PlayerMoveService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GrabService>(Lifetime.Singleton);

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
