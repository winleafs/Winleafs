using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace PolygonFromPixelArea.Test
{
    [TestClass]
    public class FindPatternsTest
    {
        #region FindPatterns
        [TestMethod]
        public void Test_FindPatterns1()
        {
            var sequence = new int[] { 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 2 };

            var result = PolygonConstructor.FindPatterns(sequence);

            Assert.IsTrue(result.SequenceEqual(new List<int>() { 8, 11 }));
        }

        [TestMethod]
        public void Test_FindPatterns2()
        {
            var sequence = new int[] { 1, 1, 1, 1, 2, 2 };

            var result = PolygonConstructor.FindPatterns(sequence);

            Assert.IsTrue(result.SequenceEqual(new List<int>() { 3, 5 }));
        }

        [TestMethod]
        public void Test_FindPatterns3()
        {
            var sequence = new int[] { 0, 1, 0, 1, 0, 1 };

            var result = PolygonConstructor.FindPatterns(sequence);

            Assert.IsTrue(result.SequenceEqual(new List<int>() { 5 }));
        }

        [TestMethod]
        public void Test_FindPatterns4()
        {
            var sequence = new int[] { 1, 1, 2, 2, 4, 4 };

            var result = PolygonConstructor.FindPatterns(sequence);

            Assert.IsTrue(result.SequenceEqual(new List<int>() { 1, 3, 5 }));
        }

        [TestMethod]
        public void Test_FindPatterns5()
        {
            var sequence = new int[] { 0, 0, 0, 0, 0 };

            var result = PolygonConstructor.FindPatterns(sequence);

            Assert.IsTrue(result.SequenceEqual(new List<int>() { 4 }));
        }
        #endregion

        #region FindPatternInSequence
        [TestMethod]
        public void Test_FindPatternInSequence1()
        {
            var sequence = new int[] { 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 2 };

            var result = PolygonConstructor.FindPatternInSequence(sequence);

            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public void Test_FindPatternInSequence2()
        {
            var sequence = new int[] { 1, 1, 1, 1, 2, 2 };

            var result = PolygonConstructor.FindPatternInSequence(sequence);

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void Test_FindPatternInSequence3()
        {
            var sequence = new int[] { 0, 1, 0, 1, 0, 1 };

            var result = PolygonConstructor.FindPatternInSequence(sequence);

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Test_FindPatternInSequence4()
        {
            var sequence = new int[] { 1, 1, 2, 2, 4, 4 };

            var result = PolygonConstructor.FindPatternInSequence(sequence);

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Test_FindPatternInSequence5()
        {
            var sequence = new int[] { 0, 0, 0, 0, 0 };

            var result = PolygonConstructor.FindPatternInSequence(sequence);

            Assert.AreEqual(4, result);
        }
        #endregion
    }
}
