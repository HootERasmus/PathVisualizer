﻿using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows.Media;
using Lib.Events;
using Lib.SharedModels;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Events;

namespace LinePlot.ViewModels
{
    public class LinePlotViewModel : BindableBase
    {
        public PlotModel MyPlotModel { get; set; }

        private List<DataPoint> _dataPoints;

        private readonly IEventAggregator _eventAggregator;

        public string Message { get; set; }

        public LinePlotViewModel(IEventAggregator eventAggregator)
        {
            MyPlotModel = new PlotModel();
            MyPlotModel.Axes.Add( new LinearAxis{Position = AxisPosition.Bottom});
            MyPlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<TagEvent>().Subscribe(PlotLine);
        }

        private void PlotLine(Tag tag)
        {
           
            _dataPoints = ConvertIntoDataPoints(tag);
            MyPlotModel.Series.Add(new LineSeries{ItemsSource = _dataPoints});
            
            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);


        }

        private List<DataPoint> ConvertIntoDataPoints(Tag tag)
        {
            var points = new List<DataPoint>();

            foreach (var timeCoordinate in tag.TimeCoordinates)
            {
                points.Add(new DataPoint(timeCoordinate.X, timeCoordinate.Y));
            }

            return points;
        }
    }
}
