using Prism.Commands;
using Prism.Events;
using ZonePlot.Views;

namespace ZonePlot.ViewModels
{
    public class MenuToolZoneViewModel
    {
        public DelegateCommand ManageZonesCommand { get; set; }
        private readonly IEventAggregator _eventAggregator;

        public MenuToolZoneViewModel(IEventAggregator eventAggregator)
        {
            ManageZonesCommand = new DelegateCommand(ManageZonesAction);
            _eventAggregator = eventAggregator;
        }

        private void ManageZonesAction()
        {
            var window = new ManageZonesWindow
            {
                DataContext = new ManageZonesViewModel(_eventAggregator)
            };

            window.Show();
        }
    }
}
