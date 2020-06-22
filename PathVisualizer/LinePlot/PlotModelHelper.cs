using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Lib.SharedModels;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;

namespace LinePlot
{
    public class PlotModelHelper
    {
        public static async Task<PlotModel> PlotTagOnPlotModel(PlotModel model, Tag tag, PlotSettingsEventModel settings)
        {
            return await Task.Run(() =>
            {
                var color = Color.FromName(settings.LineColor);
                model.Series.Clear();
                var dataPoints = ConvertIntoDataPoints(tag);

                model.Series.Add(new LineSeries { Title = tag.Id, ItemsSource = dataPoints, Color = OxyColor.FromRgb(color.R, color.G, color.B) });
                model.LegendBackground = OxyColor.FromRgb(255, 255, 255);

                return model;
            });
        }

        public static async Task<PlotModel> ApplySettings(PlotModel model, PlotSettingsEventModel settings)
        {
            return await Task.Run(() =>
            {
                model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = settings.XAxisTitle, Minimum = settings.XAxisMinimum, Maximum = settings.XAxisMaximum });
                model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = settings.YAxisTitle, Minimum = settings.YAxisMinimum, Maximum = settings.YAxisMaximum });

                // Set image as background
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
                    Interpolate = true
                };
                model.Annotations.Add(imageAnnotation);

                return model;
            });
            
        }

        public static void ExportImage(PlotModel model, string path, int height, int width)
        {
            var exporter = new PngExporter { Height = height, Width = width };
            exporter.ExportToFile(model, path);
        }


        private static List<DataPoint> ConvertIntoDataPoints(Tag tag)
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
