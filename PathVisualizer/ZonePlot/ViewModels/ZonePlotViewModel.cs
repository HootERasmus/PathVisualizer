using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<string> PointsInZones { get; set; }
        public DelegateCommand CalculateCommand { get; set; }
        private readonly List<int> _pointsInsideZones;
        private Tag _tag;

        private readonly List<Zone> _zones;
        private readonly IPlotModelHelper _plotModelHelper;
        private readonly IEventAggregator _eventAggregator;

        public ZonePlotViewModel(IEventAggregator eventAggregator, IPlotModelHelper plotModelHelper, IPlotSettingService plotSettingService)
        {
            PointsInZones = new ObservableCollection<string>();
            _plotModelHelper = plotModelHelper;

            CalculateCommand = new DelegateCommand(CalculateAction);

            var zoneUserSettingsService = new ZoneUserSettingsService();
            _zones = zoneUserSettingsService.LoadZones(eventAggregator);
            
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PlotSettingsEvent>().Subscribe(ApplyPlotSettings);
            _eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(OnPipelineCompletedEvent);
            _eventAggregator.GetEvent<ZoneChangeEvent>().Subscribe(PlotZone);
            

            Settings = plotSettingService.LoadPlotSettings();
            ApplyPlotSettings(Settings);
            
            _pointsInsideZones = new List<int>();
        }

        private async void OnPipelineCompletedEvent(IList<PipelineCompletedEventModel> history)
        {
            if (history.Any())
            {
                _tag = history.Last().Tag;
                await PlotLine(history.Last().Tag, _zones);
            }
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

            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);
        }

        private async Task PlotLine(Tag tag, List<Zone> zones)
        {
            SelectedTag = tag;

            MyPlotModel = await _plotModelHelper.PlotTagOnLinePlotModel(MyPlotModel, SelectedTag, Settings);
            MyPlotModel = await _plotModelHelper.ClearTextAnnotations(MyPlotModel);

            foreach (var zone in zones)
            {
                MyPlotModel = await _plotModelHelper.AddLineSeriesToPlot(MyPlotModel, zone.Points, zone.SelectedColor, zone.TextAnnotation);
            }
            
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

        private async void CalculateAction()
        {
            PointsInZones.Clear();

            await Task.Run(() =>
            {
                if (_tag == null) return;

                _pointsInsideZones.Clear();
                var count = 0;

                _eventAggregator.GetEvent<ProgressEvent>().Publish(new ProgressEventModel(0, _zones.Count, count));
                for (int i = 0; i < _zones.Count; i++)
                {
                    _pointsInsideZones.Add(0);
                    foreach (var coordinate in _tag.TimeCoordinates)
                    {
                        if (_zones[i].IsPointInsideZone(new DataPoint(coordinate.X, coordinate.Y)))
                        {
                            _pointsInsideZones[i]++;
                        }
                    }
                    _eventAggregator.GetEvent<ProgressEvent>().Publish(new ProgressEventModel(0, _zones.Count, ++count));
                }
            });


            for (int i = 0; i < _pointsInsideZones.Count; i++)
            {

                PointsInZones.Add($"{_zones[i].TextAnnotation} - {_pointsInsideZones[i]}");
            }
        }
    }
}
