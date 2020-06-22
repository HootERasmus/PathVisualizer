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
            regionManager.RegisterViewWithRegion(Lib.RegionNames.PipelineRegion, typeof(ZonePipelineView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<ZonePlotView>();
            containerProvider.Resolve<BannerNavigationView>();
            containerProvider.Resolve<MenuToolZoneView>();
            containerProvider.Resolve<ZonePipelineView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ZonePlotView>();
            containerRegistry.Register<BannerNavigationView>();
            containerRegistry.Register<MenuToolZoneView>();
            containerRegistry.Register<ZonePipelineView>();

        }
    }
}