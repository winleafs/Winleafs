using System.Collections.Generic;
using System.Drawing;

namespace PolygonsFromLines.Models
{
    internal class ConstructableVertex
    {
        public PointF Point { get; set; }
        public IEnumerable<PointF> NeighbouringPoints { get; set; }
    }
}
