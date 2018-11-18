using DevToolsMessage;

using System;

namespace DevToolsConnector.Common
{
    public interface IDevToolConnector
    {
        void RegisterListener(EnumDevMessageType pType, IDevListener pListener);
        void RegisterListener(EnumDevMessageType pType, Action<IDevSocket, DevMessage> pListener);
        void UnRegisterListener(EnumDevMessageType pType, IDevListener pListener);
        void UnRegisterListener(EnumDevMessageType pType, Action<IDevSocket, DevMessage> pListener);
        void UnRegisterListener(IDevListener pListener);
        void UnRegisterListener(Action<IDevSocket, DevMessage> pListener);
        void Close();
    }
}
