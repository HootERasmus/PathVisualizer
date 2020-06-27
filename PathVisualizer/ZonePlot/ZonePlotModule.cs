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
            regionManager.RegisterViewWithRegion(Lib.RegionNames.MenuRegionTool, typeof(MenuToolZoneView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ZonePlotView>();
            containerRegistry.Register<BannerNavigationView>();
            containerRegistry.Register<MenuToolZoneView>();
        }
    }
}