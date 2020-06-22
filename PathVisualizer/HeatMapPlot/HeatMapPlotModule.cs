using HeatMapPlot.ViewModels;
using HeatMapPlot.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace HeatMapPlot
{
    public class HeatMapPlotModule : IModule
    {
        public HeatMapPlotModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.BannerLineRegion, typeof(BannerNavigationView));
            regionManager.RegisterViewWithRegion(Lib.RegionNames.PlotRegion, typeof(HeatMapPlotView));
            regionManager.RegisterViewWithRegion(Lib.RegionNames.MenuRegionFileExport, typeof(MenuFileExportHeatMapPlotView));
            regionManager.RegisterViewWithRegion(Lib.RegionNames.MenuRegionFileExport, typeof(MenuFileExportMultiHeatMapPlotView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<BannerNavigationView>();
            containerProvider.Resolve<HeatMapPlotView>();
            containerProvider.Resolve<MenuFileExportHeatMapPlotView>();
            containerProvider.Resolve<MenuFileExportMultiHeatMapPlotViewModel>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<BannerNavigationView>();
            containerRegistry.Register<HeatMapPlotView>();
            containerRegistry.Register<MenuFileExportHeatMapPlotView>();
            containerRegistry.Register<MenuFileExportMultiHeatMapPlotView>();
        }
    }
}