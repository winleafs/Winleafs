using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShortestCycleBasis.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ShortestCycleBasis.Test
{
    [TestClass]
    public class ShortestPathTest
    {
        [TestMethod]
        public void Test_ShortestPath1()
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

            var path = graph.ShortestPath(vertex2, vertex6);

            Assert.AreEqual(3, path.Count);
            Assert.AreEqual(vertex2, path[0]);
            Assert.AreEqual(vertex7, path[1]);
            Assert.AreEqual(vertex6, path[2]);
        }
    }
}
