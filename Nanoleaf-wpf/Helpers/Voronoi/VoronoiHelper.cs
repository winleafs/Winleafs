using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VoronoiLib;
using VoronoiLib.Structures;
using Winleafs.Wpf.Api.Layouts;

namespace Winleafs.Wpf.Helpers.Voronoi
{
    public static class VoronoiHelper
    {
        public static void ConstructScreenshotAreas(List<DrawablePanel> panels, int width, int height)
        {
            var points = new List<FortuneSite>();

            foreach (var panel in panels)
            {
                points.Add(new FortuneSite(panel.MidPoint.X, panel.MidPoint.Y)); //Prepare the points for the voronoi algorithm
            }

            var voronoiEdges = FortunesAlgorithm.Run(points, 0, 0, width, height); //Calculate the edges with the help of the library

            var edges = new List<Edge>();

            foreach (var edge in voronoiEdges)
            {
                edges.Add(new Edge
                {
                    Start = new Point(edge.Start.X, edge.Start.Y),
                    End = new Point(edge.End.X, edge.End.Y)
                });
            }

            //Add the 4 edges, which represent the edges of the screen
            edges.Add(new Edge
            {
                Start = new Point(0, 0),
                End = new Point(0, height)
            });
            edges.Add(new Edge
            {
                Start = new Point(0, 0),
                End = new Point(width, 0)
            });
            edges.Add(new Edge
            {
                Start = new Point(width, 0),
                End = new Point(width, height)
            });
            edges.Add(new Edge
            {
                Start = new Point(0, height),
                End = new Point(width, height)
            });

            ConstructPolygons(edges);
        }

        //https://web.ist.utl.pt/alfredo.ferreira/publications/12EPCG-PolygonDetection.pdf
        private static void ConstructPolygons(List<Edge> edges)
        {

        }
    }
}
