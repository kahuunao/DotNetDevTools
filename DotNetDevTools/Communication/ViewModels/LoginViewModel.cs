using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Communication.ViewModels
{
    public class LoginViewModel : BindableBase
    {
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

        public LoginViewModel()
        {
            Remote = new Uri("localhost:12000");
            State = "En attente de connexion ...";
        }

        private void Login()
        {
            
        }
    }
}
