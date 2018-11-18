
using DevToolsConnector.Common;

using DevToolsMessage;
using DevToolsMessage.Request;

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

            // Demande d'informations de l'application distante
            var remoteApp = await Identification();

            OnConnectChanged?.Invoke(this, EventArgs.Empty);
            return Socket.IsConnected;
        }

        public override void Close()
        {
            if (Socket != null)
            {
                Socket.Close();
                Socket = null;
            }
            OnConnectChanged?.Invoke(this, EventArgs.Empty);
        }

        public Task<DevMessage> SendMessage(DevMessage pRequest)
        {
            return new DevTransaction(Socket, pRequest).Send();
        }

        private Task<DevMessage> Identification()
        {
            return SendMessage(new DevMessage
            {
                RequestType = EnumDevMessageType.IDENTIFICATION,
                Request = new DevRequest
                {
                    Identification = new DevIdentificationRequest()
                }
            });
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
