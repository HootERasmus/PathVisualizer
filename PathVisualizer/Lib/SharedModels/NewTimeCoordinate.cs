using System;

namespace Lib.SharedModels
{
    public class NewTimeCoordinate : ITimeCoordinate
    {
        public NewTimeCoordinate(double x, double y, DateTime timestamp)
        {
            X = x;
            Y = y;
            Timestamp = timestamp;

            var t = timestamp - new DateTime(1970, 1,1);
            EpochTimestamp = t.TotalSeconds;
        }

        public double X { get; }
        public double Y { get; }
        public DateTime Timestamp { get; }
        public double EpochTimestamp { get; }

        private DateTime FromUnixTime(double unixTime)
        {
            return _epoch.AddSeconds(unixTime);
        }
        private readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
