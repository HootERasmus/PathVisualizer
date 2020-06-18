using System.Collections.Generic;
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
    public class FiltersViewModel : BindableBase
    {
        public DelegateCommand AddFilterCommand { get; set; }
        public DelegateCommand RemoveFilterCommand { get; set; }
        public DelegateCommand MoveFilterDownCommand { get; set; }
        public DelegateCommand MoveFilterUpCommand { get; set; }
        
        public ObservableCollection<IFilter> FiltersInUse { get; set; }

        private IFilter _selectedFilter;
        public IFilter SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if(value == SelectedFilter) return;

                _selectedFilter = value;
                RaisePropertyChanged();
                MoveFilterUpCommand.RaiseCanExecuteChanged();
                MoveFilterDownCommand.RaiseCanExecuteChanged();
            }
        }

        private readonly IEventAggregator _eventAggregator;
        private AddFilterWindow _addFilterWindow;

        public FiltersViewModel(IEventAggregator eventAggregator)
        {
            AddFilterCommand = new DelegateCommand(AddFilterAction);
            RemoveFilterCommand = new DelegateCommand(RemoveFilterAction);
            MoveFilterDownCommand = new DelegateCommand(MoveFilterDownAction, CanMoveFilterDown);
            MoveFilterUpCommand = new DelegateCommand(MoveFilterUpAction, CanMoveFilterUp);
        
            FiltersInUse = new ObservableCollection<IFilter>();

            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<OperationEvent>().Subscribe(CloseAddFilterWindow, ThreadOption.UIThread, false,
                x => x.ViewName == nameof(AddFilterWindow) && x.OperationType == OperationType.Close);

            _eventAggregator.GetEvent<FilterEvent>().Subscribe(AddFilterToCollection, ThreadOption.UIThread, false);
        }

        private void AddFilterAction()
        {
            _addFilterWindow = new AddFilterWindow {DataContext = new AddFilterViewModel(_eventAggregator)};

            _addFilterWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            _addFilterWindow.Show();
        }

        private void RemoveFilterAction()
        {
            FiltersInUse.Remove(SelectedFilter);
        }

        private void MoveFilterDownAction()
        {
            var index = FiltersInUse.IndexOf(SelectedFilter);
            Swap(FiltersInUse, index, index+1);
        }

        private bool CanMoveFilterDown()
        {
            return FiltersInUse.IndexOf(SelectedFilter) < FiltersInUse.Count - 1;
        }

        private void MoveFilterUpAction()
        {
            var index = FiltersInUse.IndexOf(SelectedFilter);
            Swap(FiltersInUse, index, index - 1);
        }

        private bool CanMoveFilterUp()
        {
            return FiltersInUse.IndexOf(SelectedFilter) > 0;
        }

        private void CloseAddFilterWindow(OperationEventModel model)
        {
            _addFilterWindow.Close();
        }

        private void AddFilterToCollection(IFilter filter)
        {
            FiltersInUse.Add(filter);
        }

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
    }
}
