using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI
{
    [DisallowMultipleComponent]
    class MenuScope : LifetimeScope
    {
        [SerializeField] MenuView view;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(view);
            builder.RegisterEntryPoint<MenuPresenter>();
        }
    }
}
