
using DevToolsMessage;

using System;
using System.Threading.Tasks;

namespace DevToolsConnector
{
    public interface IDevToolClient : IDevToolConnector
    {
        event EventHandler OnConnectChanged;
        bool IsConnected { get; }

        IDevSocket Socket { get; }

        Task<bool> Connect(Uri remote);
        Task<DevMessage> SendMessage(DevMessage pRequest);
    }
}
