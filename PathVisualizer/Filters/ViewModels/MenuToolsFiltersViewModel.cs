using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Filters.Views;
using Prism.Events;

namespace Filters.ViewModels
{
    public class MenuToolsFiltersViewModel : BindableBase
    {
        public DelegateCommand OpenFilterWindowCommand { get; set; }

        private readonly IEventAggregator _eventAggregator;

        public MenuToolsFiltersViewModel(IEventAggregator eventAggregator)
        {
            OpenFilterWindowCommand = new DelegateCommand(OpenFilterWindowAction);
            _eventAggregator = eventAggregator;
        }

        private void OpenFilterWindowAction()
        {
            var window = new FiltersWindow {DataContext = new FiltersViewModel(_eventAggregator)};
            window.Show();
        }
    }
}
