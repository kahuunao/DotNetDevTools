using DevToolsMessage;

namespace DevToolsConnector.Common
{
    public interface IDevListener
    {
        void HandleResponse(IDevSocket pSocket, DevMessage pMessage);
    }
}
