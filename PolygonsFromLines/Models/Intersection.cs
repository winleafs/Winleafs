using System.Drawing;

namespace PolygonsFromLines.Models
{
    internal class Intersection
    {
        public Line Line1 { get; set; }
        public Line Line2 { get; set; }

        public PointF IntersectionPoint { get; set; }
    }
}
