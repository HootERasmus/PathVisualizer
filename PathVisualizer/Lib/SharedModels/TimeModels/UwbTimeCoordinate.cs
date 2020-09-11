using System;

namespace Lib.SharedModels.TimeModels
{
    public class UwbTimeCoordinate : ITimeCoordinate
    {
        public UwbTimeCoordinate(double x, double y, double z, string batteryPower, double timestamp, string unit, string dqi)
        {
            X = x;
            Y = y;
            Z = z;
            BatteryPower = batteryPower;
            EpochTimestamp = timestamp;
            Timestamp = FromUnixTime(timestamp);
            Unit = unit;
            DQI = dqi;
        }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public string BatteryPower { get; }
        public double EpochTimestamp { get; }
        public DateTime Timestamp { get; }
        public string Unit { get; }
        public string DQI { get; }

        private DateTime FromUnixTime(double unixTime)
        {
            return _epoch.AddSeconds(unixTime);
        }
        private readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
