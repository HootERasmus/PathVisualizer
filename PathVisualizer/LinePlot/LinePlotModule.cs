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
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<LinePlotView>();
            containerProvider.Resolve<BannerNavigationView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<LinePlotView>();
            containerRegistry.Register<BannerNavigationView>();
        }
    }
}