﻿using DevToolsConnector.Common;
using DevToolsConnector.Inspector;

using DevToolsMessage;
using DevToolsTestMessage.Request;
using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Communication.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly IDevToolClient _devTool;

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

        public LoginViewModel(IDevToolClient pCom)
        {
            _devTool = pCom;
            _devTool.OnConnectChanged += OnConnectChanged;

            Remote = new Uri("tcp://localhost:12000");
            State = (_devTool?.Socket?.IsConnected ?? false) ? "Connecté" : "Non Connecté";
        }

        private void Login()
        {
            Start().RunSafe();
        }

        private void OnConnectChanged(object sender, EventArgs e)
        {
            State = (_devTool?.Socket?.IsConnected ?? false) ? "Connecté" : "Non Connecté";
        }

        private async Task Start()
        {
            await _devTool.Connect(Remote);
        }
    }
}
