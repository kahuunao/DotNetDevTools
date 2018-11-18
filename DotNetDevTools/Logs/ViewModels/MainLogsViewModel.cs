using DevToolsMessage.Request;

using Logs.Services;

using Prism.Mvvm;

using System.Collections.ObjectModel;

namespace Logs.ViewModels
{
    public class MainLogsViewModel : BindableBase
    {
        private ObservableCollection<DevLogLine> _logs;
        public ObservableCollection<DevLogLine> Logs
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
