using DevToolsMessage;

namespace DevToolsConnector
{
    public interface IDevListener
    {
        void HandleResponse(IDevSocket pSocket, DevMessage pMessage);
    }
}
