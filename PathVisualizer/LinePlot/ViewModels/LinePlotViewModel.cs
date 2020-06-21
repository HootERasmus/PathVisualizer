using System;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Lib.Events;
using Lib.SharedModels;
using LinePlot.Events;
using MetadataExtractor;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Reporting;
using OxyPlot.Wpf;
using PipelineService;
using Prism.Events;
using Color = System.Drawing.Color;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;
using Tag = Lib.SharedModels.Tag;

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
            _eventAggregator.GetEvent<PipelineCompletedEvent>().Subscribe(OnPipelineCompletedEvent);
            _eventAggregator.GetEvent<ExportPlotEvent>().Subscribe(ExportImage);

        }

        private async void OnPipelineCompletedEvent(IDictionary<string, Tag> history)
        {
            await PlotLine(history.Values.Last());
        }

        private async Task PlotLine(Tag tag)
        {
            SelectedTag = tag;

            await Task.Run(() =>
            {
                // Set image as background
                using var fs = new FileStream(BackgroundImage, FileMode.Open);
                var image0 = new OxyImage(fs);

                var imageAnnotation = new ImageAnnotation
                {
                    ImageSource = image0,
                    Opacity = 1,
                    X = new PlotLength(0.5, PlotLengthUnit.RelativeToPlotArea),
                    Y = new PlotLength(0.5, PlotLengthUnit.RelativeToPlotArea),
                    Width = new PlotLength(1, PlotLengthUnit.RelativeToPlotArea),
                    Height = new PlotLength(1, PlotLengthUnit.RelativeToPlotArea),
                    Layer = AnnotationLayer.BelowSeries,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Middle,
                    Interpolate = true
                };
                MyPlotModel.Annotations.Add(imageAnnotation);

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

        private async void ApplyPlotSettings(PlotSettingsEventModel model)
        {
            Settings = model;

            BackgroundImage = Settings.BackgroundImage;

            MyPlotModel = new PlotModel();
            MyPlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = model.XAxisTitle, Minimum = model.XAxisMinimum, Maximum = model.XAxisMaximum });
            MyPlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = model.YAxisTitle, Minimum = model.YAxisMinimum, Maximum = model.YAxisMaximum });

            if (SelectedTag != null)
            {
                await PlotLine(SelectedTag);
            }
        }

        private void ExportImage(string path)
        {
            var directories = ImageMetadataReader.ReadMetadata(BackgroundImage);

            int height;
            int width;

            try
            {
                width = int.Parse(directories[0].Tags[0].Description);
                height = int.Parse(directories[0].Tags[1].Description);
            }
            catch (Exception e)
            {
               Debug.WriteLine(e);
               height = 800;
               width = 600;
            }

            var exporter = new PngExporter{Height = height, Width = width};
            exporter.ExportToFile(MyPlotModel, path);
        }
    }
}
