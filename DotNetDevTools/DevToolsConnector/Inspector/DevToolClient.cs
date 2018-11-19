
using DevToolsConnector.Common;

using DevToolsMessage;

using NLog;

using System;
using System.Threading.Tasks;

namespace DevToolsConnector.Inspector
{
    public class DevToolClient : AbstractDevTool, IDevToolClient
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Changement d'état de la connexion
        /// </summary>
        public event EventHandler OnConnectChanged;

        /// <summary>
        /// Indique si la connexion est établie
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return Socket != null && Socket.IsConnected;
            }
        }

        /// <summary>
        /// Constructeur de socket
        /// </summary>
        private readonly IDevSocketFactory _factory;

        /// <summary>
        /// Socket de communication
        /// </summary>
        public IDevSocket Socket { get; private set; }

        public DevToolClient(IDevSocketFactory pFactory)
        {
            _factory = pFactory;
        }

        public async Task<bool> Connect(Uri pRemote)
        {
            // Fermeture des précédents connexions
            Close();

            Socket = _factory.BuildSocket();
            Socket.OnConnectionChanged += OnConnectionChangedHandler;
            Socket.OnMessageReceived += OnMessageReceivedHandler;
            var isConnected = await Socket.Connect(pRemote);
            return Socket.IsConnected;
        }

        public override void Close()
        {
            if (Socket != null)
            {
                Socket.OnConnectionChanged -= OnConnectionChangedHandler;
                Socket.OnMessageReceived -= OnMessageReceivedHandler;
                Socket.Close();
                Socket = null;

                // Force le dispatch de la déconnexion
                OnConnectChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public Task<IDevMessage> SendMessage(IDevMessage pRequest)
        {
            return new DevTransaction(Socket, pRequest).Send();
        }

        private void OnConnectionChangedHandler(object sender, EventArgs e)
        {
            OnConnectChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnMessageReceivedHandler(object sender, DevMessageReceivedEventArg e)
        {
            DispatchMessage(Socket, e.MessageReceived);
        }
    }
}
