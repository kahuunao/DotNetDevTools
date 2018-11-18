using DevToolsMessage;

using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DevToolsConnector
{
    public interface IDevSocket
    {
        bool IsConnected { get; }

        void UseConnectedSocket(TcpClient socket);
        Task<bool> Connect(Uri pRemote);
        void Stop();
        Task Send(DevRequest pRequest);
    }
}