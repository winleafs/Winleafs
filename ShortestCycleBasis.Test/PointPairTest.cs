using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShortestCycleBasis.Models;
using System.Drawing;

namespace ShortestCycleBasis.Test
{
    [TestClass]
    public class PointPairTest
    {
        [TestMethod]
        public void Test_GetHashCode()
        {
            //Test if two different object with the same points generate an equal hashcode
            var pointPair1 = new PointPair<PointF>(new PointF(0, 0), new PointF(1, 1));
            var pointPair2 = new PointPair<PointF>(new PointF(0, 0), new PointF(1, 1));

            Assert.AreEqual(pointPair1.GetHashCode(), pointPair2.GetHashCode());
        }
    }
}
