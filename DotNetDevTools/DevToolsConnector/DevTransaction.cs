using DevToolsMessage;

using NLog;

using System;
using System.Threading.Tasks;

namespace DevToolsConnector
{
    /// <summary>
    /// Réalise un échange requête - response pouvant être attendue
    /// </summary>
    public class DevTransaction
    {
        private readonly static Logger LOGGER = LogManager.GetCurrentClassLogger();

        private IDevSocket _socket;
        private DevMessage _request;
        private TaskCompletionSource<DevMessage> _response;

        public DevTransaction(IDevSocket pSocket, DevMessage pRequest)
        {
            _socket = pSocket;
            _socket.OnConnectionChanged += OnConnectionChanged;
            _socket.OnMessageReceived += OnMessageReceived;
            _request = pRequest;
            _response = new TaskCompletionSource<DevMessage>();
        }

        public async Task<DevMessage> Send()
        {
            DevMessage result = null;
            try
            {
                await _socket.Send(_request);
                result = await _response.Task;
            }
            catch(TaskCanceledException)
            {
                LOGGER.Warn("Annulation de l'échange");
                _response.TrySetCanceled();
            }
            catch (Exception e)
            {
                LOGGER.Error(e);
                _response.TrySetException(e);
            }
            return result;
        }

        private bool IsSameTransaction(DevMessage pRequest, DevMessage pResponse)
        {
            return pRequest != null && pResponse != null && pRequest.Id == pResponse.Id;
        }

        private void OnMessageReceived(object sender, DevMessageReceivedEventArg e)
        {
            if (IsSameTransaction(_request, e.MessageReceived))
            {
                _response.SetResult(e.MessageReceived);
            }
        }

        private void OnConnectionChanged(object sender, EventArgs e)
        {
            if (!_socket.IsConnected)
            {
                switch (_response.Task.Status)
                {
                    case TaskStatus.Created:
                    case TaskStatus.Running:
                    case TaskStatus.WaitingForActivation:
                    case TaskStatus.WaitingForChildrenToComplete:
                    case TaskStatus.WaitingToRun:
                        LOGGER.Warn("Annulation de l'échange");
                        _response.TrySetCanceled();
                        break;
                    default:
                        // OSEF
                        break;
                }
            }
        }
    }
}
