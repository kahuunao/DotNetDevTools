using NLog;

using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace DevToolsClientCore.Socket
{
    public class CommunicationService : ICommunicationService
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public event EventHandler OnStateChanged;

        private ClientWebSocket _socket;

        private string _state = "En attente de connexion";
        public string State
        {
            get
            {
                return _state;
            }
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public async Task StartAsync(Uri pRemote)
        {
            try
            {
                if (_socket != null)
                {
                    _socket.Abort();
                    _socket.Dispose();
                }
            }
            catch (Exception e)
            {
                LOGGER.Error(e, "Erreur durant la fermeture de la socket précédente");
            }

            try
            {
                _socket = new ClientWebSocket();
                await _socket.ConnectAsync(pRemote, new System.Threading.CancellationToken());
                LOGGER.Debug("Socket state {0}", _socket.State);
            }
            catch (Exception e)
            {
                LOGGER.Error(e, "Erreur durant l'ouverture de la socket vers l'uri {0}", pRemote);
            }
            finally
            {
                if (_socket.State == WebSocketState.Open)
                {
                    State = $"Connecté à {_socket}";
                }
                else
                {
                    State = "Echec de connexion";
                }
            }
        }
    }
}
