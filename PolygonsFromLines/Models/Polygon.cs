using System.Collections.Generic;
using System.Drawing;

namespace PolygonsFromLines.Models
{
    public class Polygon
    {
        public IList<PointF> Points { get; private set; }

        public Polygon(IList<PointF> points)
        {
            Points = points;
        }
    }
}
