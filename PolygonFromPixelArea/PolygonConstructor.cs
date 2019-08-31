using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PolygonFromPixelArea
{
    public static class PolygonConstructor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixelArea"></param>
        /// <returns></returns>
        /// <remarks>
        /// Total running time:
        /// O(4n) + O(n log n) + O((3n - 1) + m_1) + O((3n - 1) + m_2) + O(m)
        /// where m is the number of corner points in the polygon,
        /// m_1 the number of corner points in the upper half of the polygon
        /// m_2 the number of corner points in the lower half of the polygon
        /// </remarks>
        public static Polygon Construct(IList<Point> pixelArea)
        {
            //Group the pixels on X value, then make a list of all highest and lowest Y values
            var grouping = pixelArea.GroupBy(pixel => pixel.X).OrderBy(group => group.Key); //O(n) + O(n log n) (n log n average, n^2 wordt case)
            var upperEdgesPoints = grouping.Select(group => new Point(group.Key, group.Max(p => p.Y))).ToList(); //O(n)
            var lowerEdgesPoints = grouping.Select(group => new Point(group.Key, group.Min(p => p.Y))).ToList(); //O(n)

            var upperEdges = CreateEdges(upperEdgesPoints); //O((3n - 1) + m)
            var lowerEdges = CreateEdges(lowerEdgesPoints); //O((3n - 1) + m)
            lowerEdges.Reverse(); //Reverse since we are constructing the polygon clockwise and the lowerEdges are from left to right, O(n)

            var polygon = new Polygon();
            polygon.Points.AddRange(upperEdges); //Add the points from the top left point to the top right point of the polygon 

            if (polygon.Points.Last().Equals(lowerEdges.First()))
            {
                //Case: right side of polygon ends in a single point
                lowerEdges.RemoveAt(0); //Remove the first element in lowerEdges otherwise that point would appear double in the polygon
            }

            if (polygon.Points.First().Equals(lowerEdges.Last()))
            {
                //Case: left side of polygon ends in a single point
                lowerEdges.RemoveAt(lowerEdges.Count - 1); //Remove the last element in lowerEdges otherwise that point would appear double in the polygon
            }

            polygon.Points.AddRange(lowerEdges); //Add the points from the lower right point to the lower left point of the polygon

            //Polygon is now connected
            return polygon;
        }

        /// <summary>
        /// Return a list of points that mark the corner points along the given set of point in an edge
        /// </summary>
        /// <remarks>
        /// O((3n - 1) + m) where m is the number corners
        /// </remarks>
        internal static List<Point> CreateEdges(IList<Point> pointsInEdge)
        {
            var elevationDifferences = new int[pointsInEdge.Count - 1]; //-1 since the last one has no elevation difference

            //Create a list of elevation differences
            for (var i = 0; i < pointsInEdge.Count - 1; i++)
            {
                elevationDifferences[i] = Math.Abs(pointsInEdge[i + 1].Y) - Math.Abs(pointsInEdge[i].Y);
            }

            //Find all indexes on which there should be a corner in the edge
            var cornerIndexes = FindPatterns(elevationDifferences); //O(2n)

            var points = new List<Point>() { pointsInEdge[0] };

            foreach (var cornerIndex in cornerIndexes)
            {
                points.Add(pointsInEdge[cornerIndex + 1]); //+1 since the length of the elevationDifferences is one less than the number of points
            }

            return points;
        }

        /// <summary>
        /// Searches and report all end indexes of patterns in the given sequence of numbers
        /// </summary>
        /// <remarks>
        /// O(2n) complexity since it processes the whole sequence only once (the 2 is from <see cref="FindPatternInSequence"/>)
        /// </remarks>
        internal static List<int> FindPatterns(int[] sequence)
        {
            //Find the first pattern in the sequence
            var patternIndex = FindPatternInSequence(sequence);
            var patternIndexes = new List<int>() { patternIndex };

            while (patternIndex < sequence.Length - 2) //-2 since the last number can't be a pattern on its own
            {
                var currentSequence = new int[sequence.Length - patternIndex - 1];
                Array.Copy(sequence, patternIndex + 1, currentSequence, 0, sequence.Length - patternIndex - 1);
                patternIndex += FindPatternInSequence(currentSequence) + 1;

                patternIndexes.Add(patternIndex);
            }

            if (patternIndex == sequence.Length - 2)
            {
                //If we have a floating remaining pixel, we include it in the last pattern
                patternIndexes[patternIndexes.Count - 1]++;
            }

            return patternIndexes;
        }

        /// <summary>
        /// Search for the first pattern in a sequence. A pattern in this case is a series of equal numbers followed by one other number (e.g. 0001)
        /// Once such a pattern is found, the function checks how many times it appears and return the last index.
        /// If only a single instance of the pattern is found, we do not return the last index of the pattern, but one less.
        /// This is because in that case, the last digit could be the start of the following pattern
        /// </summary>
        /// <remarks>
        /// O(2n) complexity, one to loop the whole array, one to copy the array
        /// </remarks>
        internal static int FindPatternInSequence(int[] sequence)
        {
            var firstNumber = sequence[0];

            //Special case: all elements are equal, it is one long pattern
            if (sequence.All(number => number == firstNumber))
            {
                return sequence.Length - 1;
            }

            var patternLength = 0;

            for (int i = 1; i < sequence.Length; i++)
            {
                if (firstNumber != sequence[i])
                {
                    patternLength = i + 1;
                    break;
                }
            }

            //The pattern is too large to repeat itself within the current sequence
            if (patternLength + patternLength > sequence.Length)
            {
                //-1 since arrays start at 0 and another -1 since there is only 1 pattern. We want to exclude the last element, since that one marks the start of a new pattern
                return patternLength - 2;
            }

            var pattern = new int[patternLength];
            Array.Copy(sequence, 0, pattern, 0, patternLength);

            var j = patternLength;
            var patternEndIndex = patternLength - 1;
            var numberOfPatterns = 1;

            //Check how many times we can find the repeating pattern
            while (j + patternLength <= sequence.Length)
            {
                var currentPattern = new int[patternLength];
                Array.Copy(sequence, j, currentPattern, 0, patternLength);

                if (!pattern.SequenceEqual(currentPattern))
                {
                    break;
                }

                j += patternLength;
                patternEndIndex = j - 1;
                numberOfPatterns++;
            }

            if (numberOfPatterns == 1)
            {
                //If there is only 1 pattern, we want to exclude the last element, since that one marks the start of a new pattern
                return patternEndIndex - 1;
            }
            else
            {
                return patternEndIndex;
            }
        }
    }
}
