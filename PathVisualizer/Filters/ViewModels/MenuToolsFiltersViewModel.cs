using Prism.Commands;
using Prism.Mvvm;
using Filters.Views;
using Prism.Events;

namespace Filters.ViewModels
{
    public class MenuToolsFiltersViewModel : BindableBase
    {
        public DelegateCommand OpenFilterWindowCommand { get; set; }

        private readonly IEventAggregator _eventAggregator;
        private readonly FiltersViewModel _filtersViewModel;

        public MenuToolsFiltersViewModel(IEventAggregator eventAggregator)
        {
            OpenFilterWindowCommand = new DelegateCommand(OpenFilterWindowAction);
            
            _eventAggregator = eventAggregator;
            _filtersViewModel = new FiltersViewModel(_eventAggregator);
        }

        private void OpenFilterWindowAction()
        {
            var window = new FiltersWindow {DataContext = _filtersViewModel};
            window.Show();
        }
    }
}
