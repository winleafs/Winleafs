using PolygonsFromLines.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PolygonsFromLines
{
    internal static class IntersectionRemover
    {
        public static List<Line> RemoveIntersections(IList<Line> lines)
        {
            var intersections = new List<Intersection>();
            var result = new List<Line>();

            //Find intersections by comparing each line 1 time with each other line
            for (var i = 0; i < lines.Count - 1; i++) //-1 since we don't need the last one to compare with anything
            {
                for (var j = i + 1; j < lines.Count; j++)
                {
                    var intersectionPoint = GetIntersectionPoint(lines[i].Start, lines[i].End, lines[j].Start, lines[j].End);

                    if (intersectionPoint.Item1)
                    {
                        intersections.Add(new Intersection
                        {
                            IntersectionPoint = intersectionPoint.Item2,
                            Line1 = lines[i],
                            Line2 = lines[j]
                        });
                    }
                }
            }

            //Get the intersection points for each line and cut it up
            foreach (var line in lines)
            {
                //Get all intersection points for the current line. Intersection point is not counted when its equal to the start or end of the line.
                //Order the intersection points by X then by Y
                var intersectionsForLine = intersections
                                            .Where(i => (i.Line1 == line || i.Line2 == line) && !PointFEquals(i.IntersectionPoint, line.Start) && !PointFEquals(i.IntersectionPoint, line.End))
                                            .Select(ip => ip.IntersectionPoint)
                                            .OrderBy(ip => ip.X)
                                            .ThenBy(ip => ip.Y).ToList();

                if (intersectionsForLine.Count == 0)
                {
                    //This line intersects with no other line, we can safely add it to the result
                    result.Add(line);
                    continue;
                }

                //Also order the start and end of the line by X and Y so we can safely cut the line from the start to end
                var lineOrdered = new List<PointF>() { line.Start, line.End }.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
                line.Start = lineOrdered[0];
                line.End = lineOrdered[1];

                var currentStartPoint = line.Start;

                for (var i = 0; i < intersectionsForLine.Count; i++)
                {
                    result.Add(new Line(currentStartPoint, intersectionsForLine[i])); //Create the new line and add it to the result
                    currentStartPoint = intersectionsForLine[i];

                    if (i == intersectionsForLine.Count - 1)
                    {
                        //We have reached the last intersection, finish the line
                        result.Add(new Line(currentStartPoint, line.End));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Based on https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
        /// Determines if the lines p->p2 and q->q2 intersect
        /// </summary>
        /// <returns>True with the intersection point if any intersection is found</returns>
        private static (bool, PointF) GetIntersectionPoint(PointF p, PointF p2, PointF q, PointF q2)
        {
            PointF r = new PointF(p2.X - p.X, p2.Y - p.Y);
            PointF s = new PointF(q2.X - q.X, q2.Y - q.Y);

            var r_s = CrossProduct(r, s); // r x s
            var q_p_r = CrossProduct(SubstractPointFs(q, p), r); // (q − p) × r

            if (r_s == 0f && q_p_r == 0f)
            {
                var t1 = DotProduct(AddPointFs(q, SubstractPointFs(s, p)), r) / DotProduct(r, r);
                var t0 = t1 - DotProduct(s, r) / DotProduct(r, r);

                // Collinear and so intersect if they have any overlap, however overlapping is an ambiguous point so throw exception
                if (t0 >= 0f && t0 <= 1f || t1 >= 0f && t1 <= 1f)
                {
                    throw new ParallelLinesException($"Line1 start:{p.X};{p.Y} end:{p2.X};{p2.Y} is parallel and overlaps Line2 start:{q.X};{q.Y} end:{q2.X};{q2.Y}");
                }
                else
                {
                    // Collinear and do not overlap
                    return (false, new PointF(float.NaN, float.NaN));
                }

            }
            else if (r_s == 0f)
            {
                // Lines are parallel but not overlapping
                return (false, new PointF(float.NaN, float.NaN));
            }

            float t = CrossProduct(SubstractPointFs(q, p), s) / CrossProduct(r, s);
            float u = CrossProduct(SubstractPointFs(q, p), r) / CrossProduct(r, s);

            if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
            {
                var intersectionPoint = AddPointFs(p, MultiplyPointF(r, t)); // == AddPointFs(q, MultiplyPointF(s, u))
                return (true, intersectionPoint);
            }
            else
            {
                return (false, new PointF(float.NaN, float.NaN));
            }
        }

        private static PointF MultiplyPointF(PointF input, float multiplier)
        {
            return new PointF(input.X * multiplier, input.Y * multiplier);
        }

        private static PointF AddPointFs(PointF pointF1, PointF pointF2)
        {
            return new PointF(pointF1.X + pointF2.X, pointF1.Y + pointF2.Y);
        }

        private static PointF SubstractPointFs(PointF pointF1, PointF pointF2)
        {
            return new PointF(pointF1.X - pointF2.X, pointF1.Y - pointF2.Y);
        }

        private static float CrossProduct(PointF pointF1, PointF pointF2)
        {
            return pointF1.X * pointF2.Y - pointF1.Y * pointF2.X;
        }

        private static float DotProduct(PointF pointF1, PointF pointF2)
        {
            return pointF1.X * pointF2.X + pointF1.Y * pointF2.Y;
        }

        private static bool PointFEquals(PointF pointF1, PointF pointF2)
        {
            return pointF1.X == pointF2.X && pointF1.Y == pointF2.Y;
        }
    }
}
