using DevToolsConnector.Common;

namespace DevToolsConnector.Inspected
{
    public interface IDevToolServer
    {
        IDevSocket Client { get; }

        void Bound(int? pPort = null);
        void Close();
    }
}
