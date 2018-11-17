using DevToolsMessage;
using System;
using System.Threading.Tasks;

namespace DevToolsClientCore.Socket
{
    public interface IDevRequestService
    {
        event EventHandler OnStateChanged;

        string State { get; }
        Task<bool> StartAsync(Uri remote);
        void Stop();
        Task SendRequest(DevRequest pRequest);
    }
}
