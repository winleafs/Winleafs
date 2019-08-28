using PolygonsFromLines;
using PolygonsFromLines.Models;
using System.Collections.Generic;
using System.Drawing;
using VoronoiLib;
using VoronoiLib.Structures;

namespace LargestOrthogonalRectangleForVoronoi
{
    public static class LargestOrthogonalRectangleConstructor
    {
        //TODO: add some form of reference to the original points
        public static void Construct(IList<PointF> input, int minX, int minY, int maxX, int maxY)
        {
            //1. Make the voronoi diagram
            var points = new List<FortuneSite>();

            foreach (var point in input)
            {
                points.Add(new FortuneSite(point.X, point.Y)); //Prepare the points for the voronoi algorithm
            }

            var voronoiEdges = FortunesAlgorithm.Run(points, minX, minY, maxX, maxY); //Calculate the edges of the voronoi with the help of the library

            var edges = new List<Line>();

            foreach (var edge in voronoiEdges)
            {
                edges.Add(new Line(new PointF((float)edge.Start.X, (float)edge.Start.Y), new PointF((float)edge.End.X, (float)edge.End.Y)));
            }

            //Add the 4 edges, which represent the edges of the rectangle the voronoi is generated in
            edges.Add(new Line(new PointF(minX, minY), new PointF(minX, maxY)));
            edges.Add(new Line(new PointF(minX, minY), new PointF(maxX, minY)));
            edges.Add(new Line(new PointF(maxX, minY), new PointF(maxX, maxY)));
            edges.Add(new Line(new PointF(minX, maxY), new PointF(maxX, maxY)));

            //2. Construct the polygons from the given edges
            var polygons = PolygonConstructor.Construct(edges);

            //3. Find the largest rectangle in each polygon
        }
    }
}
