using PolygonsFromLines.Models;
using System;
using System.Collections.Generic;

namespace PolygonsFromLines
{
    /// <summary>
    /// https://web.ist.utl.pt/alfredo.ferreira/publications/12EPCG-PolygonDetection.pdf
    /// </summary>
    public static class PolygonConstructor
    {
        public static List<Polygon> Construct(IList<Line> lines, bool skipIntersectionRemoval = false)
        {
            //Step 1: remove all intersections from the given lines O(n^2)
            lines = IntersectionRemover.RemoveIntersections(lines);

            //Step 2: make a connected graph

            //Step 3: find the shortest cycle basis

            return null;
        }
    }
}
