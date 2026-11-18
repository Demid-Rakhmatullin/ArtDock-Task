namespace Game.Network
{
    public interface IConnectionService
    {
        void StartHost();

        void StartClient();

        void Shutdown();
    }
}
