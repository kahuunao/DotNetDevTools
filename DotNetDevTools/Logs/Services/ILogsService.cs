using DevToolsMessage.Request;

using System.Collections.ObjectModel;

namespace Logs.Services
{
    public interface ILogsService
    {
        ObservableCollection<DevLogLine> Logs { get; }
    }
}
