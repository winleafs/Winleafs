using PolygonsFromLines.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PolygonsFromLines
{
    internal static class GraphConstructor
    {
        public static Graph ConstructGraphFromLines(IList<Line> lines)
        {
            var distinctPointsInLines = lines.Select(line => line.Start).ToList();
            distinctPointsInLines.AddRange(lines.Select(line => line.End));
            distinctPointsInLines = distinctPointsInLines.Distinct().ToList();

            var pointsWithNeighbouringPoints = new Dictionary<PointF, IEnumerable<PointF>>();

            foreach (var point in distinctPointsInLines)
            {
                var neighbouringPoints = lines.Where(line => line.Start == point).Select(line => line.End).ToList();
                neighbouringPoints.AddRange(lines.Where(line => line.End == point).Select(line => line.Start));

                pointsWithNeighbouringPoints.Add(point, neighbouringPoints);
            }

            //Depth first search to construct graph

            return null;
        }
    }
}
