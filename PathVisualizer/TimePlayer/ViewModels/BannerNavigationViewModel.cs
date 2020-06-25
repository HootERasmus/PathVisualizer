using System;
using Lib;
using Prism.Regions;

namespace TimePlayer.ViewModels
{
    public class BannerNavigationViewModel : Navigation
    {
        public BannerNavigationViewModel(IRegionManager regionManager) : base(regionManager)
        {
            ButtonContent = "Time player";
            NavigationView = new Uri("/TimePlayerView", UriKind.Relative);
        }

        public sealed override string ButtonContent { get; set; }
        public sealed override Uri NavigationView { get; set; }
        public override void NavigateAction()
        {
            RegionManager.RequestNavigate(Lib.RegionNames.PlotRegion, NavigationView);
        }
    }
}
