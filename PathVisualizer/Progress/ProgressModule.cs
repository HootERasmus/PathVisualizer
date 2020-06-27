using Progress.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Progress
{
    public class ProgressModule : IModule
    {
        public ProgressModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.ProgressRegion, typeof(ProgressView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ProgressView>();
        }
    }
}