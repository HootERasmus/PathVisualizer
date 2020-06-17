namespace Lib.SharedModels
{
    public class TimeCoordinate
    {
        public TimeCoordinate(double x, double y, double z, string batteryPower, double timestamp, string unit, string dqi)
        {
            X = x;
            Y = y;
            Z = z;
            BatteryPower = batteryPower;
            Timestamp = timestamp;
            Unit = unit;
            DQI = dqi;
        }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public string BatteryPower { get; }
        public double Timestamp { get; }
        public string Unit { get; }
        public string DQI { get; }
    }
}
