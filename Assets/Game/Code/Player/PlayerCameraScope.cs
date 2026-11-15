using UnityEngine;
using VContainer;
using VContainer.Unity;
using static Game.Player.PlayerKeys;

namespace Game.Player
{
    class PlayerCameraScope : LifetimeScope  
    {

        [SerializeField] Transform _cameraTransform;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_cameraTransform)
                .Keyed(CAMERA_MAIN);

            builder.RegisterEntryPoint<PlayerCameraService>(Lifetime.Singleton);
        }
    }
}
