using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.UI
{
    [DisallowMultipleComponent]
    class LeaveConfirmScope : LifetimeScope
    {
        [SerializeField] LeaveConfirmView view;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(view);
            builder.RegisterEntryPoint<LeaveConfirmPresenter>();
        }
    }
}
