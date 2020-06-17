using DataLoader.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DataLoader
{
    public class DataLoaderModule : IModule
    {
        public DataLoaderModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.MenuRegionFile, typeof(MenuFileOpenView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<MenuFileOpenView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<MenuFileOpenView>();
        }
    }
}