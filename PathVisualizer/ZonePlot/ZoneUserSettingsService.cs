using System;
using System.Collections.Generic;
using System.Linq;
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
                sb.Append($"{zone.PointsInText}]{zone.TextAnnotation}]{zone.SelectedColor}]{zone.ZoneId}|");
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

            if(split.Length == 1 && string.IsNullOrEmpty(split.First()))
                return new List<Zone>();

            foreach (var item in split)
            {
                var items = item.Split(']');

                zones.Add(new Zone(eventAggregator, items[0], items[1], items[2], Guid.Parse(items[3])));
            }

            return zones;
        }
    }
}
