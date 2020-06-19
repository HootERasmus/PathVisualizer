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
            
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<LinePlotView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<LinePlotView>();
        }
    }
}