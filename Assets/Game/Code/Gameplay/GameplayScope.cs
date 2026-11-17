using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay
{
    [DisallowMultipleComponent]
    class GameplayScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameplayData>(Lifetime.Singleton);
        }
    }
}
