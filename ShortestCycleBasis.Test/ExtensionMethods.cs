using ShortestCycleBasis.Models;
using System;
using System.Drawing;

namespace ShortestCycleBasis.Test
{
    public static class ExtensionMethods
    {
        public static void AddNeighbour(this GraphVertex<PointF> graphVertex, GraphVertex<PointF> neighbour)
        {
            graphVertex.Neighbours.Add(neighbour);
            graphVertex.Weights.Add(EuclideanDistance(graphVertex.Point, neighbour.Point));

        }

        /// <summary>
        /// Calculates the euclidean distance between two points
        /// </summary>
        private static double EuclideanDistance(PointF point1, PointF point2)
        {
            return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
        }
    }
}
