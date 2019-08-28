using PolygonsFromLines.Models;
using System.Collections.Generic;

namespace PolygonsFromLines
{
    public static class PolygonConstructor
    {
        public static List<Polygon> Construct(IList<Line> lines, bool skipIntersectionRemoval = false)
        {
            //Step 1: remove all intersections from the given lines
            lines = IntersectionRemover.RemoveIntersections(lines);

            return null;
        }        
    }
}
