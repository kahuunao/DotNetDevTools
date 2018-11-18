using Communication;
using DevToolsClient.Views;
using DevToolsClientCore.Socket;
using DevToolsConnector;
using DevToolsConnector.Impl;
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
            containerRegistry.Register<IDevRequestService, DevRequestService>();
            containerRegistry.Register<IDevRequestHandler, DevRequestHandler>();
            containerRegistry.Register<IDevSocketFactory, DevSocketFactory>();
            containerRegistry.Register<IDevMessageSerializer, NewtonsoftSerializer>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<LogsModule>();
            moduleCatalog.AddModule<CommunicationModule>();
        }
    }
}
