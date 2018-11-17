using DevToolsMessage;

namespace DevToolsConnector
{
    public interface IDevRequestHandler
    {
        void HandleRequest(DevSocket pSocket, DevRequest pRequest);
    }
}
