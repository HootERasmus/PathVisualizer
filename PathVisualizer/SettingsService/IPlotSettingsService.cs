namespace SettingsService
{
    public interface IPlotSettingService
    {
        void SavePlotSettings(PlotSettingsEventModel model);
        PlotSettingsEventModel LoadPlotSettings();
    }
}
