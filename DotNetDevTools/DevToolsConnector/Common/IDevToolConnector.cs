using DevToolsMessage;

using System;

namespace DevToolsConnector.Common
{
    public interface IDevToolConnector
    {
        void RegisterListener<TMessageType>(IDevListener pListener);
        void RegisterListener<TMessageType>(Action<IDevSocket, IDevMessage> pListener);
        void UnRegisterListener<TMessageType>(IDevListener pListener);
        void UnRegisterListener<TMessageType>(Action<IDevSocket, IDevMessage> pListener);
        void Close();
    }
}
