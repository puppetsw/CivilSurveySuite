// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.Core
{
    /// <summary>
    /// A Math helper class for converting units, angles and coordinates.
    /// </summary>
    public static class MathHelpers
    {
        /// <summary>
        /// Converts links to meters.
        /// </summary>
        /// <param name="link">The links value</param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <returns>A double representing the links value in meters.</returns>
        public static double ConvertLinkToMeters(double link, int decimalPlaces = 4)
        {
            const double linkConversion = 0.201168;
            return Math.Round(link * linkConversion, decimalPlaces);
        }

        /// <summary>
        /// Converts feet and inches to meters.
        /// </summary>
        /// <param name="feetAndInches">
        /// Feet and inches represented as decimal. 5feet 2inch 5.02.
        /// Inches less than 10 must have a preceding 0. 
        /// </param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <returns>A double representing the feet and inches in meters.</returns>
        public static double ConvertFeetToMeters(double feetAndInches, int decimalPlaces = 4)
        {
            const double feetConversion = 0.3048;
            const double inchConversion = 0.0254;

            var feet = Math.Truncate(feetAndInches) * feetConversion;
            var inch1 = feetAndInches - Math.Truncate(feetAndInches);
            var inch2 = (inch1 * 100) * inchConversion;

            return Math.Round(feet + inch2, decimalPlaces);
        }

        /// <summary>
        /// Converts a <see cref="Angle"/> to decimal degrees.
        /// </summary>
        /// <param name="angle">The <see cref="Angle"/> to convert.</param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <returns>A double representing the <see cref="Angle"/> in decimal degrees.</returns>
        /// <remarks>The returned value is rounded to 4 decimal places, unless otherwise specified.</remarks>
        public static double AngleToDecimalDegrees(Angle angle, int decimalPlaces = 4)
        {
            if (angle == null)
                return 0;

            double minutes = (double) angle.Minutes / 60;
            double seconds = (double) angle.Seconds / 3600;

            double decimalDegree = angle.Degrees + minutes + seconds;

            return Math.Round(decimalDegree, decimalPlaces);
        }

        /// <summary>
        /// Converts a decimal degrees value to radians.
        /// </summary>
        /// <param name="decimalDegrees">The decimal degrees to convert.</param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <returns>A double value containing the decimal degrees in radians.</returns>
        public static double DecimalDegreesToRadians(double decimalDegrees, int decimalPlaces = 6)
        {
            return Math.Round(decimalDegrees * (Math.PI / 180), decimalPlaces);
        }

        /// <summary>
        /// Converts a decimal degrees value to <see cref="Angle"/> object.
        /// </summary>
        /// <param name="decimalDegrees"></param>
        /// <returns>A <see cref="Angle"/> representing the converted decimal degrees values.</returns>
        public static Angle DecimalDegreesToAngle(double decimalDegrees)
        {
            var degrees = Math.Floor(decimalDegrees);
            var minutes = Math.Floor((decimalDegrees - degrees) * 60);
            var seconds = Math.Round(((decimalDegrees - degrees) * 60 - minutes) * 60, 0);

            return new Angle { Degrees = (int) degrees, Minutes = (int) minutes, Seconds = (int) seconds };
        }

        /// <summary>
        /// Gets distance between two coordinates.
        /// </summary>
        /// <param name="x1">Easting of first coordinate.</param>
        /// <param name="x2">Easting of second coordinate.</param>
        /// <param name="y1">Northing of first coordinate.</param>
        /// <param name="y2">Northing of second coordinate.</param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <returns>A double representing the distance between the two coordinates.</returns>
        public static double DistanceBetweenPoints(double x1, double x2, double y1, double y2, int decimalPlaces = 4)
        {
            double x = Math.Abs(x1 - x2);
            double y = Math.Abs(y1 - y2);

            double distance = Math.Round(Math.Sqrt(x * x + y * y), decimalPlaces);

            return distance;
        }

        /// <summary>
        /// Gets distance between two coordinates.
        /// </summary>
        /// <param name="point1">The first coordinate.</param>
        /// <param name="point2">The second coordinate.</param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <returns>A double representing the distance between the two coordinates.</returns>
        public static double DistanceBetweenPoints(Point point1, Point point2, int decimalPlaces = 4)
        {
            return DistanceBetweenPoints(point1.X, point2.X, point1.Y, point2.Y, decimalPlaces);
        }

        /// <summary>
        /// Gets angle/bearing between two coordinates
        /// </summary>
        /// <param name="x1">Easting of first coordinate</param>
        /// <param name="x2">Easting of second coordinate</param>
        /// <param name="y1">Northing of first coordinate</param>
        /// <param name="y2">Northing of second coordinate</param>
        /// <returns>A <see cref="Angle"/> representing the angle/bearing between the two coordinates.</returns>
        public static Angle AngleBetweenPoints(double x1, double x2, double y1, double y2)
        {
            double rad = Math.Atan2(x2 - x1, y2 - y1);

            if (rad < 0)
                rad += 2 * Math.PI; // If radians is less than 0 add 2PI.

            double decDeg = Math.Abs(rad) * 180 / Math.PI;
            return DecimalDegreesToAngle(decDeg);
        }

        /// <summary>
        /// Gets angle/bearing between two coordinates
        /// </summary>
        /// <param name="point1">The first coordinate.</param>
        /// <param name="point2">The second coordinate.</param>
        /// <returns>A <see cref="Angle"/> representing the angle/bearing between the two coordinates.</returns>
        public static Angle AngleBetweenPoints(Point point1, Point point2)
        {
            return AngleBetweenPoints(point1.X, point2.X, point1.Y, point2.Y);
        }

        /// <summary>
        /// Converts a list of <see cref="Angle"/> objects into a list of <see cref="Point"/> objects.
        /// </summary>
        /// <param name="bearingList"></param>
        /// <param name="basePoint"></param>
        /// <returns>collection of <see cref="Point"/></returns>
        public static List<Point> BearingAndDistanceToCoordinates(IEnumerable<TraverseObject> bearingList, Point basePoint)
        {
            var pointList = new List<Point> { basePoint };

            var i = 0;
            foreach (TraverseObject item in bearingList)
            {
                double dec = AngleToDecimalDegrees(item.Angle);
                double rad = DecimalDegreesToRadians(dec);

                double departure = item.Distance * Math.Sin(rad);
                double latitude = item.Distance * Math.Cos(rad);

                double newX = Math.Round(pointList[i].X + departure, 4);
                double newY = Math.Round(pointList[i].Y + latitude, 4);

                pointList.Add(new Point(newX, newY));
                i++;
            }

            return pointList;
        }

        /// <summary>
        /// Converts a <see cref="IEnumerable{T}"/> of <see cref="TraverseAngleObject"/> to a List of <see cref="Point"/>.
        /// </summary>
        /// <param name="angleList">A enumerable list containing the <see cref="TraverseAngleObject"/>'s.</param>
        /// <param name="basePoint">The base point.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="Point"/>.</returns>
        public static List<Point> AngleAndDistanceToCoordinates(IEnumerable<TraverseAngleObject> angleList, Point basePoint)
        {
            var newPointList = new List<Point> { basePoint };
            var lastBearing = new Angle();
            var i = 0;
            foreach (TraverseAngleObject item in angleList)
            {
                Angle nextBearing = lastBearing;

                if (!item.Angle.IsZero)
                {
                    switch (item.ReferenceDirection)
                    {
                        case AngleReferenceDirection.Backward:
                            nextBearing = lastBearing - new Angle(180);
                            break;
                        case AngleReferenceDirection.Forward:
                            nextBearing = lastBearing;
                            break;
                    }

                    switch (item.RotationDirection)
                    {
                        case AngleRotationDirection.Negative:
                            nextBearing -= item.Angle;
                            break;
                        case AngleRotationDirection.Positive:
                            nextBearing += item.Angle;
                            break;
                    }
                }
                newPointList.Add(AngleAndDistanceToPoint(nextBearing, item.Distance, newPointList[i]));
                lastBearing = nextBearing;
                i++;
            }
            return newPointList;
        }

        public static Point AngleAndDistanceToPoint(Angle angle, double distance, Point basePoint)
        {
            double dec = AngleToDecimalDegrees(angle);
            double rad = DecimalDegreesToRadians(dec);

            double departure = distance * Math.Sin(rad);
            double latitude = distance * Math.Cos(rad);

            double newX = Math.Round(basePoint.X + departure, 4);
            double newY = Math.Round(basePoint.Y + latitude, 4);

            return new Point(newX, newY);
        }

        public static bool LineSegementsIntersect(Vector p, Vector p2, Vector q, Vector q2, out Point intersectingPoint, bool considerCollinearOverlapAsIntersect = false)
        {
            intersectingPoint = new Point();

            var r = p2 - p;
            var s = q2 - q;
            var rxs = r.Cross(s);
            var qpxr = (q - p).Cross(r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (Vector.IsZero(rxs) && Vector.IsZero(qpxr))
            {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                if (considerCollinearOverlapAsIntersect)
                    if ((0 <= (q - p)*r && (q - p)*r <= r*r) || (0 <= (p - q)*s && (p - q)*s <= s*s))
                        return true;

                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (Vector.IsZero(rxs) && !Vector.IsZero(qpxr))
                return false;

            // t = (q - p) x s / (r x s)
            var t = (q - p).Cross(s)/rxs;

            // u = (q - p) x r / (r x s)
            var u = (q - p).Cross(r)/rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!Vector.IsZero(rxs) && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // We can calculate the intersection point using either t or u.
                Vector intersection = p + t*r;
                intersectingPoint = new Point(intersection.X, intersection.Y);
                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }
    }
}