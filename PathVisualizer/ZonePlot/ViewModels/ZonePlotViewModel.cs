using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.Events;
using Lib.SharedModels;
using OxyPlot;
using PipelineService;
using PlotModelService;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using SettingsService;
using ZonePlot.Events;
using ZonePlot.Models;

namespace ZonePlot.ViewModels
{
    public class ZonePlotViewModel : BindableBase
    {
        public PlotModel MyPlotModel { get; set; }
        public PlotSettingsEventModel Settings { get; set; }
        public Tag SelectedTag { get; set; }
        private readonly List<Zone> _zones;
        private readonly Dictionary<Guid, int> _pointsInsideZones;
        private readonly ZoneUserSettingsService _zoneUserSettingsService;
        private readonly IPlotModelHelper _plotModelHelper;
        private readonly IEventAggregator _eventAggregator;
        private readonly IPipeline _pipeline;
        public DelegateCommand TestCommand { get; set; }

        public ZonePlotViewModel(IEventAggregator eventAggregator, IPipeline pipeline, IPlotModelHelper plotModelHelper, IPlotSettingService plotSettingService)
        {
            _plotModelHelper = plotModelHelper;
            
            _zoneUserSettingsService = new ZoneUserSettingsService();
            _zones = _zoneUserSettingsService.LoadZones(eventAggregator);
            _pipeline = pipeline;

            ZoneService.Service.Zones = _zones;

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PlotSettingsEvent>().Subscribe(ApplyPlotSettings);
            _eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(OnPipelineCompletedEvent);
            _eventAggregator.GetEvent<ZoneChangeEvent>().Subscribe(PlotZone);

            Settings = plotSettingService.LoadPlotSettings();
            ApplyPlotSettings(Settings);

            _zones = new List<Zone>();
            _pointsInsideZones = new Dictionary<Guid, int>();
        }

        private async void OnPipelineCompletedEvent(IDictionary<string, Tag> history)
        {
            await PlotLine(history.Values.Last(), _zones);
        }

        private async void PlotZone(Zone zone)
        {
            var firstOrDefault = _zones.FirstOrDefault(x => x.ZoneId == zone.ZoneId);
            if (firstOrDefault != null)
                _zones.Remove(firstOrDefault);
            
            if(!string.IsNullOrEmpty(zone.PointsInText))
                _zones.Add(zone);
            
            if(SelectedTag == null) return;
            
            await PlotLine(SelectedTag, _zones);

            ZoneService.Service.Zones.Clear();
            ZoneService.Service.Zones = _zones;

            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);
        }

        private async Task PlotLine(Tag tag, List<Zone> zones)
        {
            SelectedTag = tag;

            var zonesInListFormat = new List<List<DataPoint>>();

            foreach (var zone in zones)
            {
                zonesInListFormat.Add(zone.Points);
            }

            await _plotModelHelper.PlotTagAndZonesOnLinePlotModel(MyPlotModel, SelectedTag, zonesInListFormat, Settings);

            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);
        }

        private async void ApplyPlotSettings(PlotSettingsEventModel model)
        {
            Settings = model;

            MyPlotModel = new PlotModel();
            MyPlotModel = _plotModelHelper.ApplyLinePlotSettings(MyPlotModel, Settings);

            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);

            if (SelectedTag != null)
            {
                await PlotLine(SelectedTag, _zones);
            }
        }
    }
}
