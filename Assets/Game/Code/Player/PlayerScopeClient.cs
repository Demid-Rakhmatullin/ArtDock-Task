using Game.Interactions;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using static Game.Interactions.InteractionsKeys;

namespace Game.Player
{
    [DisallowMultipleComponent]
    class PlayerScopeClient : LifetimeScope
    {
        [SerializeField] PlayerObject _object;

        List<LifetimeScope> _childScopes;

        protected override void Configure(IContainerBuilder builder)
        {
            _childScopes = new List<LifetimeScope>();

            builder.RegisterComponent(_object.Input)
                .AsImplementedInterfaces();
            builder.RegisterComponent(_object.Controller)
                .As<Collider>();
            builder.RegisterComponent(_object.Network)
                .As<IGrabNetworkClient>();
            builder.RegisterComponent(_object.MainTransform);
            builder.RegisterComponent(_object.GripPosition)
                .Keyed(GRIP_POSITION);

            builder.RegisterEntryPoint<PlayerMoveService>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GrabServiceClient>(Lifetime.Singleton);

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

            _object.Input.Enable();
        }

        private void OnDispose(IObjectResolver resolver)
        {
            foreach (var child in _childScopes)
                child.DisposeCore();
        }
    }
}
