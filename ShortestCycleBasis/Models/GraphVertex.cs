using System;
using System.Collections.Generic;
using System.Drawing;

namespace ShortestCycleBasis.Models
{
    public class GraphVertex<PointType>
    {
        public PointType Point { get; set; }

        public IList<GraphVertex<PointType>> Neighbours { get; set; }
        public IList<double> Weights { get; set; }

        public GraphVertex(PointType point)
        {
            Point = point;
            Neighbours = new List<GraphVertex<PointType>>();
            Weights = new List<double>();
        }

        /// <summary>
        /// Override Equals and GetHashCode for comparisons and usage in dictionaries
        /// Vertices are considered equal when their points are equal
        /// </summary>
        public override bool Equals(object obj)
        {
            return ((GraphVertex<PointType>)obj).Point.Equals(Point);
        }

        public override int GetHashCode()
        {
            return Point.GetHashCode();
        }
    }
}
