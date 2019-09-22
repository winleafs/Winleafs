using System;
using System.Collections.Generic;
using System.Drawing;

namespace PolygonsFromLines.Models
{
    public class GraphVertex
    {
        public PointF Point { get; set; }

        /// <summary>
        /// Neighbours with the weights of their edges
        /// </summary>
        public IDictionary<GraphVertex, double> Neighbours { get; set; }

        public GraphVertex(PointF point)
        {
            Point = point;
            Neighbours = new Dictionary<GraphVertex, double>();
        }

        /// <summary>
        /// Override Equals and GetHashCode for comparisons and usage in dictionaries
        /// Vertices are considered equal when their points are equal
        /// </summary>
        public override bool Equals(object obj)
        {
            return ((GraphVertex)obj).Point.Equals(Point);
        }

        public override int GetHashCode()
        {
            return Point.GetHashCode();
        }

        /// <summary>
        /// Adds a neighbour and sets the distance as calculated by the given distance function.
        /// if no distance function is given, euclidean distance is used
        /// </summary>
        public void AddNeighbour(GraphVertex newNeighbour, Func<GraphVertex, GraphVertex, double> distanceFunction = null)
        {
            if (distanceFunction == null)
            {
                distanceFunction = (vertex1, vertex2) => EuclideanDistance(vertex1, vertex2);
            }

            Neighbours.Add(newNeighbour, distanceFunction(this, newNeighbour));
        }

        /// <summary>
        /// Calculates the euclidean distance between two vertices
        /// </summary>
        private static double EuclideanDistance(GraphVertex vertex1, GraphVertex vertex2)
        {
            return Math.Sqrt(Math.Pow(vertex2.Point.X - vertex1.Point.X, 2) + Math.Pow(vertex2.Point.Y - vertex1.Point.Y, 2));
        }
    }
}
