using PolygonsFromLines;
using PolygonsFromLines.Models;
using System.Collections.Generic;
using System.Drawing;
using VoronoiLib;
using VoronoiLib.Structures;
using Winleafs.Wpf.Api.Layouts;

namespace Winleafs.Wpf.Helpers.Voronoi
{
    public static class VoronoiHelper
    {
        public static List<Line> ConstructScreenshotAreas(List<DrawablePanel> panels, int width, int height)
        {
            var points = new List<FortuneSite>();

            foreach (var panel in panels)
            {
                points.Add(new FortuneSite(panel.MidPoint.X, panel.MidPoint.Y)); //Prepare the points for the voronoi algorithm
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

            //Construct the polygons from the given edges
            PolygonConstructor.Construct(edges);

            ConstructPolygons(edges);

            return edges;
        }

        //https://web.ist.utl.pt/alfredo.ferreira/publications/12EPCG-PolygonDetection.pdf
        private static void ConstructPolygons(List<Line> edges)
        {

        }
    }
}
