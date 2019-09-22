using PolygonsFromLines.Models;
using System.Collections.Generic;
using System.Linq;

namespace PolygonsFromLines
{
    public static class PolygonConstructor
    {
        public static List<Polygon> ConstructFromLines(IList<Line> lines, bool skipIntersectionRemoval = false)
        {
            //Step 1: remove all intersections from the given lines O(n^2)
            lines = IntersectionRemover.RemoveIntersections(lines);

            //Step 2: make a connected graph
            var graph = GraphConstructor.ConstructGraphFromLines(lines);

            //Step 3: find the shortest cycles recursively

            return null;
        }



    }
}
