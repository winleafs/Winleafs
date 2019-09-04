using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolygonsFromLines.Models;
using System.Collections.Generic;
using System.Drawing;

namespace PolygonsFromLines.Test
{
    [TestClass]
    public class IntersectionRemoverTest
    {
        [TestMethod]
        public void Non_intersecting_corner()
        {
            var lines = new List<Line>
            {
                new Line(new PointF(0, 0), new PointF(50, 0)),
                new Line(new PointF(0, 0), new PointF(0, 50))
            };

            lines = IntersectionRemover.RemoveIntersections(lines);

            Assert.AreEqual(2, lines.Count);
        }

        [TestMethod]
        public void Intersecting_with_endpoint()
        {
            var lines = new List<Line>
            {
                new Line(new PointF(0, 0), new PointF(50, 0)),
                new Line(new PointF(25, 0), new PointF(25, 25))
            };

            lines = IntersectionRemover.RemoveIntersections(lines);

            Assert.AreEqual(3, lines.Count);
        }

        [TestMethod]
        public void Intersecting()
        {
            var lines = new List<Line>
            {
                new Line(new PointF(-50, 0), new PointF(50, 0)),
                new Line(new PointF(0, -50), new PointF(0, 50))
            };

            lines = IntersectionRemover.RemoveIntersections(lines);

            Assert.AreEqual(4, lines.Count);
        }

        [TestMethod]
        public void Paralell_and_overlapping()
        {
            var lines = new List<Line>
            {
                new Line(new PointF(0, 0), new PointF(50, 0)),
                new Line(new PointF(-10, 0), new PointF(10, 0))
            };

            Assert.ThrowsException<ParallelLinesException>(() => IntersectionRemover.RemoveIntersections(lines));
        }

        [TestMethod]
        public void Parallel_not_overlapping()
        {
            var lines = new List<Line>
            {
                new Line(new PointF(0, 0), new PointF(10, 0)),
                new Line(new PointF(10.1f, 0), new PointF(20, 0))
            };

            lines = IntersectionRemover.RemoveIntersections(lines);

            Assert.AreEqual(2, lines.Count);
        }
    }
}
