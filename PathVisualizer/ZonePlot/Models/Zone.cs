using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using OxyPlot;
using Prism.Events;
using Prism.Mvvm;
using ZonePlot.Events;

namespace ZonePlot.Models
{
    public class Zone : BindableBase
    {
        private string _pointsInText;
        public string PointsInText
        {
            get => _pointsInText;
            set
            {
                if (value == PointsInText) return;

                _pointsInText = value;
                RaisePropertyChanged();
                _eventAggregator.GetEvent<ZoneChangeEvent>().Publish(this);
            }
        }

        public ObservableCollection<string> Colors { get; set; }
        public Guid ZoneId { get; }
        public List<DataPoint> Points => TextIntoDataPoints();

        private string _textAnnotation;
        public string TextAnnotation
        {
            get => _textAnnotation;
            set
            {
                if(value == TextAnnotation) return;

                _textAnnotation = value;
                RaisePropertyChanged();
                _eventAggregator.GetEvent<ZoneChangeEvent>().Publish(this);
            }
        }

        private string _selectedColor;
        public string SelectedColor
        {
            get => _selectedColor;
            set
            {
                if(value == SelectedColor) return;

                _selectedColor = value;
                RaisePropertyChanged();
                _eventAggregator.GetEvent<ZoneChangeEvent>().Publish(this);
            }
        }
        
        private readonly IEventAggregator _eventAggregator;
        
        public Zone(IEventAggregator eventAggregator, string pointsInText = "", string textAnnotation = "", string selectedColor = "Red", Guid zoneId = default)
        {
            ZoneId = zoneId == default ? Guid.NewGuid() : zoneId;

            _eventAggregator = eventAggregator;
            _pointsInText = pointsInText;
            SelectedColor = selectedColor;
            TextAnnotation = textAnnotation;

            Colors = new ObservableCollection<string>();

            var colorType = typeof(Color);
            var propInfoList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (var c in propInfoList)
            {
                Colors.Add(c.Name);
            }
        }

        private List<DataPoint> TextIntoDataPoints()
        {
            var list = new List<DataPoint>();
            // (5.4, 4.5); (5.4, 4.5)
            var stringWithoutSpace = PointsInText.Replace(" ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);

            // 5.4,4.5;5.4,4.5
            var coordinates = stringWithoutSpace.Split(';');

            // ["5.4,4.5", "5.4,4.5"]
            foreach (var coordinate in coordinates)
            {
                // "5.4,4.5"
                var singleValues = coordinate.Split(',');

                // "["5.4", "4.5"]
                if (singleValues.Length != 2) continue;
                
                var x = double.TryParse(singleValues[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var xResult);
                var y = double.TryParse(singleValues[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var yResult);
                    
                if(x && y)
                    list.Add(new DataPoint(xResult, yResult));
            }
            

            if(list.Any())
                list.Add(list.First());

            return list;
        }

        public bool IsPointInsideZone(DataPoint dataPoint)
        {
            return IsInside(Points.ToArray(),Points.Count, dataPoint);
        }

        #region CopyCode

        // Given three colinear points p, q, r,  
        // the function checks if point q lies 
        // on line segment 'pr' 
        private bool OnSegment(DataPoint p, DataPoint q, DataPoint r)
        {
            if (q.X <= Math.Max(p.X, r.X) &&
                q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) &&
                q.Y >= Math.Min(p.Y, r.Y))
            {
                return true;
            }
            return false;
        }

        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are colinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        private int Orientation(DataPoint p, DataPoint q, DataPoint r)
        {
            double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);

            if (Math.Abs(val) < 0.000001)
            {
                return 0; // colinear 
            }
            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        // The function that returns true if  
        // line segment 'p1q1' and 'p2q2' intersect. 
        private bool DoIntersect(DataPoint p1, DataPoint q1, DataPoint p2, DataPoint q2)
        {
            // Find the four orientations needed for  
            // general and special cases 
            var o1 = Orientation(p1, q1, p2);
            var o2 = Orientation(p1, q1, q2);
            var o3 = Orientation(p2, q2, p1);
            var o4 = Orientation(p2, q2, q1);

            // General case 
            if (o1 != o2 && o3 != o4)
            {
                return true;
            }

            // Special Cases 
            // p1, q1 and p2 are colinear and 
            // p2 lies on segment p1q1 
            if (o1 == 0 && OnSegment(p1, p2, q1))
            {
                return true;
            }

            // p1, q1 and p2 are colinear and 
            // q2 lies on segment p1q1 
            if (o2 == 0 && OnSegment(p1, q2, q1))
            {
                return true;
            }

            // p2, q2 and p1 are colinear and 
            // p1 lies on segment p2q2 
            if (o3 == 0 && OnSegment(p2, p1, q2))
            {
                return true;
            }

            // p2, q2 and q1 are colinear and 
            // q1 lies on segment p2q2 
            if (o4 == 0 && OnSegment(p2, q1, q2))
            {
                return true;
            }

            // Doesn't fall in any of the above cases 
            return false;
        }

        // Returns true if the point p lies  
        // inside the polygon[] with n vertices 
        private bool IsInside(DataPoint[] polygon, int n, DataPoint p)
        {
            // There must be at least 3 vertices in polygon[] 
            if (n < 3)
            {
                return false;
            }

            // Create a point for line segment from p to infinite 
            DataPoint extreme = new DataPoint(double.MaxValue, p.Y);

            // Count intersections of the above line  
            // with sides of polygon 
            int count = 0, i = 0;
            do
            {
                int next = (i + 1) % n;

                // Check if the line segment from 'p' to  
                // 'extreme' intersects with the line  
                // segment from 'polygon[i]' to 'polygon[next]' 
                if (DoIntersect(polygon[i], polygon[next], p, extreme))
                {
                    // If the point 'p' is colinear with line  
                    // segment 'i-next', then check if it lies  
                    // on segment. If it lies, return true, otherwise false 
                    if (Orientation(polygon[i], p, polygon[next]) == 0)
                    {
                        return OnSegment(polygon[i], p, polygon[next]);
                    }
                    count++;
                }
                i = next;
            } while (i != 0);

            // Return true if count is odd, false otherwise 
            return (count % 2 == 1); // Same as (count%2 == 1) 
        }

        #endregion

    }
}
