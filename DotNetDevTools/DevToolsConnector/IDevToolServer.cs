namespace DevToolsConnector
{
    public interface IDevToolServer
    {
        void Init(int? pPort = null);
        void Close();
    }
}
