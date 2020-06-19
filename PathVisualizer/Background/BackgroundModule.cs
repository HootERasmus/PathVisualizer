using Background.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Background
{
    public class BackgroundModule : IModule
    {
        public BackgroundModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.BackgroundRegion, typeof(BackgroundView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<BackgroundView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<BackgroundView>();
        }
    }
}