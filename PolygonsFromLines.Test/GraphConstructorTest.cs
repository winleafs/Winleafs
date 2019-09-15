using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolygonsFromLines.Models;
using System.Collections.Generic;
using System.Drawing;

namespace PolygonsFromLines.Test
{
    [TestClass]
    public class GraphConstructorTest
    {
        [TestMethod]
        public void Test_GraphConstructor1()
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

            Assert.AreEqual(4, graph.Vertices.Count);
            Assert.AreEqual(1, graph.Vertices[0].Weights[0]);
            Assert.AreEqual(2, graph.Vertices[0].Neighbours.Count);
        }

        [TestMethod]
        public void Test_GraphConstructor2()
        {
            //A simple triangle
            var lines = new List<Line>
            {
                new Line(new PointF(0, 0), new PointF(4, 0)),
                new Line(new PointF(0, 0), new PointF(2, 4)),
                new Line(new PointF(2, 4), new PointF(4, 0))
            };

            var graph = GraphConstructor.ConstructGraphFromLines(lines);

            Assert.AreEqual(3, graph.Vertices.Count);
        }
    }
}
