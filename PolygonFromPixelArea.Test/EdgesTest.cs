using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Drawing;

namespace PolygonFromPixelArea.Test
{
    [TestClass]
    public class EdgesTest
    {
        #region CreateEdges
        [TestMethod]
        public void Test_CreateEdges1()
        {
            //A linear increasing line
            var points = new List<Point>()
            {
                new Point(0, 0),
                new Point(1, 1),
                new Point(2, 2),
                new Point(3, 3)
            };

            var result = PolygonConstructor.CreateEdges(points);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(points[0], result[0]);
            Assert.AreEqual(points[3], result[1]);
        }

        [TestMethod]
        public void Test_CreateEdges2()
        {
            //A straight line
            var points = new List<Point>()
            {
                new Point(0, 0),
                new Point(1, 0),
                new Point(2, 0),
                new Point(3, 0)
            };

            var result = PolygonConstructor.CreateEdges(points);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(points[0], result[0]);
            Assert.AreEqual(points[3], result[1]);
        }

        [TestMethod]
        public void Test_CreateEdges3()
        {
            //A bridge shaped line
            var points = new List<Point>()
            {
                new Point(0, 0),
                new Point(1, 1),
                new Point(2, 2),
                new Point(3, 3),
                new Point(4, 3),
                new Point(5, 3),
                new Point(6, 2),
                new Point(7, 1),
                new Point(8, 0)
            };

            var result = PolygonConstructor.CreateEdges(points);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(points[0], result[0]);
            Assert.AreEqual(points[3], result[1]);
            Assert.AreEqual(points[5], result[2]);
            Assert.AreEqual(points[8], result[3]);
        }
        #endregion
    }
}
