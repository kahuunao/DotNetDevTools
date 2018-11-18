using DevToolsMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevToolsConnector.Impl
{
    public abstract class AbstractDevTool : IDevToolConnector
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        private Dictionary<EnumDevMessageType, List<IDevListener>> _listeners = new Dictionary<EnumDevMessageType, List<IDevListener>>();

        public void RegisterListener(EnumDevMessageType pType, IDevListener pListener)
        {
            var ls = GetListeners(pType);
            if (!ls.Contains(pListener))
            {
                ls.Add(pListener);
            }
        }

        public void RegisterListener(EnumDevMessageType pType, Action<IDevSocket, DevMessage> pListener)
        {
            RegisterListener(pType, new FuncDevListener(pListener));
        }

        public void UnRegisterListener(EnumDevMessageType pType, IDevListener pListener)
        {
            GetListeners(pType).Remove(pListener);
        }

        public void UnRegisterListener(EnumDevMessageType pType, Action<IDevSocket, DevMessage> pListener)
        {
            GetListeners(pType).RemoveAll((l) => l is FuncDevListener f && f.Fct == pListener);
        }

        public void UnRegisterListener(IDevListener pListener)
        {
            foreach (var key in _listeners.Keys)
            {
                UnRegisterListener(key, pListener);
            }
        }

        public void UnRegisterListener(Action<IDevSocket, DevMessage> pListener)
        {
            foreach (var key in _listeners.Keys)
            {
                UnRegisterListener(key, pListener);
            }
        }

        public void ClearListeners()
        {
            _listeners.Clear();
        }

        public abstract void Close();

        protected List<IDevListener> GetListeners(EnumDevMessageType pType)
        {
            if (!_listeners.ContainsKey(pType))
            {
                _listeners.Add(pType, new List<IDevListener>());
            }
            return _listeners[pType];
        }

        protected void DispatchMessage(IDevSocket pSocket, DevMessage pMessage)
        {
            GetListeners(pMessage.RequestType).ForEach((l) =>
            {
                try
                {
                    l?.HandleResponse(pSocket, pMessage);
                }
                catch (Exception e)
                {
                    LOGGER.Error(e, "Une erreur s'est produit pendant la gestion des messages");
                }
            });
        }
    }

    class FuncDevListener : IDevListener
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
