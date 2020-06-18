using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Filters.Events;
using Filters.Models;
using Filters.Views;
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
        public DelegateCommand ClosingCommand { get; set; }
        
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
        private Tag _lastTag;

        public FiltersViewModel(IEventAggregator eventAggregator)
        {
            AddFilterCommand = new DelegateCommand(AddFilterAction);
            RemoveFilterCommand = new DelegateCommand(RemoveFilterAction);
            MoveFilterDownCommand = new DelegateCommand(MoveFilterDownAction, CanMoveFilterDown);
            MoveFilterUpCommand = new DelegateCommand(MoveFilterUpAction, CanMoveFilterUp);
            ClosingCommand = new DelegateCommand(ClosingAction);

            FiltersInUse = new ObservableCollection<IFilter>();

            _lastTag = new Tag("", new List<TimeCoordinate>());

            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<FilterSelectionEvent>().Subscribe(AddFilterToCollection, ThreadOption.UIThread, false);
            _eventAggregator.GetEvent<TagSelectionEvent>().Subscribe(ApplyFilters, ThreadOption.UIThread);

        }

        private void AddFilterAction()
        {
            _addFilterWindow = new AddFilterWindow
            {
                DataContext = new AddFilterViewModel(_eventAggregator),
                WindowStartupLocation = WindowStartupLocation.Manual
            };

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

        private void AddFilterToCollection(IFilter filter)
        {
            FiltersInUse.Add(filter);
        }

        private async void ApplyFilters(Tag tag)
        {
            if(tag == null) return;
            
            _lastTag = tag;

            var tempData = tag.TimeCoordinates;

            foreach (var filter in FiltersInUse)
            {
                tempData = await filter.Filter(tempData);
            }

            _eventAggregator.GetEvent<TagFilterEvent>().Publish(new Tag(tag.Id, tempData));
        }

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }   

        private void ClosingAction()
        {
            ApplyFilters(_lastTag);
        }
    }
}
