using DevToolsConnector.Common;
using DevToolsConnector.Inspector;

using DevToolsMessage;

using DevToolsTestMessage;
using DevToolsTestMessage.Request;

using NLog;

using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Logs.Services
{
    public class LogsService : ILogsService
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public ObservableCollection<Log> Logs { get; private set; } = new ObservableCollection<Log>();

        private readonly IDevToolClient _client;

        public LogsService(IDevToolClient pClient)
        {
            _client = pClient;
            _client.RegisterListener<DevLogsRequest>(OnLogReceived); // FIXME
            _client.OnConnectChanged += OnConnectChangedHandler;
            RequestLogs().RunSafe();
        }

        private void OnLogReceived(IDevSocket pSocket, IDevMessage pMessage)
        {
            if (pMessage is DevLogsRequest request)
            {
                LOGGER.Debug("Réception de logs");
                if (request.Logs != null)
                {
                    Logs.AddRange(request.Logs);
                }
                pSocket.RespondAt(request);
            }
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
                await _client.SendMessage(new DevStartSendLogsRequest());
            }
        }
    }
}
