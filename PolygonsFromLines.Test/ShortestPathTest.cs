using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolygonsFromLines.Models;
using System.Collections.Generic;
using System.Drawing;

namespace PolygonsFromLines.Test
{
    [TestClass]
    public class ShortestPathTest
    {
        [TestMethod]
        public void Test_ShortestPath1()
        {
            //A simple square
            var lines = new List<Line>
            {
                new Line(new PointF(0, 0), new PointF(0, 1)),
                new Line(new PointF(0, 0), new PointF(1, 0)),
                new Line(new PointF(0, 1), new PointF(1, 1)),
                new Line(new PointF(1, 0), new PointF(1, 1))
            };

            var graph = GraphConstructor.ConstructGraphFromLines(lines);

            //The first vertex is always 0,0 and the last vertex is always 2,2
            var path = graph.ShortestPath(graph.Vertices[0], graph.Vertices[3]);

            Assert.AreEqual(3, path.Count);
        }
    }
}
