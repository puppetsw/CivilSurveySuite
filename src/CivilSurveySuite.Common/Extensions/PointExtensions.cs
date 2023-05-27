using CivilSurveySuite.Shared.Models;

namespace CivilSurveySuite.Shared.Extensions
{
    public static class PointExtensions
    {
        public static Point ToPoint(this CivilPoint point)
        {
            return new Point(point.Easting, point.Northing, point.Elevation);
        }
    }
}
