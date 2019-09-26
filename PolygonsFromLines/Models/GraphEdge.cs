using System.Drawing;

namespace PolygonsFromLines.Models
{
    internal class GraphEdge
    {
        public GraphVertex Point1 { get; private set; }
        public GraphVertex Point2 { get; private set; }

        public GraphEdge(GraphVertex point1, GraphVertex point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        /// <summary>
        /// Override Equals and GetHashCode for comparisons and usage in dictionaries.
        /// Vertices are considered equal when their points are equal, as implemented in the overridden functions in <see cref="GraphVertex"/>.
        /// </summary>
        public override bool Equals(object obj)
        {
            var graphEdge = (GraphEdge)obj;
            return graphEdge.Point1.Equals(Point1) && graphEdge.Point2.Equals(Point2);
        }

        /// <summary>
        /// Generates a hashcode where the order of points does not matter
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = 0x2D2816FE;

            hashCode = hashCode ^ 31 + Point1.GetHashCode();
            hashCode = hashCode ^ 31 + Point2.GetHashCode();

            return hashCode;
        }
    }
}
