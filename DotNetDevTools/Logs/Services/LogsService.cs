using DevToolsConnector;

using DevToolsMessage;
using DevToolsMessage.Request;

using NLog;

using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Logs.Services
{
    public class LogsService : ILogsService
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public ObservableCollection<DevLogLine> Logs { get; private set; } = new ObservableCollection<DevLogLine>();

        private readonly IDevToolClient _client;

        public LogsService(IDevToolClient pClient)
        {
            _client = pClient;
            _client.RegisterListener(EnumDevMessageType.LOG_LINE, OnLogReceived);
            _client.OnConnectChanged += OnConnectChangedHandler;
            RequestLogs().RunSafe();
        }

        private void OnLogReceived(IDevSocket pSocket, DevMessage pMessage)
        {
            LOGGER.Debug("Réception de logs");
            if (pMessage != null && pMessage.Request != null && pMessage.Request.LogLine != null)
            {
                Logs.AddRange(pMessage.Request.LogLine);
            }
            pSocket.RespondAt(pMessage);
        }

        private void OnConnectChangedHandler(object sender, System.EventArgs e)
        {
            RequestLogs().RunSafe();
        }

        private async Task RequestLogs()
        {
            if (_client.IsConnected)
            {
                LOGGER.Debug("Envoi de la configuration des logs attendues");
                await _client.SendMessage(new DevMessage
                {
                    RequestType = EnumDevMessageType.SET_LOG_CONFIG
                });
            }
        }
    }
}
