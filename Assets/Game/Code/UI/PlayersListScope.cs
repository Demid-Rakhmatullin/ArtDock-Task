using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI
{
    [RequireComponent(typeof(PlayersListView))]
    [DisallowMultipleComponent]
    class PlayersListScope : LifetimeScope
    {
        [SerializeField] PlayersListView _view;
        [SerializeField] bool _showByKey;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_view);
            builder.RegisterEntryPoint<PlayersListPresenter>(Lifetime.Singleton)
                .WithParameter(typeof(bool), _showByKey);
        }
    }
}
