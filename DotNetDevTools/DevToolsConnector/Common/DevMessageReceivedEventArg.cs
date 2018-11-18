using DevToolsMessage;

using System;

namespace DevToolsConnector.Common
{
    public class DevMessageReceivedEventArg : EventArgs
    {
        public DevMessage MessageReceived { get; private set; }

        public DevMessageReceivedEventArg(DevMessage pMessage)
        {
            MessageReceived = pMessage;
        }
    }
}
