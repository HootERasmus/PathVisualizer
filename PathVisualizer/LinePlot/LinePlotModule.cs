using LinePlot.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace LinePlot
{
    public class LinePlotModule : IModule
    {
        public LinePlotModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.PlotRegion, typeof(LinePlotView));
            regionManager.RegisterViewWithRegion(Lib.RegionNames.BannerLineRegion, typeof(BannerNavigationView));
            regionManager.RegisterViewWithRegion(Lib.RegionNames.MenuRegionFileExport, typeof(MenuFileExportView));
            regionManager.RegisterViewWithRegion(Lib.RegionNames.MenuRegionFileExport, typeof(MenuFileExportMultiLinePlotView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<LinePlotView>();
            containerRegistry.Register<BannerNavigationView>();
            containerRegistry.Register<MenuFileExportView>();
            containerRegistry.Register<MenuFileExportMultiLinePlotView>();
        }
    }
}