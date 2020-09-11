using System.Collections.Generic;
using Lib.SharedModels.TimeModels;

namespace Lib.SharedModels
{
    public class Tag
    {
        public Tag(string id, List<ITimeCoordinate> timeCoordinates)
        {
            TimeCoordinates = timeCoordinates;
            Id = id;
        }

        public string Id { get; }
        public List<ITimeCoordinate> TimeCoordinates { get; }
    }
}
