using Unity.Netcode;
using UnityEngine;

namespace Game.Network
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ManagerData))]
    class GameNetworkManager : NetworkManager
    {
        //additional serializable fields of NetworkManager inherited classes do not shown in Inspector
        // is it caused by strict custom Editor scripts for NetworkManager?
        ManagerData _data;

        GameNetworkConfig _config;
        public void SetConfig(GameNetworkConfig config) => _config = config;

        protected override void Awake()
        {
            //allow keeping NetworkManager on Menu scene without additional Startup scene
            //todo: additional scene?
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            _data = GetComponent<ManagerData>();

            NetworkConfig.NetworkTransport = _config.UseSteam
                ? _data.SteamTransport
                : _data.UnityTransport;

            base.Awake();
        }
    }
}
