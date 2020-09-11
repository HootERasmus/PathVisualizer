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
using PipelineService;
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
        private readonly List<IFilter> _removedFilters;

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

        private FrameworkElement _contentControlView;
        public FrameworkElement ContentControlView
        {
            get => _contentControlView;
            set
            {
                _contentControlView = value;
                RaisePropertyChanged();
            }
        }

        public FiltersViewModel(IEventAggregator eventAggregator, IPipeline pipeline)
        {
            AddFilterCommand = new DelegateCommand(AddFilterAction);
            RemoveFilterCommand = new DelegateCommand(RemoveFilterAction);
            MoveFilterDownCommand = new DelegateCommand(MoveFilterDownAction, CanMoveFilterDown);
            MoveFilterUpCommand = new DelegateCommand(MoveFilterUpAction, CanMoveFilterUp);
            ClosingCommand = new DelegateCommand(ClosingAction);

            FiltersInUse = new ObservableCollection<IFilter>();

            _addFilterViewModel = new AddFilterViewModel(eventAggregator);

            _lastTag = new Tag("", new List<ITimeCoordinate>());

            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<FilterSelectionEvent>().Subscribe(AddFilterToCollection);
            _eventAggregator.GetEvent<TagSelectionEvent>().Subscribe(tag => _lastTag = tag);

            _pipeline = pipeline;
            _removedFilters = new List<IFilter>();
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
            _removedFilters.Add(SelectedFilter);
            FiltersInUse.Remove(SelectedFilter);
        }

        private void MoveFilterDownAction()
        {
            var index = FiltersInUse.IndexOf(SelectedFilter);
            Swap(FiltersInUse, index, index+1);

            RemoveFiltersFromPipeline(new List<IFilter>
            {
                FiltersInUse[index],
                FiltersInUse[index+1]
            });
        }

        private bool CanMoveFilterDown()
        {
            return FiltersInUse.IndexOf(SelectedFilter) < FiltersInUse.Count - 1;
        }

        private void MoveFilterUpAction()
        {
            var index = FiltersInUse.IndexOf(SelectedFilter);
            Swap(FiltersInUse, index, index - 1);

            RemoveFiltersFromPipeline(new List<IFilter>
            {
                FiltersInUse[index],
                FiltersInUse[index-1]
            });
        }

        private bool CanMoveFilterUp()
        {
            return FiltersInUse.IndexOf(SelectedFilter) > 0;
        }

        private void AddFilterToCollection(IFilter filter)
        {
            FiltersInUse.Add(filter);
        }

        public void Swap<T>(IList<T> list, int indexA, int indexB)
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

            if(sb.Length != 0)
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

            AddFiltersToPipeline(FiltersInUse.ToList());
        }

        private void AddFiltersToPipeline(List<IFilter> filters)
        {
            foreach (var filter in filters)
            {
                _pipeline.AddActionToPipe(filter.Name, tag =>
                {
                    return Task.Run(async () =>
                    {
                        if (tag == null) return null;

                        var newData = await filter.Filter(tag.TimeCoordinates);
                        return new Tag(tag.Id, newData);
                    });
                }, 2);
            }
        }

        private void RemoveFiltersFromPipeline(List<IFilter> filters)
        {
            foreach (var filter in filters)
            {
                _pipeline.RemoveActionFromPipe(filter.Name, 2);
            }

            _removedFilters.Clear();
        }


        private void ClosingAction()
        {
            SaveFilters();

            RemoveFiltersFromPipeline(_removedFilters);
            AddFiltersToPipeline(FiltersInUse.ToList());

            _eventAggregator.GetEvent<PipelineStartEvent>().Publish(new PipelineStartEventModel(this, _lastTag));
        }
    }
}
