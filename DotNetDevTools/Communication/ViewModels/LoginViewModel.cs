using DevToolsClientCore.Socket;

using DevToolsConnector;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Windows.Input;

namespace Communication.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly IDevRequestService _communication;

        private Uri _remote;
        public Uri Remote
        {
            get { return _remote; }
            set { SetProperty(ref _remote, value); }
        }

        private string _state;
        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        public ICommand LoginCommand => new DelegateCommand(Login);

        public LoginViewModel(IDevRequestService pCom)
        {
            _communication = pCom;
            _communication.OnStateChanged += OnStateChangedHandler;

            Remote = new Uri("ws://localhost:12000");
            State = _communication.State;
        }

        private void Login()
        {
            _communication.StartAsync(Remote).RunSafe();
        }

        private void OnStateChangedHandler(object sender, EventArgs e)
        {
            State = _communication.State;
        }
    }
}
