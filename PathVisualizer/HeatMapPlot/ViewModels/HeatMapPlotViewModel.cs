using System;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Lib.Events;
using Lib.SharedModels;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PipelineService;
using Prism.Events;

namespace HeatMapPlot.ViewModels
{
    public class HeatMapPlotViewModel : BindableBase
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
                if (value == BackgroundImage) return;

                _backgroundImage = value;
                RaisePropertyChanged();
            }
        }

        private double[,] _dataPoints;

        private readonly IEventAggregator _eventAggregator;
        private int _xOffSet;
        private int _yOffSet;

        public HeatMapPlotViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<PlotSettingsEvent>().Subscribe(ApplyPlotSettings);
            _eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(OnPipelineCompletedEvent);
        }

        private async void OnPipelineCompletedEvent(IDictionary<string,Tag> history)
        {
            await PlotHeatMap(history.Values.Last());
        }

        private async Task PlotHeatMap(Tag tag)
        {
            SelectedTag = tag;

            await Task.Run(() =>
            {
                MyPlotModel.Series.Clear();

                _dataPoints = ConvertIntoDataPoints(tag, 1);

                var heatMapSeries = new HeatMapSeries
                {
                    X0 = Settings.XAxisMinimum,
                    X1 = Settings.XAxisMaximum,
                    Y0 = Settings.YAxisMinimum,
                    Y1 = Settings.YAxisMaximum,
                    Interpolate = true,
                    RenderMethod = HeatMapRenderMethod.Bitmap,
                    Data = _dataPoints
                };

                MyPlotModel.Series.Add(heatMapSeries);
                RaisePropertyChanged(nameof(MyPlotModel));
                MyPlotModel.InvalidatePlot(true);
            });
        }

        private double[,] ConvertIntoDataPoints(Tag tag, int squareSize)
        {
            var points = new double[((int)Settings.XAxisMaximum - (int)Settings.XAxisMinimum)/squareSize, ((int)Settings.YAxisMaximum - (int)Settings.YAxisMinimum)/squareSize];
            
            foreach (var timeCoordinate in tag.TimeCoordinates)
            {
                var x = (int) timeCoordinate.X + _xOffSet;
                var y = (int) timeCoordinate.Y + _yOffSet;
            
                try
                {
                    if(points[x, y] <= 10)
                        points[x, y]++;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            return points;
        }

        private void ApplyPlotSettings(PlotSettingsEventModel model)
        {
            Settings = model;

            BackgroundImage = Settings.BackgroundImage;
            _xOffSet = (int)Math.Abs(Settings.XAxisMinimum);
            _yOffSet = (int) Math.Abs(Settings.YAxisMinimum);

            MyPlotModel = new PlotModel();

            // Color axis (the X and Y axes are generated automatically)
            MyPlotModel.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Jet(100) });
            MyPlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = model.XAxisTitle, Minimum = model.XAxisMinimum, Maximum = model.XAxisMaximum });
            MyPlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = model.YAxisTitle, Minimum = model.YAxisMinimum, Maximum = model.YAxisMaximum });

            RaisePropertyChanged(nameof(MyPlotModel));
            MyPlotModel.InvalidatePlot(true);

            if (SelectedTag != null)
            {
                PlotHeatMap(SelectedTag);
            }
        }
    }
}
