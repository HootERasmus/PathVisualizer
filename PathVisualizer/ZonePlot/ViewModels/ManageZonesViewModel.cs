using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ZonePlot.Events;
using ZonePlot.Models;

namespace ZonePlot.ViewModels
{
    public class ManageZonesViewModel : BindableBase
    {
        public ObservableCollection<Zone> Zones { get; set; }
        
        public DelegateCommand AddZoneCommand { get; set; }
        public DelegateCommand RemoveZoneCommand { get; set; }
        public DelegateCommand ClosingCommand { get; set; }

        private Zone _lastSelectedZone;

        private Zone _selectedZone;
        public Zone SelectedZone
        {
            get => _selectedZone;
            set
            {
                if (value == SelectedZone) return;

                _selectedZone = value;
                RaisePropertyChanged();
                RemoveZoneCommand.RaiseCanExecuteChanged();
                if(SelectedZone != null)
                    _lastSelectedZone = _selectedZone;
            }
        }

        private readonly IEventAggregator _eventAggregator;
        private readonly ZoneUserSettingsService _settingsService;

        public ManageZonesViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            
            _settingsService = new ZoneUserSettingsService();
            Zones = new ObservableCollection<Zone>(_settingsService.LoadZones(_eventAggregator));

            AddZoneCommand = new DelegateCommand(AddZoneAction);
            RemoveZoneCommand = new DelegateCommand(RemoveZoneAction, CanRemoveZoneAction);
            ClosingCommand = new DelegateCommand(ClosingAction);
        }

        private void AddZoneAction()
        {
            Zones.Add(new Zone(_eventAggregator));
        }

        private void RemoveZoneAction()
        {
            _lastSelectedZone.PointsInText = string.Empty;
            _eventAggregator.GetEvent<ZoneChangeEvent>().Publish(_lastSelectedZone);
            Zones.Remove(_lastSelectedZone);
            _lastSelectedZone = null;
            RemoveZoneCommand.RaiseCanExecuteChanged();
        }

        private bool CanRemoveZoneAction()
        {
            return _lastSelectedZone != null;
        }

        private void ClosingAction()
        {
            _settingsService.SaveZones(Zones.ToList());
        }
    }
}
