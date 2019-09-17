using ShortestCycleBasis.Models;
using System.Collections.Generic;

namespace ShortestCycleBasis
{
    public static class CycleBasis
    {
        /// <summary>
        /// A polynomial time algorithm to find the shortest cycle basis of a graph.
        /// Implementation of: https://dl.acm.org/citation.cfm?id=33351
        /// </summary>
        public static void ConstructShortestCycleBasis<PointType>(Graph<PointType> graph)
        {
            //1. Find minimum paths between all pairs of vertices
            var paths = FindMinimumPaths(graph);
        }

        /// <summary>
        /// For each pair of vertices, find the shortest (minimum weight) path
        /// </summary>
        internal static Dictionary<PointPair<PointType>, IList<GraphVertex<PointType>>> FindMinimumPaths<PointType>(Graph<PointType> graph)
        {
            var paths = new Dictionary<PointPair<PointType>, IList<GraphVertex<PointType>>>();

            for (var i = 0; i < graph.Vertices.Count - 1; i++) //-1 since we don't need to compare the last element with anything
            {
                for (var j = i + 1; j < graph.Vertices.Count; j++)
                {
                    paths.Add(
                        new PointPair<PointType>(graph.Vertices[i].Point, graph.Vertices[j].Point),
                        graph.ShortestPath(graph.Vertices[i], graph.Vertices[j])
                        );
                }
            }

            return paths;
        }
    }
}
