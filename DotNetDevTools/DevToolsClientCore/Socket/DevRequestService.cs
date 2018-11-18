using DevToolsConnector;

using DevToolsMessage;

using NLog;

using System;
using System.Threading.Tasks;

namespace DevToolsClientCore.Socket
{
    public class DevRequestService : IDevRequestService
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public event EventHandler OnStateChanged;

        private readonly IDevSocketFactory _factory;
        private IDevSocket _socket;

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

        public DevRequestService(IDevSocketFactory pFactory)
        {
            _factory = pFactory;
        }

        public async Task<bool> StartAsync(Uri pRemote)
        {
            // Fermeture des précédents connexions
            Stop();

            _socket = _factory.BuildSocket();
            var isConnected =  await _socket.Connect(pRemote);
            State = isConnected ? "Connecté" : "Echec de connexion";
            return isConnected;
        }

        public void Stop()
        {
            if (_socket != null)
            {
                _socket.Stop();
                _socket = null;
            }
        }

        public Task SendRequest(DevRequest pRequest)
        {
            return _socket?.Send(pRequest);
        }
    }
}
