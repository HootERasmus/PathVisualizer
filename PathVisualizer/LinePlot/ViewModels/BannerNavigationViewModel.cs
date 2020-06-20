using System;
using System.Collections.Generic;
using System.Text;
using Lib;
using Prism.Regions;

namespace LinePlot.ViewModels
{
    public class BannerNavigationViewModel : Navigation
    {
        public BannerNavigationViewModel(IRegionManager regionManager) : base(regionManager)
        {
            NavigationView = new Uri("/LinePlotView", UriKind.Relative);
            ButtonContent = "Line plot";
        }

        public sealed override string ButtonContent { get; set; }
        public sealed override Uri NavigationView { get; set; }
        public override void NavigateAction()
        {
            RegionManager.RequestNavigate(RegionNames.BannerLineRegion, NavigationView);
        }
    }
}
