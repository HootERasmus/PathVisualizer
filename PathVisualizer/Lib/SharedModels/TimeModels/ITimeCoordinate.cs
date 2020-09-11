using System;

namespace Lib.SharedModels.TimeModels
{
    public interface ITimeCoordinate
    {
        public double X { get; }
        public double Y { get; }
        public DateTime Timestamp { get; }
        public double EpochTimestamp { get; }
    }
}
