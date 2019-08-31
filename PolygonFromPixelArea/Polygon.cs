using System.Collections.Generic;
using System.Drawing;

namespace PolygonFromPixelArea
{
    public class Polygon
    {
        public List<Point> Points { get; set; }

        public Polygon()
        {
            Points = new List<Point>();
        }
    }
}
