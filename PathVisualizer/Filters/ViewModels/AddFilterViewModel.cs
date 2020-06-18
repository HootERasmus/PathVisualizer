using System.Collections.ObjectModel;
using System.Windows;
using Filters.Events;
using Filters.Models;
using Filters.Views;
using Lib;
using Lib.Events;
using Lib.SharedModels;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Filters.ViewModels
{
    public class AddFilterViewModel : BindableBase
    {
        public ObservableCollection<IFilter> Filters { get; set; }
        
        private IFilter _selectedFilter;
        public IFilter SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (value == SelectedFilter) return;

                _selectedFilter = value;
                RaisePropertyChanged();
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand<Window> AddCommand { get; set; }
        public DelegateCommand<Window> CancelCommand { get; set; }

        private readonly IEventAggregator _eventAggregator;

        public AddFilterViewModel(IEventAggregator eventAggregator)
        {
            Filters = new ObservableCollection<IFilter> {new MovingAverageFilterModel(), new RemoveDeltaFilterModel()};

            AddCommand = new DelegateCommand<Window>(AddAction, CanAddAction);
            CancelCommand = new DelegateCommand<Window>(CancelAction);

            _eventAggregator = eventAggregator;
        }

        private void AddAction(Window window)
        {
            _eventAggregator.GetEvent<FilterSelectionEvent>().Publish(SelectedFilter);
            CancelCommand.Execute(window);
        }

        private bool CanAddAction(Window window)
        {
            return SelectedFilter != null;
        }

        private void CancelAction(Window window)
        {
            window.Close();
        }
    }
}
