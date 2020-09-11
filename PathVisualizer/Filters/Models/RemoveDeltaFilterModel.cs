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

        public async Task<List<ITimeCoordinate>> Filter(List<ITimeCoordinate> originalData)
        {
            var lines = new List<ITimeCoordinate>();

            await Task.Run(() =>
            {
                
                var numberOfErrorSpikes = 0;

                foreach (var timeCoordinate in originalData)
                {
                    var newPoint = new NewTimeCoordinate(
                        timeCoordinate.X,
                        timeCoordinate.Y,
                        timeCoordinate.Timestamp
                        );

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

        private bool IsErrorSpike(ITimeCoordinate first, ITimeCoordinate second, int numberOfErrors, double errorThreshold)
        {
            return Euclidean(first, second) > errorThreshold * (numberOfErrors + 1);
        }

        public double Euclidean(ITimeCoordinate first, ITimeCoordinate second)
        {
            return Math.Sqrt(Math.Pow(first.X - second.X, 2) + Math.Pow(first.Y - second.Y, 2));
        }
    }
}
