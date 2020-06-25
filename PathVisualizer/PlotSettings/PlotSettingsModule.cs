using PlotSettings.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace PlotSettings
{
    public class PlotSettingsModule : IModule
    {
        public PlotSettingsModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.MenuRegionView, typeof(MenuViewPlotSettingsView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<MenuViewPlotSettingsView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<MenuViewPlotSettingsView>();
        }
    }
}