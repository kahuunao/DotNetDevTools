using System;
using System.Threading.Tasks;

namespace DevToolsClientCore.Socket
{
    public interface ICommunicationService
    {
        event EventHandler OnStateChanged;

        string State { get; }
        Task StartAsync(Uri remote);
    }
}
