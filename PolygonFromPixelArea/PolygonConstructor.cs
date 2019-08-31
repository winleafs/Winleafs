using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PolygonFromPixelArea
{
    public static class PolygonConstructor
    {
        public static void Construct(IList<Point> pixelArea)
        {
            //Group the pixels on X value, then make a list of all highest and lowest Y values
            var grouping = pixelArea.GroupBy(pixel => pixel.X).OrderBy(group => group.Key);
            var upperEdges = grouping.Select(group => new Point(group.Key, group.Max(p => p.Y))).ToList();
            var lowerEdges = grouping.Select(group => new Point(group.Key, group.Min(p => p.Y))).ToList();


        }

        internal static List<Point> CreateEdges(IList<Point> pointsInEdges)
        {
            var elevationDifferences = new int[pointsInEdges.Count - 1]; //-1 since the last one has no elevation difference

            //Create a list of elevation differences
            for (var i = 0; i < pointsInEdges.Count - 1; i++)
            {
                elevationDifferences[i] = Math.Abs(pointsInEdges[i + 1].Y) - Math.Abs(pointsInEdges[i].Y);
            }

            var cornerIndexes = FindPatterns(elevationDifferences);

            var pointsInPolygon = new List<Point>() { pointsInEdges[0] };

            foreach (var cornerIndex in cornerIndexes)
            {
                pointsInPolygon.Add(pointsInEdges[cornerIndex + 1]); //+1 since the length of the elevationDifferences is one less than the number of points
            }

            return pointsInPolygon;
        }

        internal static List<int> FindPatterns(int[] sequence)
        {
            var patternIndex = FindPatternInSequence(sequence);
            var patternIndexes = new List<int>() { patternIndex };

            while (patternIndex < sequence.Length - 2)
            {
                var currentSequence = new int[sequence.Length - patternIndex - 1];
                Array.Copy(sequence, patternIndex + 1, currentSequence, 0, sequence.Length - patternIndex - 1);
                patternIndex += FindPatternInSequence(currentSequence) + 1;

                patternIndexes.Add(patternIndex);
            }

            if (patternIndex == sequence.Length - 2)
            {
                //If we have a floating remaing pixel, we include it in the last pattern
                patternIndexes[patternIndexes.Count - 1]++;
            }

            return patternIndexes;
        }

        internal static int FindPatternInSequence(int[] sequence)
        {
            var elevation = sequence[0];

            //Special case: all elements are equal, it is one long pattern
            if (sequence.All(number => number == elevation))
            {
                return sequence.Length - 1;
            }

            var patternLength = 0;

            for (int i = 1; i < sequence.Length; i++)
            {
                if (elevation != sequence[i])
                {
                    patternLength = i + 1;
                    break;
                }
            }

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
