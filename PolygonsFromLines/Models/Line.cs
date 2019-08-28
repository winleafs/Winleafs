using System.Drawing;

namespace PolygonsFromLines.Models
{
    public class Line
    {
        public Line(PointF start, PointF end)
        {
            Start = start;
            End = end;
        }

        public PointF Start { get; set; }
        public PointF End { get; set; }
    }
}
