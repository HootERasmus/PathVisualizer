using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.SharedModels;

namespace Filters.Models
{
    public class RemoveDeltaFilterModel : IFilter
    {
        public string Name { get; }

        public double ErrorSpike { get; set; }
        
        public RemoveDeltaFilterModel()
        {
            Name = "Remove Delta";
            ErrorSpike = 1;
        }

        public async Task<List<TimeCoordinate>> Filter(List<TimeCoordinate> originalData)
        {
            var lines = new List<TimeCoordinate>();

            await Task.Run(() =>
            {
                
                var numberOfErrorSpikes = 0;

                foreach (var timeCoordinate in originalData)
                {
                    var newPoint = new TimeCoordinate(
                        timeCoordinate.X,
                        timeCoordinate.Y,
                        timeCoordinate.Z,
                        timeCoordinate.BatteryPower,
                        timeCoordinate.Timestamp,
                        timeCoordinate.Unit,
                        timeCoordinate.DQI);

                    if (!lines.Any())
                        lines.Add(newPoint);

                    if (!IsErrorSpike(lines.Last(), newPoint, numberOfErrorSpikes, ErrorSpike))
                    {
                        lines.Add(newPoint);
                        numberOfErrorSpikes = 0;
                    }
                    else
                        numberOfErrorSpikes++;
                }

                
            });
            return lines;

        }

        private bool IsErrorSpike(TimeCoordinate first, TimeCoordinate second, int numberOfErrors, double errorThreshold)
        {
            return Euclidean(first, second) > errorThreshold * (numberOfErrors + 1);
        }

        public double Euclidean(TimeCoordinate first, TimeCoordinate second)
        {
            return Math.Sqrt(Math.Pow(first.X - second.X, 2) + Math.Pow(first.Y - second.Y, 2));
        }
    }
}
