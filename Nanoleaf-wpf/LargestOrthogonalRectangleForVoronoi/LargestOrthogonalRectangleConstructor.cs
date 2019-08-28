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
        public static void Construct(IList<PointF> input, int width, int height)
        {
            //1. Make the voronoi diagram
            var points = new List<FortuneSite>();

            foreach (var point in input)
            {
                points.Add(new FortuneSite(point.X, point.Y)); //Prepare the points for the voronoi algorithm
            }

            var voronoiEdges = FortunesAlgorithm.Run(points, 0, 0, width, height); //Calculate the edges of the voronoi with the help of the library

            var edges = new List<Line>();

            foreach (var edge in voronoiEdges)
            {
                edges.Add(new Line(new PointF((float)edge.Start.X, (float)edge.Start.Y), new PointF((float)edge.End.X, (float)edge.End.Y)));
            }

            //Add the 4 edges, which represent the edges of the screen
            edges.Add(new Line(new PointF(0, 0), new PointF(0, height)));
            edges.Add(new Line(new PointF(0, 0), new PointF(width, 0)));
            edges.Add(new Line(new PointF(width, 0), new PointF(width, height)));
            edges.Add(new Line(new PointF(0, height), new PointF(width, height)));

            //2. Construct the polygons from the given edges
            var polygons = PolygonConstructor.Construct(edges);

            //3. Find the largest rectangle in each polygon
        }
    }
}
