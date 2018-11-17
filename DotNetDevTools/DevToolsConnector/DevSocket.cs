using DevToolsMessage;
using Fleck;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevToolsConnector
{
    public class DevSocket
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public event EventHandler OnClosed;

        public IWebSocketConnection Socket { get; private set; }
        public IDevRequestHandler Handler { get; private set; }

        public DevSocket(IWebSocketConnection socket, IDevRequestHandler pHandler)
        {
            Socket = socket;
            socket.OnOpen = OnOpen;
            socket.OnClose = OnClose;
            socket.OnError = OnError;
            socket.OnMessage = OnMessage;
            Handler = pHandler;
        }

        private void OnOpen()
        {
            LOGGER.Debug("Open from : {0}", Socket.ConnectionInfo.ClientIpAddress);
        }

        private void OnClose()
        {
            LOGGER.Debug("Close from : {0}", Socket.ConnectionInfo.ClientIpAddress);
            OnClosed?.Invoke(this, EventArgs.Empty);
        }

        private void OnError(Exception pE)
        {
            LOGGER.Error("Error : {0} from : {1}", pE, Socket.ConnectionInfo.ClientIpAddress);
        }

        private void OnMessage(string pMessage)
        {   
            LOGGER.Debug("Receive message : {0} from : {1}", pMessage, Socket.ConnectionInfo.ClientIpAddress);
            try
            {
                var request = JsonConvert.DeserializeObject<DevRequest>(pMessage);
                Handler?.HandleRequest(this, request);
            }
            catch(Exception e)
            {
                LOGGER.Error(e, "Invalide message: {0}", pMessage);
            }
        }
    }
}
