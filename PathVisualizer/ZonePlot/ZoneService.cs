using System.Collections.Generic;
using ZonePlot.Models;

namespace ZonePlot
{
    public class ZoneService
    {
        private static ZoneService _service;
        public static ZoneService Service => _service ??= new ZoneService();

        private ZoneService()
        {
            Zones = new List<Zone>();
        }

        public List<Zone> Zones { get; set; }
    }
}
