namespace Game.Network
{
    class ClientDataProviderSimple : IClientDataProvider
    {
        public void UpdateData(NetworkClientData data)
        {
            data.Name = $"Player {data.networkId + 1}";
        }
    }
}
