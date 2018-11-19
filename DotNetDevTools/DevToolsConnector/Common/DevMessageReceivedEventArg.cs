using DevToolsMessage;

using System;

namespace DevToolsConnector.Common
{
    public class DevMessageReceivedEventArg : EventArgs
    {
        public IDevMessage MessageReceived { get; private set; }

        public DevMessageReceivedEventArg(IDevMessage pMessage)
        {
            MessageReceived = pMessage;
        }
    }
}
