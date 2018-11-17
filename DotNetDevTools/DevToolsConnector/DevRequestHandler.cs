using DevToolsMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevToolsConnector
{
    public class DevRequestHandler : IDevRequestHandler
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public void HandleRequest(DevSocket pSocket, DevRequest pRequest)
        {
            if (pRequest == null)
            {
                LOGGER.Warn("Request null");
                return;
            }

            switch (pRequest.RequestType)
            {
                case EnumDevRequestType.UNDEFINED:
                    LOGGER.Warn("Request undefined");
                    break;
                case EnumDevRequestType.GET_FILE_CONFIG:
                    SendFileConfig(pSocket).RunSafe();
                    break;
                default:
                    LOGGER.Warn("Request undefined");
                    break;
            }
        }

        private Task SendFileConfig(DevSocket pSocket)
        {
            // TODO
            return pSocket.Socket.Send("Toto");
        }
    }
}
