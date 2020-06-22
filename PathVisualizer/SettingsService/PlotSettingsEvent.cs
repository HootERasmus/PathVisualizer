using Prism.Events;

namespace SettingsService
{
    public class PlotSettingsEvent : PubSubEvent<PlotSettingsEventModel>
    {
    }

    public class PlotSettingsEventModel
    {
        public PlotSettingsEventModel()
        {
            XAxisTitle = "X[m]";
            YAxisTitle = "Y[m]";

            XAxisMinimum = 0;
            XAxisMaximum = 10;

            YAxisMinimum = 0;
            YAxisMaximum = 10;

            LineColor = "Cyan";
            DotColor = "Cyan";
        }

        public string XAxisTitle { get; set; }
        public string YAxisTitle { get; set; }

        public double XAxisMinimum { get; set; }
        public double XAxisMaximum { get; set; }

        public double YAxisMinimum { get; set; }
        public double YAxisMaximum { get; set; }

        public string BackgroundImage { get; set; }

        public string LineColor { get; set; }
        public string DotColor { get; set; }
    }
}
