using DataPanel.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DataPanel
{
    public class DataGridLoaderModule : IModule
    {
        public DataGridLoaderModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.DataRegion, typeof(DataGridView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<DataGridView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<DataGridView>();
        }
    }
}