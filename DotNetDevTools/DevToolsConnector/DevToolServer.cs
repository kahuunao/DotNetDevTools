using NLog;

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DevToolsConnector
{
    /// <summary>
    /// Server de connexion pour les outils de développeur
    /// </summary>
    public class DevToolServer : IDevToolServer
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        private TcpListener _server;
        private List<DevSocket> _sockets = new List<DevSocket>();
        private IDevRequestHandler _handler;

        /// <summary>
        /// Initialisation du server
        /// </summary>
        /// <param name="pHandler"></param>
        /// <param name="pPort"></param>
        public void Init(IDevRequestHandler pHandler, int? pPort = null)
        {
            Close();

            _handler = pHandler;
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
                var s = new DevSocket(_handler);
                s.UseConnectedSocket(newClient);
                _sockets.Add(s);
            }
        }

        /// <summary>
        /// Ferme toutes les connexions
        /// </summary>
        public void Close()
        {
            LOGGER.Debug("Fermeture des connexions dev");
            _sockets.ForEach((s) => s?.Stop());
            _sockets.Clear();

            _server?.Stop();
            _server = null;
        }
    }
}
