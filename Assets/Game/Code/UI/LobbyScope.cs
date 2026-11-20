using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI
{
    [DisallowMultipleComponent]
    class LobbyScope : LifetimeScope
    {
        [SerializeField] LobbyView view;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(view);
            builder.RegisterEntryPoint<LobbyPresenter>();
        }
    }
}
