using System.Collections.Generic;

namespace PolygonsFromLines.Models
{
    internal class Graph
    {
        public IList<GraphVertex> Vertices { get; set; }

        public Graph()
        {
            Vertices = new List<GraphVertex>();
        }

        /// <summary>
        /// Dijkstra's algorithm to find the shortest path from any vertex to another vertex
        /// Returns the path or null if no path is found.
        /// Implementation based on:
        /// https://github.com/mburst/dijkstras-algorithm/blob/master/dijkstras.cs
        /// </summary>
        public List<GraphVertex> ShortestPath(GraphVertex start, GraphVertex finish)
        {
            var previous = new Dictionary<GraphVertex, GraphVertex>();
            var distances = new Dictionary<GraphVertex, double>();
            var nodes = new List<GraphVertex>();
            List<GraphVertex> path = null;

            foreach (var vertex in Vertices)
            {
                if (vertex.Equals(start))
                {
                    distances[vertex] = 0;
                }
                else
                {
                    distances[vertex] = double.MaxValue;
                }

                nodes.Add(vertex);
            }

            while (nodes.Count > 0)
            {
                nodes.Sort((x, y) => CompareVertices(x, y, distances));

                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (smallest.Equals(finish))
                {
                    path = new List<GraphVertex>();

                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    path.Add(start);
                    break;
                }

                if (distances[smallest] == double.MaxValue)
                {
                    break;
                }

                for (var i = 0; i < smallest.Neighbours.Count; i++)
                {
                    var alt = distances[smallest] + smallest.Weights[i];
                    if (alt < distances[smallest.Neighbours[i]])
                    {
                        distances[smallest.Neighbours[i]] = alt;
                        previous[smallest.Neighbours[i]] = smallest;
                    }
                }
            }

            return path;
        }

        //Compare vertices by their distances
        private int CompareVertices(GraphVertex x, GraphVertex y, Dictionary<GraphVertex, double> distances)
        {
            var distance = distances[x] - distances[y];
            return distance < 0 ? -1 : (distance > 0 ? 1 : 0);
        }
    }
}
