using System.Collections.Generic;
using System.Drawing;

namespace PolygonsFromLines.Models
{
    internal class GraphVertex
    {
        public PointF Point { get; set; }

        public IList<GraphVertex> Neighbours { get; set; }

        public GraphVertex(PointF point)
        {
            Point = point;
            Neighbours = new List<GraphVertex>();
        }
    }
}
