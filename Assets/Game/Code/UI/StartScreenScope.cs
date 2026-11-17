using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI
{
    [DisallowMultipleComponent]
    class StartScreenScope : LifetimeScope
    {
        [SerializeField] StartScreenView view;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(view);
            builder.RegisterEntryPoint<StartScreenPresenter>();
        }
    }
}
