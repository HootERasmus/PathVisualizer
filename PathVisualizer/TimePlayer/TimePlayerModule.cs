using TimePlayer.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace TimePlayer
{
    public class TimePlayerModule : IModule
    {
        public TimePlayerModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.BannerLineRegion, typeof(BannerNavigationView));
            regionManager.RegisterViewWithRegion(Lib.RegionNames.PlotRegion, typeof(TimePlayerView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
           
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<BannerNavigationView>();
            containerRegistry.Register<TimePlayerView>();
        }
    }
}