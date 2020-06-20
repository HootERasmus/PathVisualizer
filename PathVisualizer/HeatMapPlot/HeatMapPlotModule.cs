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
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<BannerNavigationView>();
            containerProvider.Resolve<HeatMapPlotView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<BannerNavigationView>();
            containerRegistry.Register<HeatMapPlotView>();
        }
    }
}