
using DevToolsConnector.Common;

using DevToolsMessage;

using System;
using System.Threading.Tasks;

namespace DevToolsConnector.Inspector
{
    public interface IDevToolClient : IDevToolConnector
    {
        IDevSocket Socket { get; }
        event EventHandler OnConnectChanged;
        bool IsConnected { get; }
        Task<bool> Connect(Uri remote);
        Task<DevMessage> SendMessage(DevMessage pRequest);
    }
}
