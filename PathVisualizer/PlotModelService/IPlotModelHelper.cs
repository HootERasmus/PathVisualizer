using System.Collections.Generic;
using System.Threading.Tasks;
using Lib.SharedModels;
using OxyPlot;
using SettingsService;

namespace PlotModelService
{
    public interface IPlotModelHelper
    {
        Task<PlotModel> PlotTagOnLinePlotModel(PlotModel model, Tag tag, PlotSettingsEventModel settings);
        Task<PlotModel> PlotTagOnHeatMapPlotModel(PlotModel model, Tag tag, PlotSettingsEventModel settings);
        Task<PlotModel> AddLineSeriesToPlot(PlotModel model, List<DataPoint> dataPoints, string color, string textAnnotation = "");
        Task<PlotModel> ClearTextAnnotations(PlotModel model);
        PlotModel ApplyLinePlotSettings(PlotModel model, PlotSettingsEventModel settings);
        PlotModel ApplyHeatMapPlotSettings(PlotModel model, PlotSettingsEventModel settings);
        void ExportImage(PlotModel model, string path, int height, int width);
    }
}