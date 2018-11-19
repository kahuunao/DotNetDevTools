using DevToolsMessage;

using System;

namespace DevToolsConnector.Common
{
    public interface IDevToolConnector
    {
        void RegisterListener(string pType, IDevListener pListener);
        void RegisterListener(string pType, Action<IDevSocket, IDevMessage> pListener);
        void UnRegisterListener(string pType, IDevListener pListener);
        void UnRegisterListener(string pType, Action<IDevSocket, IDevMessage> pListener);
        void UnRegisterListener(IDevListener pListener);
        void UnRegisterListener(Action<IDevSocket, IDevMessage> pListener);
        void Close();
    }
}
