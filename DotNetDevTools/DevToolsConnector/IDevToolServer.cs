namespace DevToolsConnector
{
    public interface IDevToolServer : IDevToolConnector
    {
        void Bound(int? pPort = null);
    }
}
