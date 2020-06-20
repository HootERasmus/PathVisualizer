using System.Collections.Generic;

namespace Lib.SharedModels
{
    public class Tag
    {
        public Tag(string id, List<TimeCoordinate> timeCoordinates)
        {
            TimeCoordinates = timeCoordinates;
            Id = id;
        }

        public string Id { get; }
        public List<TimeCoordinate> TimeCoordinates { get; }
    }
}
