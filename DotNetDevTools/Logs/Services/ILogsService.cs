using DevToolsTestMessage;

using System.Collections.ObjectModel;

namespace Logs.Services
{
    public interface ILogsService
    {
        ObservableCollection<Log> Logs { get; }
    }
}
