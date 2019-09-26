using PolygonsFromLines.Models;
using System.Collections.Generic;
using System.Linq;

namespace PolygonsFromLines
{
    public static class PolygonConstructor
    {
        /// <summary>
        /// Creates a planar graph from the given set of lines, then finds all polygons in that graph
        /// TODO: Add running times to all methods
        /// </summary>
        public static IEnumerable<Polygon> ConstructFromLines(IList<Line> lines, bool skipIntersectionRemoval = false)
        {
            //Step 1: remove all intersections from the given lines O(n^2)
            if (!skipIntersectionRemoval)
            {
                lines = IntersectionRemover.RemoveIntersections(lines);
            }

            //Step 2: make a connected graph
            var graph = GraphConstructor.ConstructGraphFromLines(lines);

            //Step 3: find the polygons
            return ConstructFromGraph(graph);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph">A planar graph</param>
        public static IEnumerable<Polygon> ConstructFromGraph(Graph graph)
        {
            var polygons = new List<Polygon>();

            //We can use GraphEdge in a dictionairy since the custom implementation of GetHashCode makes combination of points unique (i.e. edge (0,0)-(1,1) is considered equal to (1,1)-(0,0))
            var edgesVisited = new Dictionary<GraphEdge, int>(); //Dictionairy to count how many times each edge is visited

            //Remove loose ends beforehand, this also ensures we have a graph where any vertex is part of a cycle (i.e all vertices have at elast 2 neighbours)
            GraphCleaner.RemoveLooseEnds(graph);

            //Order the vertices from top left to bottom right, so we know we always start searching for a polygon on a "corner" vertex of the graph (i.e. a vertex with exactly 2 neighbours)
            graph.Vertices = graph.Vertices.OrderBy(vertex => vertex.Point.X).ThenBy(vertex => vertex.Point.Y).ToList();

            while (graph.Vertices.Count > 2) //You need at least 3 vertices to form a polygon
            {
                var startVertex = graph.Vertices[0];
                var endVertex = startVertex.Neighbours.FirstOrDefault().Key;

                //Temporarily remove the relationship between the two when searching for the shortest path
                startVertex.Neighbours.Remove(endVertex);
                endVertex.Neighbours.Remove(startVertex);

                //Find the shortest path between the two
                var path = graph.ShortestPath(startVertex, endVertex);

                polygons.Add(new Polygon(path.Select(vertex => vertex.Point).ToList())); //Add the polygon

                //Add the temporarily removed path again
                startVertex.AddNeighbour(endVertex);
                endVertex.AddNeighbour(startVertex);

                //Increase (or add) the times visited for each edge on the path
                path.Add(startVertex); //By adding the start to the shortest path, we have a complete cycle so the last edge back to the start vertex is now included
                AddVisitations(path, edgesVisited);

                //Remove all edges that are visited twice
                RemoveVisitedEdgesFromGraph(graph, edgesVisited);

                //Clean the graph from loose ends
                GraphCleaner.RemoveLooseEnds(graph);
            }

            return polygons;
        }

        /// <summary>
        /// Removes all edges from the graph that are visited 2 or more times
        /// </summary>
        private static void RemoveVisitedEdgesFromGraph(Graph graph, Dictionary<GraphEdge, int> edgesVisited)
        {
            var edgesToDelete = edgesVisited.Where(kvp => kvp.Value >= 2).ToList();

            foreach (var keyValuePair in edgesToDelete)
            {
                var vertex1 = keyValuePair.Key.Point1;
                var vertex2 = keyValuePair.Key.Point2;

                vertex1.Neighbours.Remove(vertex2);
                vertex2.Neighbours.Remove(vertex1);

                //Remove the dege from edgesVisited since we know it will never be visited again
                edgesVisited.Remove(keyValuePair.Key);
            }
        }

        /// <summary>
        /// For each edge in the path, increase its amount of visitations
        /// </summary>
        private static void AddVisitations(IList<GraphVertex> path, Dictionary<GraphEdge, int> edgesVisited)
        {
            for (var i = 0; i < path.Count - 1; i++) //-1 since we visit two vertices per loop
            {
                var edge = new GraphEdge(path[i], path[i + 1]);

                if (edgesVisited.ContainsKey(edge))
                {
                    if (i == 0 || i == path.Count - 1)
                    {
                        //Special case: since we know the start vertex (residing at both i = 0 and i = n - 1) is a "corner" vertex, we know it can only be part of 1 cycle
                        //hence it can be deleted after the creation of this polygon, so therefore set the edgesVisited to 2 for all edges that include the start vertex
                        edgesVisited[edge] = 2;
                    }
                    else
                    {
                        edgesVisited[edge]++;
                    }
                }
                else
                {
                    edgesVisited.Add(edge, i == 0 || i == path.Count - 1 ? 2 : 1); //Same special case as above
                }
            }
        }
    }
}
