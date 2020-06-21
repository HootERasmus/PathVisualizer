﻿using LinePlot.Views;
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
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<LinePlotView>();
            containerProvider.Resolve<BannerNavigationView>();
            containerProvider.Resolve<MenuFileExportView>();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<LinePlotView>();
            containerRegistry.Register<BannerNavigationView>();
            containerRegistry.Register<MenuFileExportView>();
        }
    }
}