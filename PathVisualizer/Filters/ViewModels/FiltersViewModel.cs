using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Filters.Events;
using Filters.Models;
using Filters.Views;
using Lib.Events;
using Lib.SharedModels;
using Pipeline;
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
        private readonly IPipeline _pipeline;
        private AddFilterWindow _addFilterWindow;
        private readonly AddFilterViewModel _addFilterViewModel;
        private Tag _lastTag;

        public FiltersViewModel(IEventAggregator eventAggregator, IPipeline pipeline)
        {
            AddFilterCommand = new DelegateCommand(AddFilterAction);
            RemoveFilterCommand = new DelegateCommand(RemoveFilterAction);
            MoveFilterDownCommand = new DelegateCommand(MoveFilterDownAction, CanMoveFilterDown);
            MoveFilterUpCommand = new DelegateCommand(MoveFilterUpAction, CanMoveFilterUp);
            ClosingCommand = new DelegateCommand(ClosingAction);

            FiltersInUse = new ObservableCollection<IFilter>();

            _addFilterViewModel = new AddFilterViewModel(eventAggregator);

            _lastTag = new Tag("", new List<TimeCoordinate>());

            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<FilterSelectionEvent>().Subscribe(AddFilterToCollection, ThreadOption.UIThread, false);

            _pipeline = pipeline;
            _pipeline.AddActionToPipe(ApplyFilters,2);
            LoadFilters();

        }

        private void AddFilterAction()
        {
            _addFilterWindow = new AddFilterWindow
            {
                DataContext = _addFilterViewModel,
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

        private Task<Tag> ApplyFilters(Tag tag)
        {
            if(tag == null) return null;
            
            _lastTag = tag;

            return Task.Run(async () =>
            {
                var tempData = tag.TimeCoordinates;

                foreach (var filter in FiltersInUse)
                {
                    tempData = await filter.Filter(tempData);
                }

                return new Tag(tag.Id, tempData);
            });
        }

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        private void SaveFilters()
        {
            var sb = new StringBuilder();

            foreach (var filter in FiltersInUse)
            {
                sb.Append($"{filter.Name};");
            }

            sb.Remove(sb.Length -1, 1);

            UserSettings.Default.Filters = sb.ToString();
            UserSettings.Default.Save();
        }

        private void LoadFilters()
        {
            var filtersString = UserSettings.Default.Filters;

            var split = filtersString.Split(';');

            foreach (var item in split)
            {
                var filter = _addFilterViewModel.Filters.FirstOrDefault(x => x.Name == item);
                if (filter != null)
                    FiltersInUse.Add(filter);
            }
        }

        private void ClosingAction()
        {
            SaveFilters();
            _eventAggregator.GetEvent<PipeLineStartEvent>().Publish(new PipelineStartEventModel(this, _lastTag));
        }
    }
}
