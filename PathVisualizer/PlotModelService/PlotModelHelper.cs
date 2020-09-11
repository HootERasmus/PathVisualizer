using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lib.SharedModels;
using Lib.SharedModels.TimeModels;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using SettingsService;
using HeatMapSeries = OxyPlot.Series.HeatMapSeries;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LinearColorAxis = OxyPlot.Axes.LinearColorAxis;
using LineSeries = OxyPlot.Series.LineSeries;
using TextAnnotation = OxyPlot.Annotations.TextAnnotation;

namespace PlotModelService
{
    public class PlotModelHelper : IPlotModelHelper
    {
        private const string BackgroundTag = "Background";
        private const string TextTag = "Text";
        private readonly object _lockObject;

        public PlotModelHelper()
        {
            _lockObject = new object();
        }

        public async Task<PlotModel> PlotTagOnLinePlotModel(PlotModel model, Tag tag, PlotSettingsEventModel settings)
        {
            return await Task.Run(() =>
            {
                var color = Color.FromName(settings.LineColor);
                model.Series.Clear();
                
                var dataPoints = ConvertIntoDataPoints(tag.TimeCoordinates);

                model.Series.Add(new LineSeries { Title = tag.Id, ItemsSource = dataPoints, Color = OxyColor.FromRgb(color.R, color.G, color.B) });
                model.LegendBackground = OxyColor.FromRgb(255, 255, 255);

                return model;
            });
        }

        public async Task<PlotModel> PlotTagOnHeatMapPlotModel(PlotModel model, Tag tag, PlotSettingsEventModel settings)
        {
            return await Task.Run(() =>
            {
                model.Series.Clear();
                var dataPoints = ConvertIntoDataPoints(tag.TimeCoordinates, settings, 1);
                var logPoints = ConvertIntoLogScale(dataPoints, settings, 1);

                var heatMapSeries = new HeatMapSeries
                {
                    X0 = settings.XAxisMinimum,
                    X1 = settings.XAxisMaximum,
                    Y0 = settings.YAxisMinimum,
                    Y1 = settings.YAxisMaximum,
                    Interpolate = true,
                    RenderMethod = HeatMapRenderMethod.Bitmap,
                    Data = logPoints
                };

                model.Series.Add(heatMapSeries);
                return model;
            });
        }

        public async Task<PlotModel> ClearTextAnnotations(PlotModel model)
        {
            return await Task.Run(() =>
            {
                var annotations = model.Annotations.Where(x => (string) x.Tag == TextTag).ToList();
                foreach (var annotation in annotations)
                {
                    model.Annotations.Remove(annotation);
                }
                return model;
            });

        }

        public async Task<PlotModel> AddLineSeriesToPlot(PlotModel model, List<DataPoint> dataPoints, string lineColor, string textAnnotation = "")
        {
            return await Task.Run(() =>
            {
                var color = Color.FromName(lineColor);

                var xAvg = dataPoints.Average(x => x.X);
                var yAvg = dataPoints.Average(x => x.Y);

                model.Series.Add(new LineSeries {ItemsSource = dataPoints, Color = OxyColor.FromRgb(color.R, color.G, color.B) });

                if (string.IsNullOrEmpty(textAnnotation)) return model;

                var annotation = new TextAnnotation
                {
                    Background = OxyColor.FromRgb(255,255,255),
                    Text = textAnnotation,
                    TextColor = OxyColor.FromRgb(0,0,0),
                    TextPosition = new DataPoint(xAvg, yAvg),
                    Tag = TextTag
                };
                model.Annotations.Add(annotation);
                return model;
            });
        }

        public PlotModel ApplyLinePlotSettings(PlotModel model, PlotSettingsEventModel settings)
        {
            lock (_lockObject)
            {
                model.Axes.Clear();
                model.Annotations.Clear();

                model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = settings.XAxisTitle, Minimum = settings.XAxisMinimum, Maximum = settings.XAxisMaximum });
                model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = settings.YAxisTitle, Minimum = settings.YAxisMinimum, Maximum = settings.YAxisMaximum });

                // Set image as background
                if (!File.Exists(settings.BackgroundImage)) return model;

                using var fs = new FileStream(settings.BackgroundImage, FileMode.Open);
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
                    Interpolate = true,
                    Tag = BackgroundTag
                };
                model.Annotations.Add(imageAnnotation);
                return model;

            }
        }

        public PlotModel ApplyHeatMapPlotSettings(PlotModel model, PlotSettingsEventModel settings)
        {
            lock (_lockObject)
            {
                model.Annotations.Clear();
                model.Axes.Clear();

                // Color axis (the X and Y axes are generated automatically)
                model.Axes.Add(new LinearColorAxis {Position = AxisPosition.Right, Palette = OxyPalettes.Jet(100)});
                model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = settings.XAxisTitle, Minimum = settings.XAxisMinimum, Maximum = settings.XAxisMaximum });
                model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = settings.YAxisTitle, Minimum = settings.YAxisMinimum,  Maximum = settings.YAxisMaximum });

                // Set image as background
                if (!File.Exists(settings.BackgroundImage)) return model;

                using var fs = new FileStream(settings.BackgroundImage, FileMode.Open);
                var image0 = new OxyImage(fs);

                var imageAnnotation = new ImageAnnotation
                {
                    ImageSource = image0,
                    Opacity = 0.5,
                    X = new PlotLength(0.5, PlotLengthUnit.RelativeToPlotArea),
                    Y = new PlotLength(0.5, PlotLengthUnit.RelativeToPlotArea),
                    Width = new PlotLength(1, PlotLengthUnit.RelativeToPlotArea),
                    Height = new PlotLength(1, PlotLengthUnit.RelativeToPlotArea),
                    Layer = AnnotationLayer.AboveSeries,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Middle,
                    Interpolate = true,
                    Tag = BackgroundTag
                };
                model.Annotations.Add(imageAnnotation);
                return model;
            }
        }

        public void ExportImage(PlotModel model, string path, int height, int width)
        {
            var exporter = new PngExporter { Height = height, Width = width };
            exporter.ExportToFile(model, path);
        }

        private double[,] ConvertIntoDataPoints(IList<ITimeCoordinate> timeCoordinates, PlotSettingsEventModel settings, int squareSize, int maxCap = Int32.MaxValue)
        {
            var points = new double[((int)settings.XAxisMaximum - (int)settings.XAxisMinimum) / squareSize, ((int)settings.YAxisMaximum - (int)settings.YAxisMinimum) / squareSize];
            var xOffSet = (int)Math.Abs(settings.XAxisMinimum);
            var yOffSet = (int)Math.Abs(settings.YAxisMinimum);
            foreach (var timeCoordinate in timeCoordinates)
            {
                var x = (int)timeCoordinate.X + xOffSet;
                var y = (int)timeCoordinate.Y + yOffSet;

                try
                {
                    if (points[x, y] < maxCap)
                        points[x, y]++;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            return points;
        }

        private double[,] ConvertIntoLogScale(double[,] map, PlotSettingsEventModel settings, int squareSize)
        {
            var xLength = ((int) settings.XAxisMaximum - (int) settings.XAxisMinimum) / squareSize;
            var yLength = ((int) settings.YAxisMaximum - (int) settings.YAxisMinimum) / squareSize;
            var logScale = new double[xLength, yLength];
            
            for (int i = 0; i < xLength - 1; i++)
            {
                for (int j = 0; j < yLength - 1; j++)
                {
                    var value = map[i, j] + 1;
                    var logValue = Math.Log(value);
                    logScale[i, j] = logValue;
                }
            }

            return logScale;
        }

        private List<DataPoint> ConvertIntoDataPoints(IList<ITimeCoordinate> timeCoordinates)
        {
            var points = new List<DataPoint>();

            foreach (var timeCoordinate in timeCoordinates)
            {
                points.Add(new DataPoint(timeCoordinate.X, timeCoordinate.Y));
            }

            return points;
        }
    }
}
