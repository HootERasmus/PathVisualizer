using System.Collections.ObjectModel;
using System.Linq;
using Filters.Views;
using Prism.Commands;
using Prism.Mvvm;

namespace Filters.ViewModels
{
    public class FiltersViewModel : BindableBase
    {
        public DelegateCommand AddFilterCommand { get; set; }
        public DelegateCommand RemoveFilterCommand { get; set; }
        public DelegateCommand MoveFilterDownCommand { get; set; }
        public DelegateCommand MoveFilterUpCommand { get; set; }
        
        public ObservableCollection<string> FiltersInUse { get; set; }

        private string _selectedFilter;
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if(value == SelectedFilter) return;

                _selectedFilter = value;
                RaisePropertyChanged();
            }
        }

        public FiltersViewModel()
        {
            AddFilterCommand = new DelegateCommand(AddFilterAction);
            RemoveFilterCommand = new DelegateCommand(RemoveFilterAction);
            MoveFilterDownCommand = new DelegateCommand(MoveFilterDownAction);
            MoveFilterUpCommand = new DelegateCommand(MoveFilterUpAction);
        
            FiltersInUse = new ObservableCollection<string>();
        }

        private void AddFilterAction()
        {
            var window = new AddFilterWindow();
            window.DataContext = new AddFilterViewModel();

            window.Show();
        }

        private void RemoveFilterAction()
        {
            FiltersInUse.Remove(SelectedFilter);
        }

        private void MoveFilterDownAction()
        {
            var index = FiltersInUse.IndexOf(SelectedFilter);
            FiltersInUse.Remove(SelectedFilter);
            FiltersInUse.Insert(index-1, SelectedFilter);
        }

        private void MoveFilterUpAction()
        {
            var index = FiltersInUse.IndexOf(SelectedFilter);
            FiltersInUse.Remove(SelectedFilter);
            FiltersInUse.Insert(index + 1, SelectedFilter);
        }

    }
}
