using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolygonsFromLines.Models;
using System.Collections.Generic;
using System.Drawing;

namespace PolygonsFromLines.Test
{
    [TestClass]
    public class GraphCleanerTest
    {
        [TestMethod]
        public void Test_GraphCleaner1()
        {
            /*
             * Construct the following graph
             * 1--2
             * |  |
             * 3--4
             * |  |
             * 5  6
             * |
             * 7
             * 
             * Result after removing loose ends shoudl be:
             * 1--2
             * |  |
             * 3--4
             */

            var vertex1 = new GraphVertex(new PointF(0, 0));
            var vertex2 = new GraphVertex(new PointF(1, 0));
            var vertex3 = new GraphVertex(new PointF(0, 1));
            var vertex4 = new GraphVertex(new PointF(1, 1));
            var vertex5 = new GraphVertex(new PointF(0, 2));
            var vertex6 = new GraphVertex(new PointF(1, 2));
            var vertex7 = new GraphVertex(new PointF(0, 3));

            vertex1.AddNeighbour(vertex2);
            vertex1.AddNeighbour(vertex3);

            vertex2.AddNeighbour(vertex1);
            vertex2.AddNeighbour(vertex4);

            vertex3.AddNeighbour(vertex1);
            vertex3.AddNeighbour(vertex4);
            vertex3.AddNeighbour(vertex5);

            vertex4.AddNeighbour(vertex2);
            vertex4.AddNeighbour(vertex3);
            vertex4.AddNeighbour(vertex6);

            vertex5.AddNeighbour(vertex3);
            vertex5.AddNeighbour(vertex7);

            vertex6.AddNeighbour(vertex4);

            vertex7.AddNeighbour(vertex5);

            var graph = new Graph()
            {
                Vertices = new List<GraphVertex>() { vertex1, vertex2, vertex3, vertex4, vertex5, vertex6, vertex7 }
            };

            GraphCleaner.RemoveLooseEnds(graph);

            Assert.AreEqual(4, graph.Vertices.Count);

            foreach (var vertex in graph.Vertices)
            {
                Assert.AreEqual(2, vertex.Neighbours.Count);
            }
        }
    }
}
