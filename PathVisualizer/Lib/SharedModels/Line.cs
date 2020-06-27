using System.Collections.Generic;

namespace Lib.SharedModels
{
    public class Line
    {
        public IList<TimeCoordinate> Points { get; set; }
        public string TextAnnotation { get; set; }
        public string Color { get; set; }
    }
}
