using System.Collections.Generic;

namespace ShortestCycleBasis.Models
{
    public class Graph<PointType>
    {
        public IList<GraphVertex<PointType>> Vertices { get; set; }

        public Graph()
        {
            Vertices = new List<GraphVertex<PointType>>();
        }

        /// <summary>
        /// Dijkstra's algorithm to find the shortest path from any vertex to another vertex
        /// Returns the path or null if no path is found.
        /// Implementation based on:
        /// https://github.com/mburst/dijkstras-algorithm/blob/master/dijkstras.cs
        /// </summary>
        public IList<GraphVertex<PointType>> ShortestPath(GraphVertex<PointType> start, GraphVertex<PointType> finish)
        {
            var previous = new Dictionary<GraphVertex<PointType>, GraphVertex<PointType>>();
            var distances = new Dictionary<GraphVertex<PointType>, double>();
            var nodes = new List<GraphVertex<PointType>>();
            List<GraphVertex<PointType>> path = null;

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
                    path = new List<GraphVertex<PointType>>();

                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    path.Add(start);
                    path.Reverse(); //Reverse since we add the steps backwards
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
        private int CompareVertices(GraphVertex<PointType> x, GraphVertex<PointType> y, Dictionary<GraphVertex<PointType>, double> distances)
        {
            var distance = distances[x] - distances[y];
            return distance < 0 ? -1 : (distance > 0 ? 1 : 0);
        }
    }
}
