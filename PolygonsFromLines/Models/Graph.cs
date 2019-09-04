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
    }
}
