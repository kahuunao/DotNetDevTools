using DevToolsMessage;

using System;

namespace DevToolsConnector.Common
{
    public class FuncDevListener : IDevListener
    {
        public Action<IDevSocket, IDevMessage> Fct { get; set; }

        public FuncDevListener(Action<IDevSocket, IDevMessage> pFct)
        {
            Fct = pFct;
        }

        public void HandleMessage(IDevSocket pSocket, IDevMessage pMessage)
        {
            Fct?.Invoke(pSocket, pMessage);
        }
    }
}
