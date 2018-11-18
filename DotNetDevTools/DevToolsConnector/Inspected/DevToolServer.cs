using DevToolsConnector.Common;

using NLog;

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DevToolsConnector.Inspected
{
    /// <summary>
    /// Server de connexion pour les outils de développeur
    /// </summary>
    public class DevToolServer : AbstractDevTool, IDevToolServer
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        private readonly IDevSocketFactory _factory;

        private TcpListener _server;

        private IDevSocket _client;
        public IDevSocket Client
        {
            get
            {
                return _client;
            }
            private set
            {
                if (_client != null)
                {
                    _client.OnMessageReceived -= OnMessageReceivedHandler;
                    _client.OnConnectionChanged -= OnConnectionChangedHandler;
                }
                _client = value;
                if (_client != null)
                {
                    _client.OnMessageReceived += OnMessageReceivedHandler;
                    _client.OnConnectionChanged += OnConnectionChangedHandler;
                }
            }
        }

        public DevToolServer(IDevSocketFactory pFactory)
        {
            _factory = pFactory;
        }

        /// <summary>
        /// Initialisation du server
        /// </summary>
        /// <param name="pHandler"></param>
        /// <param name="pPort"></param>
        public void Bound(int? pPort = null)
        {
            Close();

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Loopback, pPort ?? 12000);
            LOGGER.Debug("Démarrage du serveur ", localEndPoint);
            _server = new TcpListener(localEndPoint);
            _server.Start();
            Listen().RunSafe();
        }

        /// <summary>
        /// Ecoute de connexion
        /// </summary>
        /// <returns></returns>
        private async Task Listen()
        {
            if (_server == null || _server.Server == null || !_server.Server.IsBound)
            {
                LOGGER.Debug("Serveur non démarré");
                return;
            }

            while (_server.Server.IsBound)
            {
                LOGGER.Debug("En attente d'un nouvelle connexion ...");
                var newClient = await _server.AcceptTcpClientAsync();
                RegisterSocket(newClient);
            }
        }

        /// <summary>
        /// Ferme toutes les connexions
        /// </summary>
        public override void Close()
        {
            LOGGER.Debug("Fermeture des connexions dev");
            Client?.Close();
            Client = null;
            _server?.Stop();
            _server = null;
        }

        private void RegisterSocket(TcpClient pNewSocket)
        {
            if (pNewSocket != null)
            {
                try
                {
                    Client = _factory.BuildSocket();
                    Client.UseConnectedSocket(pNewSocket);
                }
                catch (Exception e)
                {
                    LOGGER.Error(e, "Une erreur est survenue durant la prisez en charge d'une nouvelle connexion provenant de {0}", pNewSocket?.Client?.RemoteEndPoint);
                    pNewSocket.Close();
                    Client = null;
                }
            }
        }

        private void OnConnectionChangedHandler(object sender, EventArgs e)
        {
            if (Client != null && !Client.IsConnected)
            {
                Client = null;
            }
        }

        private void OnMessageReceivedHandler(object sender, DevMessageReceivedEventArg e)
        {
            DispatchMessage(sender as IDevSocket, e.MessageReceived);
        }
    }
}
