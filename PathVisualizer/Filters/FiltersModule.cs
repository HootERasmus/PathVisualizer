using Filters.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Filters
{
    public class FiltersModule : IModule
    {
        public FiltersModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.MenuRegionTool, typeof(MenuToolFilterView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<MenuToolFilterView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<MenuToolFilterView>();
        }
    }
}