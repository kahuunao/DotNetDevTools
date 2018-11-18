using DevToolsMessage;
using DevToolsMessage.Response;

using NLog;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DevToolsConnector.Impl
{
    /// <summary>
    /// Server de connexion pour les outils de développeur
    /// </summary>
    public class DevToolServer : AbstractDevTool, IDevToolServer
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        private TcpListener _server;
        private List<IDevSocket> _sockets = new List<IDevSocket>();
        private IDevSocketFactory _factory;

        public DevToolServer(IDevSocketFactory pFactory)
        {
            _factory = pFactory;
            RegisterListener(EnumDevMessageType.IDENTIFICATION, IdentificationRequestHandler);
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
                IDevSocket s = _factory.BuildSocket();
                s.UseConnectedSocket(newClient);
                s.OnMessageReceived += (sender, e) => DispatchMessage(sender as IDevSocket, e.MessageReceived);
                _sockets.Add(s);
            }
        }

        /// <summary>
        /// Ferme toutes les connexions
        /// </summary>
        public override void Close()
        {
            LOGGER.Debug("Fermeture des connexions dev");
            _sockets.ForEach((s) => s?.Close());
            _sockets.Clear();

            _server?.Stop();
            _server = null;
        }

        private void IdentificationRequestHandler(IDevSocket pSocket, DevMessage pMessage)
        {
            pSocket.RespondAt(pMessage, new DevResponse
            {
                Identification = new DevIdentificationResponse
                {
                    AppName = AppDomain.CurrentDomain.FriendlyName
                }
            });
        }
    }
}
