using Logs.Services;
using Logs.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Logs
{
    public class LogsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("LogsRegion", typeof(MainLogs));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILogsService, LogsService>();
        }
    }
}