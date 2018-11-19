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

        #region Register
        public void RegisterListener<TMessage>(IDevListener pListener)
        {
            var ls = GetListeners(typeof(TMessage));
            if (!ls.Contains(pListener))
            {
                ls.Add(pListener);
            }
        }

        public void RegisterListener<T>(Action<IDevSocket, IDevMessage> pListener)
        {
            RegisterListener<T>(new FuncDevListener(pListener));
        }

        public void UnRegisterListener<T>(IDevListener pListener)
        {
            GetListeners(typeof(T)).Remove(pListener);
        }

        public void UnRegisterListener<T>(Action<IDevSocket, IDevMessage> pListener)
        {
            GetListeners(typeof(T)).RemoveAll((l) => l is FuncDevListener f && f.Fct == pListener);
        }
        #endregion

        public void ClearListeners()
        {
            _listeners.Clear();
        }

        public abstract void Close();

        protected List<IDevListener> GetListeners(Type pMessageType)
        {
            if (pMessageType == null)
            {
                return null;
            }
            string messageType = pMessageType.FullName;
            if (!_listeners.ContainsKey(messageType))
            {
                _listeners.Add(messageType, new List<IDevListener>());
            }
            return _listeners[messageType];
        }

        protected void DispatchMessage(IDevSocket pSocket, IDevMessage pMessage)
        {
            if (pMessage == null)
            {
                return;
            }

            bool hasListener = false;
            GetListeners(pMessage?.GetType()).ForEach((l) =>
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

            if (!hasListener && pMessage is IDevRequest request)
            {
                // Message non interprété
                pSocket.RespondAt(request, null, false);
            }
        }
    }
}
