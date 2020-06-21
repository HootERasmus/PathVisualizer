using Pipeline.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Pipeline
{
    public class PipelineModule : IModule
    {
        public PipelineModule(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(Lib.RegionNames.PipelineRegion, typeof(PipelineView));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<PipelineView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<PipelineView>();
        }
    }
}