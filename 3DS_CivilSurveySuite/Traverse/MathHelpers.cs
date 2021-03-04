using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

namespace _3DS_CivilSurveySuite.Traverse
{
    public class MathHelpers
    {
        /// <summary>
        /// Converts link to meters
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public static double ConvertLinkToMeters(double link)
        {
            const double linkConversion = 0.201168;

            return Math.Round(link * linkConversion, 4);
        }

        /// <summary>
        /// Converts feet and inches to meters
        /// </summary>
        /// <param name="feetAndInches">
        /// Feet and inches represented as decimal. 5feet 2inch 5.02.
        /// Inches less than 10 must have a preceeding 0. 
        /// </param>
        /// <returns></returns>
        public static double ConvertFeetToMeters(double feetAndInches)
        {
            const double feetConversion = 0.3048;
            const double inchConversion = 0.0254;

            var feet = Math.Truncate(feetAndInches) * feetConversion;
            var inch1 = feetAndInches - Math.Truncate(feetAndInches);
            var inch2 = (inch1 * 100) * inchConversion;

            return Math.Round(feet + inch2, 4);
        }

        /// <summary>
        /// Converts <see cref="DMS"/> object to decimal degrees
        /// </summary>
        /// <param name="dMS"></param>
        /// <returns></returns>
        public static double DMSToDecimalDegrees(DMS dMS)
        {
            if (dMS == null)
                return 0;

            double minutes = (double)dMS.Minutes / 60;
            double seconds = (double)dMS.Seconds / 3600;

            double decimalDegree = dMS.Degrees + minutes + seconds;

            return decimalDegree;
        }

        /// <summary>
        /// Converts decimal degrees to radians
        /// </summary>
        /// <param name="decimalDegrees"></param>
        /// <returns></returns>
        public static double DecimalDegreesToRadians(double decimalDegrees)
        {
            return decimalDegrees * (Math.PI / 180);
        }

        /// <summary>
        /// Converts decimal degrees to <see cref="DMS"/> object
        /// </summary>
        /// <param name="decimalDegrees"></param>
        /// <returns></returns>
        public static DMS DecimalDegreesToDMS(double decimalDegrees)
        {
            var degrees = Math.Floor(decimalDegrees);
            var minutes = Math.Floor((decimalDegrees - degrees) * 60);
            var seconds = Math.Round((((decimalDegrees - degrees) * 60) - minutes) * 60, 0);

            return new DMS() { Degrees = (int)degrees, Minutes = (int)minutes, Seconds = (int)seconds };
        }

        /// <summary>
        /// Gets distance between two coordinates
        /// </summary>
        /// <param name="x1">Easting of first coordinate</param>
        /// <param name="x2">Easting of second coordinate</param>
        /// <param name="y1">Northing of first coordinate</param>
        /// <param name="y2">Northing of second coordinate</param>
        /// <returns>double</returns>
        public static double DistanceBetweenPoints(double x1, double x2, double y1, double y2)
        {
            double x = Math.Abs(x1 - x2);
            double y = Math.Abs(y1 - y2);

            double distance = Math.Round(Math.Sqrt((x * x) + (y * y)), 4);

            return distance;
        }

        /// <summary>
        /// Gets angle/bearing between two coordinates
        /// </summary>
        /// <param name="x1">Easting of first coordinate</param>
        /// <param name="x2">Easting of second coordinate</param>
        /// <param name="y1">Northing of first coordinate</param>
        /// <param name="y2">Northing of second coordinate</param>
        /// <returns><see cref="DMS"/></returns>
        public static DMS AngleBetweenPoints(double x1, double x2, double y1, double y2)
        {
            double rad = Math.Atan2(x2 - x1, y2 - y1);

            if (rad < 0)
                rad += 2 * Math.PI; // if radians is less than 0 add 2PI

            double decDeg = Math.Abs(rad) * 180 / Math.PI;
            DMS dms = DecimalDegreesToDMS(decDeg);

            return dms;
        }

        /// <summary>
        /// Converts a list of <see cref="DMS"/> objects into a list of <see cref="Point2d"/> objects
        /// </summary>
        /// <param name="bearingList"></param>
        /// <returns>collection of <see cref="Point2d"/></returns>
        public static List<Point2d> BearingAndDistanceToCoordinates(IList<TraverseItem> bearingList, Point2d basePoint)
        {
            var pointList = new List<Point2d>();
            //add basePoint
            pointList.Add(basePoint);

            int i = 0;
            foreach (TraverseItem item in bearingList)
            {
                var dec = DMSToDecimalDegrees(item.DMSBearing);
                var rad = DecimalDegreesToRadians(dec);

                double depature = item.Distance * Math.Sin(rad);
                double latitude = item.Distance * Math.Cos(rad);

                double newX = Math.Round(pointList[i].X + depature, 4);
                double newY = Math.Round(pointList[i].Y + latitude, 4);

                pointList.Add(new Point2d(newX, newY));
                i++;
            }

            return pointList;
        }

    }
}
