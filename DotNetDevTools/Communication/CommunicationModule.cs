using Communication.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Communication
{
    public class CommunicationModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("CommunicationRegion", typeof(Login));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}