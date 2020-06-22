using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Lib.Events;
using Lib.SharedModels;
using OxyPlot;
using PipelineService;
using Prism.Commands;
using Prism.Events;

namespace ZonePlot.ViewModels
{
    public class ZonePipelineViewModel
    {
        public ObservableCollection<string> PointsInZones { get; set; }
        
        public DelegateCommand CalculateCommand { get; set; }

        private readonly List<int> _pointsInsideZones;
        private Tag _tag;

        private readonly IEventAggregator _eventAggregator;

        public ZonePipelineViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(tag => _tag = tag.Values.Last());

            _pointsInsideZones = new List<int>();
            CalculateCommand = new DelegateCommand(CalculateAction);
            PointsInZones = new ObservableCollection<string>();
        }

        private async void CalculateAction()
        {
            PointsInZones.Clear();

            await Task.Run(() =>
            {
                if (_tag == null) return;

                var zones = ZoneService.Service.Zones;
                _pointsInsideZones.Clear();
                var count = 0;

                _eventAggregator.GetEvent<ProgressEvent>().Publish(new ProgressEventModel(0, zones.Count, count));
                for(int i = 0; i < zones.Count - 1; i++)
                {
                    _pointsInsideZones.Add(0);
                    foreach (var coordinate in _tag.TimeCoordinates)
                    {
                        if (zones[i].IsPointInsideZone(new DataPoint(coordinate.X, coordinate.Y)))
                        {
                            _pointsInsideZones[i]++;
                        }
                    }
                    _eventAggregator.GetEvent<ProgressEvent>().Publish(new ProgressEventModel(0, zones.Count, ++count));
                }

                
            });

            
            for (int i = 0; i < _pointsInsideZones.Count - 1; i++)
            {

                PointsInZones.Add($"Zone {i} - {_pointsInsideZones[i]}");
            }
        }
    }
}
