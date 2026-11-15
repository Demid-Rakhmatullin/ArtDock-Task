using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Player
{
    class PlayerCameraScope : LifetimeScope  
    {
        public const string CAMERA_KEY = "PLAYER_CAMERA_KEY";

        [SerializeField] Transform _cameraTransform;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_cameraTransform)
                .Keyed(CAMERA_KEY);

            builder.RegisterEntryPoint<PlayerCameraService>(Lifetime.Singleton);
        }
    }
}
