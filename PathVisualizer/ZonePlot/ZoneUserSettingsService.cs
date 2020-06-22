using System.Collections.Generic;
using System.Text;
using Prism.Events;
using ZonePlot.Models;

namespace ZonePlot
{
    public class ZoneUserSettingsService
    {
        public void SaveZones(List<Zone> zones)
        {
            var sb = new StringBuilder();

            foreach (var zone in zones)
            {
                sb.Append($"{zone.PointsInText}|");
            }

            if (sb.Length != 0)
                sb.Remove(sb.Length - 1, 1);

            UserSettings.Default.Zones = sb.ToString();
            UserSettings.Default.Save();
        }

        public List<Zone> LoadZones(IEventAggregator eventAggregator)
        {
            var zones = new List<Zone>();
            var filtersString = UserSettings.Default.Zones;

            var split = filtersString.Split('|');

            foreach (var item in split)
            {
                zones.Add(new Zone(eventAggregator) { PointsInText = item });
            }

            return zones;
        }
    }
}
