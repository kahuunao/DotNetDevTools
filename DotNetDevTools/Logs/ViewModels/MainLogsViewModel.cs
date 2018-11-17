using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logs.ViewModels
{
    public class MainLogsViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public MainLogsViewModel()
        {
            Message = "Vue des logs";
        }
    }
}
