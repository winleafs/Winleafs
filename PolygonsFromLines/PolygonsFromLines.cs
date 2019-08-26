using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PolygonsFromLines
{
    public static class PolygonsFromLines
    {
        /*public static List<Polygon> Construct(IEnumerable<Line> lines, bool skipIntersectionRemoval = false)
        {

        }*/

        public static List<Line> RemoveIntersections(IList<Line> lines)
        {
            var intersections = new List<Intersection>();
            var result = new List<Line>();
            
            //Find intersections by comparing each line 1 time with each other line
            for (var i = 0; i < lines.Count - 1; i++) //-1 since we don't need the last one to compare with anything
            {
                var intersectionFound = false;

                for (var j = i + 1; j < lines.Count; j++)
                {
                    var intersectionPoint = GetIntersectionPoint(lines[i], lines[j]);

                    if (intersectionPoint != null)
                    {
                        intersectionFound = true;

                        intersections.Add(new Intersection
                        {
                            IntersectionPoint = intersectionPoint,
                            Line1 = lines[i],
                            Line2 = lines[j]
                        });
                    }
                }

                //This line intersects with no other line, we can safely add it to the result
                if (!intersectionFound)
                {
                    result.Add(lines[i]);
                }
            }

            //Get the intersection points for each line and cut it up
            foreach (var line in lines)
            {
                var intersectionsForLine = intersections.Where(intersection => intersection.Line1 == line || intersection.Line2 == line);


            }

            return result;
        }

        public static Point GetIntersectionPoint(Line line1, Line line2)
        {

        }
    }
}
