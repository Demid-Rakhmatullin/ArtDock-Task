using System;
using System.Threading.Tasks;

namespace Game.Network
{
    public interface IConnectionService
    {
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
