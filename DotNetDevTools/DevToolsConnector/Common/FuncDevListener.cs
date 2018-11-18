using DevToolsMessage;

using System;

namespace DevToolsConnector.Common
{
    public class FuncDevListener : IDevListener
    {
        public Action<IDevSocket, DevMessage> Fct { get; set; }

        public FuncDevListener(Action<IDevSocket, DevMessage> pFct)
        {
            Fct = pFct;
        }

        public void HandleResponse(IDevSocket pSocket, DevMessage pMessage)
        {
            Fct?.Invoke(pSocket, pMessage);
        }
    }
}
