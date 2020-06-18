using System.Collections.ObjectModel;
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

        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        private readonly IEventAggregator _eventAggregator;

        public AddFilterViewModel(IEventAggregator eventAggregator)
        {
            Filters = new ObservableCollection<IFilter> {new MovingAverageFilterModel(), new RemoveDeltaFilterModel()};

            AddCommand = new DelegateCommand(AddAction, CanAddAction);
            CancelCommand = new DelegateCommand(CancelAction);

            _eventAggregator = eventAggregator;
        }

        private void AddAction()
        {
            _eventAggregator.GetEvent<FilterEvent>().Publish(SelectedFilter);
            CancelCommand.Execute();
        }

        private bool CanAddAction()
        {
            return SelectedFilter != null;
        }

        private void CancelAction()
        {
            _eventAggregator.GetEvent<OperationEvent>().Publish(new OperationEventModel(nameof(AddFilterWindow), OperationType.Close));
        }
    }
}
