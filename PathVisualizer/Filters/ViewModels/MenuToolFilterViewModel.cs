using Prism.Commands;
using Prism.Mvvm;
using Filters.Views;
using Pipeline;
using Prism.Events;

namespace Filters.ViewModels
{
    public class MenuToolFilterViewModel : BindableBase
    {
        public DelegateCommand OpenFilterWindowCommand { get; set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly FiltersViewModel _filtersViewModel;

        public MenuToolFilterViewModel(IEventAggregator eventAggregator, IPipeline pipeline)
        {
            OpenFilterWindowCommand = new DelegateCommand(OpenFilterWindowAction);
            
            _eventAggregator = eventAggregator;
            _filtersViewModel = new FiltersViewModel(_eventAggregator, pipeline);
        }

        private void OpenFilterWindowAction()
        {
            var window = new FiltersWindow {DataContext = _filtersViewModel};
            window.Show();
        }
    }
}
