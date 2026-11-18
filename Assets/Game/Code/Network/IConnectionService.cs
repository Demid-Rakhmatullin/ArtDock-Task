using System;
using System.Threading.Tasks;

namespace Game.Network
{
    public interface IConnectionService
    {
        event Action OnDisconnecting;

        event Action<bool> OnDisconnected;

        Task<bool> StartHost();

        bool StartClient();

        void Shutdown();
    }
}
