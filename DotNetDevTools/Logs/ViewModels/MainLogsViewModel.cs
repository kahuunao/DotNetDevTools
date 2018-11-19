using DevToolsTestMessage;

using Logs.Services;

using Prism.Mvvm;

using System.Collections.ObjectModel;

namespace Logs.ViewModels
{
    public class MainLogsViewModel : BindableBase
    {
        private ObservableCollection<Log> _logs;
        public ObservableCollection<Log> Logs
        {
            get { return _logs; }
            set { SetProperty(ref _logs, value); }
        }

        public MainLogsViewModel(ILogsService pLogsService)
        {
            Logs = pLogsService.Logs;
        }
    }
}
