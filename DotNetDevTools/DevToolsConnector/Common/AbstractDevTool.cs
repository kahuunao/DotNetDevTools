using DevToolsMessage;

using NLog;

using System;
using System.Collections.Generic;

namespace DevToolsConnector.Common
{
    public abstract class AbstractDevTool : IDevToolConnector
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        private Dictionary<string, List<IDevListener>> _listeners = new Dictionary<string, List<IDevListener>>();

        public void RegisterListener(string pType, IDevListener pListener)
        {
            var ls = GetListeners(pType);
            if (!ls.Contains(pListener))
            {
                ls.Add(pListener);
            }
        }

        public void RegisterListener(string pType, Action<IDevSocket, IDevMessage> pListener)
        {
            RegisterListener(pType, new FuncDevListener(pListener));
        }

        public void UnRegisterListener(string pType, IDevListener pListener)
        {
            GetListeners(pType).Remove(pListener);
        }

        public void UnRegisterListener(string pType, Action<IDevSocket, IDevMessage> pListener)
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

        public void UnRegisterListener(Action<IDevSocket, IDevMessage> pListener)
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

        protected List<IDevListener> GetListeners(string pType)
        {
            if (!_listeners.ContainsKey(pType))
            {
                _listeners.Add(pType, new List<IDevListener>());
            }
            return _listeners[pType];
        }

        protected void DispatchMessage(IDevSocket pSocket, IDevMessage pMessage)
        {
            bool hasListener = false;
            GetListeners(pMessage.Type).ForEach((l) =>
            {
                try
                {
                    hasListener = true;
                    l?.HandleMessage(pSocket, pMessage);
                }
                catch (Exception e)
                {
                    LOGGER.Error(e, "Une erreur s'est produit pendant la gestion des messages");
                }
            });

            if (!hasListener && pMessage is IDevRequest)
            {
                // Message non interprété
                pSocket.RespondAt(pMessage, null, false);
            }
        }
    }
}
