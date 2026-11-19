namespace Game.Network
{
    public class NetworkClientData
    {
        public readonly ulong networkId;

        public string Name { get; internal set; }

        public NetworkClientData(ulong networkId)
        {
            this.networkId = networkId;
        }
    }
}
