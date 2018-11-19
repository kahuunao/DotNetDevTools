using DevToolsMessage;

namespace DevToolsConnector.Common
{
    public interface IDevListener
    {
        void HandleMessage(IDevSocket pSocket, IDevMessage pMessage);
    }
}
