using System;

namespace Lib.SharedModels.TimeModels
{
    public class TimeCoordinate : ITimeCoordinate
    {
        public TimeCoordinate(double x, double y, DateTime timestamp)
        {
            X = x;
            Y = y;
            Timestamp = timestamp;

            var t = timestamp - _epoch;
            EpochTimestamp = t.TotalSeconds;
        }

        public double X { get; }
        public double Y { get; }
        public DateTime Timestamp { get; }
        public double EpochTimestamp { get; }
        
        private readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
