using DevToolsMessage;

namespace DevToolsConnector
{
    public interface IDevRequestHandler
    {
        void HandleRequest(IDevSocket pSocket, DevRequest pRequest);
    }
}
