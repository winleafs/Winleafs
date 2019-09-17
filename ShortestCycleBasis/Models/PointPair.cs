namespace ShortestCycleBasis.Models
{
    public class PointPair<PointType>
    {
        public PointType Point1 { get; set; }
        public PointType Point2 { get; set; }

        public PointPair(PointType point1, PointType point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        /// <summary>
        /// Override Equals and GetHashCode for comparisons and usage in dictionaries
        /// Vertices are considered equal when their points are equal
        /// </summary>
        public override bool Equals(object obj)
        {
            var pointPair = (PointPair<PointType>)obj;
            return pointPair.Point1.Equals(Point1) && pointPair.Point2.Equals(Point2);
        }

        public override int GetHashCode()
        {
            int hashCode;

            hashCode = Point1 != null ? Point1.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (Point2 != null ? Point2.GetHashCode() : 0);

            return hashCode;
        }
    }
}
