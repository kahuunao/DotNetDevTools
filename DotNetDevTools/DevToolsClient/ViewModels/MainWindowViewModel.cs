using Prism.Mvvm;

namespace DevToolsClient.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Outils développeur d'application .Net";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {
            
        }
    }
}
