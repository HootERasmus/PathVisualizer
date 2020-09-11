using System.Collections.Generic;
using Lib.SharedModels.TimeModels;

namespace Lib.SharedModels
{
    public class Line
    {
        public IList<UwbTimeCoordinate> Points { get; set; }
        public string TextAnnotation { get; set; }
        public string Color { get; set; }
    }
}
