using PolygonsFromLines.Models;
using System.Collections.Generic;
using System.Linq;

namespace PolygonsFromLines
{
    internal static class GraphCleaner
    {
        /// <summary>
        /// Given a graph, removes all loose ends (vertexes with only 1 edge) from that graph recursively
        /// Running time?
        /// </summary>
        public static void RemoveLooseEnds(Graph graph)
        {
            var looseEnds = graph.Vertices.Where(vertex => vertex.Neighbours.Count == 1);

            foreach (var vertex in looseEnds)
            {
                //Recursively delete the current vertex from the neighbour's neighbours
                RecursivelyRemoveVertexFromFromLastNeighbour(vertex, vertex.Neighbours.FirstOrDefault().Key);
            }

            var looseVertices = graph.Vertices.Where(vertex => vertex.Neighbours.Count == 0);

            foreach (var vertex in looseVertices)
            {
                graph.Vertices.Remove(vertex);
            }
        }

        private static void RecursivelyRemoveVertexFromFromLastNeighbour(GraphVertex vertex, GraphVertex neighbour)
        {
            neighbour.Neighbours.Remove(vertex);

            if (neighbour.Neighbours.Count == 1)
            {
                //If the neighbour now only has 1 neighbour left, also delete that one
                RecursivelyRemoveVertexFromFromLastNeighbour(neighbour, neighbour.Neighbours.FirstOrDefault().Key);
            }
        }
    }
}
