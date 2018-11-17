namespace DevToolsConnector
{
    public interface IDevToolServer
    {
        void Init(IDevRequestHandler pHandler, int? pPort = null);
        void Close();
    }
}
