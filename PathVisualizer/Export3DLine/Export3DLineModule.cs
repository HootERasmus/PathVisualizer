using Export3DLine.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Export3DLine
{
    public class Export3DLineModule : IModule
    {
        public Export3DLineModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.MenuRegionFile, typeof(Export3DLineView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<Export3DLineView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<Export3DLineView>();
        }
    }
}