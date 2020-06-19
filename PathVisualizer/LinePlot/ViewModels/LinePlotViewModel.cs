using Prism.Mvvm;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lib.Events;
using Lib.SharedModels;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Pipeline;
using Prism.Events;
using Color = System.Drawing.Color;

namespace LinePlot.ViewModels
{
    public class LinePlotViewModel : BindableBase
    {
        public PlotModel MyPlotModel { get; set; }
        public PlotSettingsEventModel Settings { get; set; }
        public Tag SelectedTag { get; set; }

        private string _backgroundImage;
        public string BackgroundImage
        {
            get => _backgroundImage;
            set
            {
                if(value == BackgroundImage) return;

                _backgroundImage = value;
                RaisePropertyChanged();
            }
        }
        
        private List<DataPoint> _dataPoints;

        private readonly IEventAggregator _eventAggregator;

        public LinePlotViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PlotSettingsEvent>().Subscribe(ApplyPlotSettings);
            _eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(PlotLine);
        }

        private async void PlotLine(Tag tag)
        {
            SelectedTag = tag;

            await Task.Run(() =>
            {
                var color = Color.FromName(Settings.LineColor);
                MyPlotModel.Series.Clear();
                _dataPoints = ConvertIntoDataPoints(SelectedTag);
                MyPlotModel.Series.Add(new LineSeries {Title = tag.Id, ItemsSource = _dataPoints, Color = OxyColor.FromRgb(color.R, color.G, color.B)});
                MyPlotModel.LegendBackground = OxyColor.FromRgb(255,255,255);
                RaisePropertyChanged(nameof(MyPlotModel));
                MyPlotModel.InvalidatePlot(true);
            });
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

        private void ApplyPlotSettings(PlotSettingsEventModel model)
        {
            Settings = model;

            BackgroundImage = Settings.BackgroundImage;

            MyPlotModel = new PlotModel();
            MyPlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = model.XAxisTitle, Minimum = model.XAxisMinimum, Maximum = model.XAxisMaximum });
            MyPlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = model.YAxisTitle, Minimum = model.YAxisMinimum, Maximum = model.YAxisMaximum });

            if (SelectedTag != null)
            {
                PlotLine(SelectedTag);
            }
        }
    }
}
