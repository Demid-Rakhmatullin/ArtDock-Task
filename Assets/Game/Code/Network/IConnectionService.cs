using System;
using System.Threading.Tasks;
using Unity.Netcode;

namespace Game.Network
{
    public interface IConnectionService
    {
        internal Func<NetworkManager.ConnectionApprovalRequest, (bool, string)> OuterApprove { set; }

        event Action<ulong> OnConnected;

        event Action OnDisconnecting;

        event Action<bool> OnDisconnected;

        event Action<ulong> OnOtherClientConnected;

        event Action<ulong> OnOtherClientDisconnected;

        Task<bool> StartHost();

        bool StartClient();

        void Shutdown();
    }
}
