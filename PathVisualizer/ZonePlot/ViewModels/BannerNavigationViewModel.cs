using System;
using Lib;
using Prism.Regions;

namespace ZonePlot.ViewModels
{
    public class BannerNavigationViewModel : Navigation
    {
        public BannerNavigationViewModel(IRegionManager regionManager) : base(regionManager)
        {
            ButtonContent = "Zone plot";
            NavigationView = new Uri("/ZonePlotView", UriKind.Relative);
        }

        public sealed override string ButtonContent { get; set; }
        public sealed override Uri NavigationView { get; set; }
        public override void NavigateAction()
        {
            RegionManager.RequestNavigate(RegionNames.PlotRegion, NavigationView);
        }
    }
}
