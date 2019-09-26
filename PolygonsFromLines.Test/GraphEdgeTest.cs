using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolygonsFromLines.Models;
using System.Drawing;

namespace PolygonsFromLines.Test
{
    [TestClass]
    public class GraphEdgeTest
    {
        [TestMethod]
        public void Test_GetHashCode1()
        {            
            //Test if two different object with the same points generate an equal hashcode
            var pointPair1 = new GraphEdge(new GraphVertex(new PointF(0, 0)), new GraphVertex(new PointF(1, 1)));
            var pointPair2 = new GraphEdge(new GraphVertex(new PointF(0, 0)), new GraphVertex(new PointF(1, 1)));

            Assert.AreEqual(pointPair1.GetHashCode(), pointPair2.GetHashCode());
        }

        [TestMethod]
        public void Test_GetHashCode2()
        {
            //Test if two different object with the same points generate an equal hashcode
            //However, the order is switched and that should not matter
            var pointPair1 = new GraphEdge(new GraphVertex(new PointF(0, 0)), new GraphVertex(new PointF(1, 1)));
            var pointPair2 = new GraphEdge(new GraphVertex(new PointF(1, 1)), new GraphVertex(new PointF(0, 0)));

            Assert.AreEqual(pointPair1.GetHashCode(), pointPair2.GetHashCode());
        }

        [TestMethod]
        public void Test_GetHashCode3()
        {
            //These two pairs should generate a different hashcode
            var pointPair1 = new GraphEdge(new GraphVertex(new PointF(0, 0)), new GraphVertex(new PointF(1, 1)));
            var pointPair2 = new GraphEdge(new GraphVertex(new PointF(0, 0)), new GraphVertex(new PointF(0, 1)));

            Assert.AreNotEqual(pointPair1.GetHashCode(), pointPair2.GetHashCode());
        }
    }
}
