// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Linq;
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
        public static double ToDecimalDegrees(this Angle angle, int decimalPlaces = 4)
        {
            if (angle == null)
                return 0;

            double minutes = (double) angle.Minutes / 60;
            double seconds = (double) angle.Seconds / 3600;

            double decimalDegree = angle.Degrees + minutes + seconds;

            return Math.Round(decimalDegree, decimalPlaces);
        }

        /// <summary>
        /// Converts a <see cref="Angle"/> to radians.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="decimalPlaces">The decimal places to round to. You should
        /// probably leave this as 15, otherwise rounding issues will occur.</param>
        /// <returns>A double representing the <see cref="Angle"/> in radians.</returns>
        public static double ToRadians(this Angle angle, int decimalPlaces = 15)
        {
            return DecimalDegreesToRadians(angle.ToDecimalDegrees());
        }

        /// <summary>
        /// Converts an <see cref="Angle"/> to a clockwise direction.
        /// </summary>
        /// <param name="angle">The angle in a counter-clockwise direction.</param>
        /// <returns>A <see cref="Angle"/> containing the converted values.</returns>
        public static Angle ToClockwise(this Angle angle)
        {
            return new Angle(360) - angle + new Angle(90);
        }

        /// <summary>
        /// Converts an <see cref="Angle"/> to a counter-clockwise direction.
        /// </summary>
        /// <param name="angle">The angle in a clockwise direction.</param>
        /// <returns>A <see cref="Angle"/> containing the converted values.</returns>
        public static Angle ToCounterClockwise(this Angle angle)
        {
            return new Angle(90) - angle + new Angle(360);
        }

        /// <summary>
        /// Flips an <see cref="Angle"/> 180°.
        /// </summary>
        /// <param name="angle">The angle to be flipped.</param>
        /// <returns>A <see cref="Angle"/> containing the flipped values.</returns>
        public static Angle Flip(this Angle angle)
        {
            return angle - new Angle(180);
        }

        /// <summary>
        /// Converts a decimal degrees value to radians.
        /// </summary>
        /// <param name="decimalDegrees">The decimal degrees to convert.</param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <returns>A double value containing the decimal degrees in radians.</returns>
        public static double DecimalDegreesToRadians(double decimalDegrees, int decimalPlaces = 15)
        {
            return (decimalDegrees / 180) * Math.PI;
        }

        /// <summary>
        /// Converts a radians value to decimal degrees.
        /// </summary>
        /// <param name="radians">The radians to convert.</param>
        /// <param name="decimalPlaces">The number of decimal places to round to. Default 6.</param>
        /// <returns>A double value containing the radians value in decimal degrees.</returns>
        public static double RadiansToDecimalDegrees(double radians, int decimalPlaces = 15)
        {
            return (radians * 180) / Math.PI;
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
        public static List<Point> TraverseObjectsToCoordinates(IEnumerable<TraverseObject> bearingList, Point basePoint)
        {
            var pointList = new List<Point> { basePoint };

            var i = 0;
            foreach (TraverseObject item in bearingList)
            {
                double dec = ToDecimalDegrees(item.Angle);
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
        public static List<Point> TraverseAngleObjectsToCoordinates(IEnumerable<TraverseAngleObject> angleList, Point basePoint)
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

        /// <summary>
        /// Converts a <see cref="Angle"/> object and distance to a <see cref="Point"/>.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="basePoint">The base point to calculate the new <see cref="Point"/> from.</param>
        /// <returns>A <see cref="Point"/> containing the coordinates generated from the <see cref="Angle"/>
        /// and distance.</returns>
        public static Point AngleAndDistanceToPoint(Angle angle, double distance, Point basePoint)
        {
            double dec = ToDecimalDegrees(angle);
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

        /// <summary>
        /// Calculates a area in square metres from the specified coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns>A <c>double</c> containing the square metre area.</returns>
        public static double Area(IReadOnlyList<Point> coordinates)
        {
            var array = coordinates.ToArray();

            double area = 0;
            var j = array.Length - 1;

            for (int i = 0; i < array.Length; i++)
            {
                area += (coordinates[j].X + coordinates[i].X) * (coordinates[j].Y - coordinates[i].Y);
                j = i;
            }

            return area / 2;
        }

        /// <summary>
        /// Converts a radians value to <see cref="Angle"/> object.
        /// </summary>
        /// <param name="radians">The radians.</param>
        /// <returns>A <see cref="Angle"/> representing the converted radians value.</returns>
        public static Angle RadiansToAngle(double radians)
        {
            var decimalDegrees = RadiansToDecimalDegrees(radians);
            return DecimalDegreesToAngle(decimalDegrees);
        }

        /// <summary>
        /// Does a floating point comparison.
        /// </summary>
        /// <param name="x">The first comparison number.</param>
        /// <param name="y">The second comparison number.</param>
        /// <returns><c>true</c> if the numbers are nearly equal, <c>false</c> otherwise.</returns>
        public static bool NearlyEqual(double x, double y) 
        {
            double epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * 1E-15;
            return Math.Abs(x - y) <= epsilon;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Point"/> is left or right of the line
        /// defined by the startPoint and endPoint parameters.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        /// <param name="pickedPoint">The picked point.</param>
        /// <returns>A int representing which side of the line the pickedPoint is. Which can be
        /// multiplied by a distance for example.
        /// <para>Returns <c>0</c> if the <c>pickedPoint</c> is on the line.</para>
        /// <para>Returns <c>-1</c> if the <c>pickedPoint</c> is to the right of the line.</para>
        /// <para>Returns <c>1</c> if the <c>pickedPoint</c> is to the left of the line.</para></returns>
        public static int IsLeft(Point startPoint, Point endPoint, Point pickedPoint)
        {
            double side = (endPoint.X - startPoint.X) * (pickedPoint.Y - startPoint.Y) - 
                         (pickedPoint.X - startPoint.X) * (endPoint.Y - startPoint.Y);

            if (Math.Abs(side) < 1.0e-8)
            {
                return 0; //pickedPoint is on the line
            }

            if (side > 0)
            {
                return 1; //pickedPoint is left of the line (CW)
            }

            return -1; //Is right.
        }

        /// <summary>
        /// Determines whether the specified <see cref="Angle"/> is an ordinary angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns><c>true</c> if the specified <see cref="Angle"/> is within the degree range
        /// of (360)0-180°; otherwise, <c>false</c>.</returns>
        public static bool IsOrdinaryAngle(Angle angle)
        {
            return angle.Degrees < 180 && angle.Degrees > 0;
        }

        /// <summary>
        /// Determines whether the specified angle determined by the <see cref="Point"/>s is an ordinary angle.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="endPoint">The end point.</param>
        /// <returns><c>true</c> if the specified angle is within the degree range
        /// of (360)0-180°; otherwise, <c>false</c>.</returns>
        public static bool IsOrdinaryAngle(Point startPoint, Point endPoint)
        {
            return startPoint.X < endPoint.X;
        }
    }
}