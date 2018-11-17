using Communication;
using DevToolsClient.Views;
using DevToolsClientCore.Socket;
using Logs;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace DevToolsClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ICommunicationService, CommunicationService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<LogsModule>();
            moduleCatalog.AddModule<CommunicationModule>();
        }
    }
}
