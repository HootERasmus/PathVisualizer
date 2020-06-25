using System;
using Prism.Events;

namespace SettingsService
{
    public class PlotSettingsService : IPlotSettingService
    {
        private readonly IEventAggregator _eventAggregator;

        public PlotSettingsService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void SavePlotSettings(PlotSettingsEventModel model)
        {
            UserSettings.Default.XAxisTitle = model.XAxisTitle;
            UserSettings.Default.YAxisTitle = model.YAxisTitle;

            UserSettings.Default.XAxisMinimum = model.XAxisMinimum;
            UserSettings.Default.XAxisMaximum = model.XAxisMaximum;

            UserSettings.Default.YAxisMinimum = model.YAxisMinimum;
            UserSettings.Default.YAxisMaximum = model.YAxisMaximum;

            UserSettings.Default.BackgroundImagePath = model.BackgroundImage;

            UserSettings.Default.LineColor = model.LineColor;
            UserSettings.Default.DotColor = model.DotColor;

            UserSettings.Default.TimeOffSetDays = model.TimeOffSet.Days;
            UserSettings.Default.TimeOffSetHours = model.TimeOffSet.Hours;
            UserSettings.Default.TimeOffSetMinutes = model.TimeOffSet.Minutes;
            UserSettings.Default.TimeOffSetSeconds = model.TimeOffSet.Seconds;

            UserSettings.Default.Save();

            _eventAggregator.GetEvent<PlotSettingsEvent>().Publish(model);
        }

        public PlotSettingsEventModel LoadPlotSettings()
        {
            var timeOffSet = new TimeSpan(UserSettings.Default.TimeOffSetDays, UserSettings.Default.TimeOffSetHours, UserSettings.Default.TimeOffSetMinutes, UserSettings.Default.TimeOffSetSeconds);

            var model = new PlotSettingsEventModel
            {
                XAxisTitle = UserSettings.Default.XAxisTitle,
                YAxisTitle = UserSettings.Default.YAxisTitle,
                XAxisMinimum = UserSettings.Default.XAxisMinimum,
                XAxisMaximum = UserSettings.Default.XAxisMaximum,
                YAxisMinimum = UserSettings.Default.YAxisMinimum,
                YAxisMaximum = UserSettings.Default.YAxisMaximum,
                BackgroundImage = UserSettings.Default.BackgroundImagePath,
                LineColor = UserSettings.Default.LineColor,
                DotColor = UserSettings.Default.DotColor,
                TimeOffSet = timeOffSet
            };

            return model;
        }
    }
}
