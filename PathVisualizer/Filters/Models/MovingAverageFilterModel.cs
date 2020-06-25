using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.SharedModels;

namespace Filters.Models
{
    public class MovingAverageFilterModel : IFilter
    {
        public string Name { get; }
        public int Period { get; set; }

        public MovingAverageFilterModel()
        {
            Name = "Moving Average";
            Period = 15;
        }

        public async Task<List<TimeCoordinate>> Filter(List<TimeCoordinate> originalData)
        {
            var movingAverage = new List<TimeCoordinate>();
            await Task.Run(() =>
            {
                var bufferX = new double[Period];
                var bufferY = new double[Period];
                var currentIndex = 0;
                
                foreach (var point in originalData)
                {
                    bufferX[currentIndex] = point.X / Period;
                    bufferY[currentIndex] = point.Y / Period;

                    var maX = 0.0;
                    var maY = 0.0;
                    for (int j = 0; j < Period; j++)
                    {
                        maX += bufferX[j];
                        maY += bufferY[j];
                    }
                    movingAverage.Add(new TimeCoordinate(maX, maY, point.Z, point.BatteryPower, point.Timestamp, point.Unit, point.DQI));
                    currentIndex = (currentIndex + 1) % Period;
                }
            });
            
            return movingAverage.Skip(Period - 1).ToList();
        }
    }
}
