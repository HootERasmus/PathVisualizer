using System.Threading.Tasks;
using Lib.SharedModels;
using OxyPlot;

namespace PlotModelService
{
    public interface IPlotModelHelper
    {
        Task<PlotModel> PlotTagOnLinePlotModel(PlotModel model, Tag tag, PlotSettingsEventModel settings);
        Task<PlotModel> PlotTagOnHeatMapPlotModel(PlotModel model, Tag tag, PlotSettingsEventModel settings);
        Task<PlotModel> ApplyLinePlotSettings(PlotModel model, PlotSettingsEventModel settings);
        Task<PlotModel> ApplyHeatMapPlotSettings(PlotModel model, PlotSettingsEventModel settings);
        void ExportImage(PlotModel model, string path, int height, int width);
    }
}