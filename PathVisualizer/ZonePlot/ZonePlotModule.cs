using ZonePlot.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ZonePlot
{
    public class ZonePlotModule : IModule
    {
        public ZonePlotModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.PlotRegion, typeof(ZonePlotView));
            regionManager.RegisterViewWithRegion(Lib.RegionNames.BannerLineRegion, typeof(BannerNavigationView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<ZonePlotView>();
            containerProvider.Resolve<BannerNavigationView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ZonePlotView>();
            containerRegistry.Register<BannerNavigationView>();
        }
    }
}