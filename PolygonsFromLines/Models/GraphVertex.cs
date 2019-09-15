using System.Collections.Generic;
using System.Drawing;

namespace PolygonsFromLines.Models
{
    internal class GraphVertex
    {
        public PointF Point { get; set; }

        public IList<GraphVertex> Neighbours { get; set; }
        public IList<double> Weights { get; set; }

        public GraphVertex(PointF point)
        {
            Point = point;
            Neighbours = new List<GraphVertex>();
            Weights = new List<double>();
        }

        /// <summary>
        /// Override Equals and GetHashCode for comparisons and usage in dictionaries
        /// Vertices are considered equal when their points are equal
        /// </summary>
        public override bool Equals(object obj)
        {
            return ((GraphVertex)obj).Point == Point;
        }

        public override int GetHashCode()
        {
            return Point.GetHashCode();
        }
    }
}
