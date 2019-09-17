using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShortestCycleBasis.Models;
using System.Collections.Generic;
using System.Drawing;

namespace ShortestCycleBasis.Test
{
    [TestClass]
    public class FindMinimumPathsTest
    {
        [TestMethod]
        public void Test_FindMinimumPaths1()
        {
            var graph = new Graph<PointF>();

            //A malformed hexagon
            var vertex1 = new GraphVertex<PointF>(new PointF(1, 0));
            var vertex2 = new GraphVertex<PointF>(new PointF(2, 0));
            var vertex3 = new GraphVertex<PointF>(new PointF(-0.5f, 1.5f));
            var vertex4 = new GraphVertex<PointF>(new PointF(3, 1.5f));
            var vertex5 = new GraphVertex<PointF>(new PointF(0.5f, 3));
            var vertex6 = new GraphVertex<PointF>(new PointF(2.5f, 4));
            var vertex7 = new GraphVertex<PointF>(new PointF(2, 1.5f));

            //Add each neighbour with the weight being the distance between the current point and the neighbour
            vertex1.AddNeighbour(vertex2);
            vertex1.AddNeighbour(vertex3);
            vertex1.AddNeighbour(vertex7);

            vertex2.AddNeighbour(vertex1);
            vertex2.AddNeighbour(vertex4);
            vertex2.AddNeighbour(vertex7);

            vertex3.AddNeighbour(vertex1);
            vertex3.AddNeighbour(vertex5);
            vertex3.AddNeighbour(vertex7);

            vertex4.AddNeighbour(vertex2);
            vertex4.AddNeighbour(vertex6);
            vertex4.AddNeighbour(vertex7);

            vertex5.AddNeighbour(vertex3);
            vertex5.AddNeighbour(vertex6);
            vertex5.AddNeighbour(vertex7);

            vertex6.AddNeighbour(vertex4);
            vertex6.AddNeighbour(vertex5);
            vertex6.AddNeighbour(vertex7);

            vertex7.AddNeighbour(vertex1);
            vertex7.AddNeighbour(vertex2);
            vertex7.AddNeighbour(vertex3);
            vertex7.AddNeighbour(vertex4);
            vertex7.AddNeighbour(vertex5);
            vertex7.AddNeighbour(vertex6);

            graph.Vertices = new List<GraphVertex<PointF>>() { vertex1, vertex2, vertex3, vertex4, vertex5, vertex6, vertex7 };

            var paths = CycleBasis.FindMinimumPaths(graph);

            Assert.AreEqual(21, paths.Count);

            var key = new PointPair<PointF>(vertex2.Point, vertex5.Point);

            Assert.AreEqual(3, paths[key].Count);
            Assert.AreEqual(vertex2, paths[key][0]);
            Assert.AreEqual(vertex7, paths[key][1]);
            Assert.AreEqual(vertex5, paths[key][2]);
        }
    }
}
