using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolygonsFromLines.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PolygonsFromLines.Test
{
    [TestClass]
    public class PolygonConstructorTest
    {
        [TestMethod]
        public void Test_PolygonConstructor1()
        {
            /*Construct a "window" graph
            * 1--2--3
            * |  |  |
            * 4--5--6
            * |  |  |
            * 7--8--9
            */
            
            var vertex1 = new GraphVertex(new PointF(0, 0));
            var vertex2 = new GraphVertex(new PointF(1, 0));
            var vertex3 = new GraphVertex(new PointF(2, 0));
            var vertex4 = new GraphVertex(new PointF(0, 1));
            var vertex5 = new GraphVertex(new PointF(1, 1));
            var vertex6 = new GraphVertex(new PointF(2, 1));
            var vertex7 = new GraphVertex(new PointF(0, 2));
            var vertex8 = new GraphVertex(new PointF(1, 2));
            var vertex9 = new GraphVertex(new PointF(2, 2));

            vertex1.AddNeighbour(vertex2);
            vertex1.AddNeighbour(vertex4);

            vertex2.AddNeighbour(vertex1);
            vertex2.AddNeighbour(vertex3);
            vertex2.AddNeighbour(vertex5);

            vertex3.AddNeighbour(vertex2);
            vertex3.AddNeighbour(vertex6);

            vertex4.AddNeighbour(vertex1);
            vertex4.AddNeighbour(vertex5);
            vertex4.AddNeighbour(vertex7);

            vertex5.AddNeighbour(vertex2);
            vertex5.AddNeighbour(vertex4);
            vertex5.AddNeighbour(vertex6);
            vertex5.AddNeighbour(vertex8);

            vertex6.AddNeighbour(vertex3);
            vertex6.AddNeighbour(vertex5);
            vertex6.AddNeighbour(vertex9);

            vertex7.AddNeighbour(vertex4);
            vertex7.AddNeighbour(vertex8);

            vertex8.AddNeighbour(vertex5);
            vertex8.AddNeighbour(vertex7);
            vertex8.AddNeighbour(vertex9);

            vertex9.AddNeighbour(vertex6);
            vertex9.AddNeighbour(vertex8);

            var graph = new Graph()
            {
                Vertices = new List<GraphVertex>() { vertex1, vertex2, vertex3, vertex4, vertex5, vertex6, vertex7, vertex8, vertex9 }
            };

            var polygons = PolygonConstructor.ConstructFromGraph(graph);

            Assert.AreEqual(4, polygons.Count());
            
            foreach (var polygon in polygons)
            {
                Assert.AreEqual(4, polygon.Points.Count);
            }            
        }
    }
}
