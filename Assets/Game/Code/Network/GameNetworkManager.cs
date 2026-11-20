using Game.Common;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Network
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ManagerData))]
    class GameNetworkManager : NetworkManager
    {
        //additional serializable fields of NetworkManager inherited classes do not shown in Inspector
        //is it caused by strict custom Editor scripts for NetworkManager?
        ManagerData _data;

        GameNetworkConfig _config;

        public void Install(
            IContainerBuilder builder,
            GameNetworkConfig config)
        {
            //all network services are consider singletons
            //also keep GameNetworkManager outside DI container (architecture decision)

            _data = GetComponent<ManagerData>();

            _config = config;
            builder.RegisterInstance(config);

            if (_config.UseSteam)
            {
                NetworkConfig.NetworkTransport = _data.SteamTransport;

                builder.Register<ClientDataProviderSteam>(Lifetime.Singleton)
                    .WithParameter(typeof(SharedDataSteam), _data.SteamDataPrefab)
                    .AsImplementedInterfaces()
                    .AsSelf();

                builder.Register<ConnectionServiceBasic>(Lifetime.Transient)
                    .WithParameter(typeof(GameNetworkManager), this)
                    .AsSelf();
                builder.RegisterEntryPoint(r =>
                        //decorators are not supported yet by VContainer, so use manual resolution
                        new ConnectionServiceDecoratorSteam(
                            r.Resolve<ConnectionServiceBasic>(),
                            r.Resolve<GameNetworkConfig>(),
                            this,
                            r.Resolve<SystemMessagesService>(),
                            _data.SteamTransport,
                            r.Resolve<ClientDataProviderSteam>()),
                        Lifetime.Singleton);
            }
            else
            {
                NetworkConfig.NetworkTransport = _data.UnityTransport;

                builder.Register<ClientDataProviderSimple>(Lifetime.Singleton)
                    .AsImplementedInterfaces();

                builder.Register<ConnectionServiceBasic>(Lifetime.Singleton) 
                    .WithParameter(typeof(GameNetworkManager), this)
                    .AsImplementedInterfaces();
            }

            builder.Register<NetworkClientsService>(Lifetime.Singleton)
                .WithParameter(typeof(GameNetworkManager), this)
                .AsImplementedInterfaces();
        }

        protected override void Awake()
        {
            //allow keeping NetworkManager on Menu scene without additional Startup scene
            //todo: additional scene?
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            base.Awake();
        }
    }
}
