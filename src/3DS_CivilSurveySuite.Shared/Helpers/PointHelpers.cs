// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Shared.Models;

namespace _3DS_CivilSurveySuite.Shared.Helpers
{
    public static class PointHelpers
    {
        /// <summary>
        /// Gets distance between two coordinates.
        /// </summary>
        /// <param name="point1">The first coordinate.</param>
        /// <param name="point2">The second coordinate.</param>
        /// <param name="useRounding"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns>A double representing the distance between the two coordinates.</returns>
        public static double GetDistanceBetweenPoints(Point point1, Point point2, bool useRounding = false, int decimalPlaces = 4)
        {
            return MathHelpers.GetDistanceBetweenPoints(point1.X, point2.X, point1.Y, point2.Y, useRounding, decimalPlaces);
        }

        /// <summary>
        /// Gets the minimum and maximum <see cref="Point"/>.
        /// </summary>
        /// <param name="points"><see cref="IReadOnlyList{T}"/> of <see cref="Point"/></param>
        /// <returns>MinMaxPoint.</returns>
        public static Bounds GetBounds(IReadOnlyList<Point> points)
        {
            var minPoint = points[0];
            var maxPoint = points[0];

            foreach (var point in points)
            {
                minPoint = new Point(Math.Min(minPoint.X, point.X), Math.Min(minPoint.Y, point.Y));
                maxPoint = new Point(Math.Max(maxPoint.X, point.X), Math.Max(maxPoint.Y, point.Y));
            }

            return new Bounds { MinPoint = minPoint, MaxPoint = maxPoint };
        }

        /// <summary>
        /// Gets the mid-point between two <see cref="Point"/>s.
        /// </summary>
        /// <param name="point1">First point.</param>
        /// <param name="point2">Second point.</param>
        /// <returns>A <see cref="Point"/> representing the mid-point between the two <see cref="Point"/>s.</returns>
        public static Point GetMidpointBetweenPoints(Point point1, Point point2)
        {
            double x = (point1.X + point2.X) / 2;
            double y = (point1.Y + point2.Y) / 2;
            return new Point(x, y);
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
                double dec = item.Angle.ToDecimalDegrees();
                double rad = MathHelpers.DecimalDegreesToRadians(dec);

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
        public static List<Point> TraverseObjectsToCoordinates(IEnumerable<TraverseAngleObject> angleList, Point basePoint)
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
            double dec = angle.ToDecimalDegrees();
            double rad = MathHelpers.DecimalDegreesToRadians(dec); //Millionths

            double departure = Math.Round(Math.Sin(rad) * distance, 10);
            double latitude = Math.Round(Math.Cos(rad) * distance, 10);

            double newX = basePoint.X + departure;
            double newY = basePoint.Y + latitude;

            return new Point(newX, newY);
        }

        /// <summary>
        /// Converts a <see cref="Point"/> to <see cref="Vector"/>
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>A <see cref="Vector"/> representing the <see cref="Point"/>.</returns>
        public static Vector ToVector(this Point point)
        {
            return new Vector(point.X, point.Y);
        }

        /// <summary>
        /// Calculates a <see cref="Point"/> at the intersection of two <see cref="Angle"/> objects.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="angle1">The angle1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="angle2">The angle2.</param>
        /// <param name="intersectionPoint"></param>
        /// <returns>A <see cref="Point"/> representing the intersection of two <see cref="Angle"/> objects.</returns>
        /// <remarks>
        /// Seems to be a rounding issue that I've yet to fix.
        /// Might be an issue with AutoCAD internal. Math confirms with
        /// other online calculations.
        /// </remarks>
        public static bool AngleAngleIntersection(Point point1, Angle angle1, Point point2, Angle angle2, out Point intersectionPoint)
        {
            intersectionPoint = Point.Origin;
            var inverseAng = AngleHelpers.GetAngleBetweenPoints(point1, point2);
            var inverseDist = GetDistanceBetweenPoints(point1, point2);

            Angle internalA = angle1 - inverseAng;

            if (internalA.Degrees > 180 && internalA.Minutes >= 0 && internalA.Seconds >= 0)
                internalA = new Angle(360) - internalA;

            Angle internalB = angle2 - inverseAng.Flip();

            if (internalB.Degrees > 180 && internalB.Minutes >= 0 && internalB.Seconds >= 0)
                internalB = new Angle(360) - internalB;

            // Calculate remaining internal angle.
            Angle internalC = new Angle(180) - internalA - internalB;

            // If the internal angle is greater than or equal to 180°
            // Just thought that if the minutes or seconds are greater
            // It's still going to think it's okay.
            if (internalC.Degrees >= 180 && internalC.Minutes >= 0 && internalC.Seconds >= 0)
                return false;

            var radA = internalA.ToRadians();
            var radB = internalC.ToRadians();

            var dist1 = Math.Sin(radA) * inverseDist / Math.Sin(radB);

            intersectionPoint = AngleAndDistanceToPoint(angle2, dist1, point2);
            return true;
        }

        /// <summary>
        /// Calculates a point perpendicular to a line.
        /// </summary>
        /// <param name="firstPoint">The first point.</param>
        /// <param name="secondPoint">The second point.</param>
        /// <param name="pickedPoint">The picked point.</param>
        /// <param name="intersectionPoint">The intersection point.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool PerpendicularIntersection(Point firstPoint, Point secondPoint, Point pickedPoint, out Point intersectionPoint)
        {
            var k = ((secondPoint.Y - firstPoint.Y) * (pickedPoint.X - firstPoint.X) - (secondPoint.X - firstPoint.X) * (pickedPoint.Y - firstPoint.Y)) / (Math.Pow(secondPoint.Y - firstPoint.Y, 2) + Math.Pow(secondPoint.X - firstPoint.X, 2));
            var x = pickedPoint.X - k * (secondPoint.Y - firstPoint.Y);
            var y = pickedPoint.Y + k * (secondPoint.X - firstPoint.X);

            intersectionPoint = new Point(x, y);
            return intersectionPoint.IsValid();
        }

        /// <summary>
        /// Calculates two possible <see cref="Point"/> objects from a distance-distance intersection.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="dist1">The dist1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="dist2">The dist2.</param>
        /// <param name="solution1">The solution1.</param>
        /// <param name="solution2">The solution2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool DistanceDistanceIntersection(Point point1, double dist1, Point point2, double dist2, out Point solution1, out Point solution2)
        {
            solution1 = Point.Origin;
            solution2 = Point.Origin;

            double distBetweenPoints = GetDistanceBetweenPoints(point1, point2);
            if (distBetweenPoints <= dist1 + dist2 && distBetweenPoints >= Math.Abs(dist1 - dist2))
            {
                double xDifference = point2.X - point1.X;
                double yDifference = point2.Y - point1.Y;
                double num1 = (dist1 * dist1 - dist2 * dist2 + distBetweenPoints * distBetweenPoints) / (2.0 * distBetweenPoints);
                double num2 = point1.X + xDifference * num1 / distBetweenPoints;
                double num3 = point1.Y + yDifference * num1 / distBetweenPoints;
                double num4 = Math.Sqrt(dist1 * dist1 - num1 * num1);
                double num5 = -yDifference * (num4 / distBetweenPoints);
                double num6 = xDifference * (num4 / distBetweenPoints);

                solution1 = new Point(num2 + num5, num3 + num6, 0.0);
                solution2 = new Point(num2 - num5, num3 - num6, 0.0);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates a point at the intersection of a bearing from one point and distance from a second.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="angle1">The angle1.</param>
        /// <param name="point2">The point2.</param>
        /// <param name="radius">The dist.</param>
        /// <param name="solution1"></param>
        /// <param name="solution2"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool AngleDistanceIntersection(Point point1, Angle angle1, Point point2, double radius, out Point solution1, out Point solution2, int decimalPlaces = 5)
        {
            var point1A = AngleAndDistanceToPoint(angle1, 32000, point1);

            solution1 = Point.Origin;
            solution2 = Point.Origin;

            double num1 = point1A.X - point1.X;
            double num2 = point1A.Y - point1.Y;
            double num3 = num1 * num1 + num2 * num2;
            double num4 = 2.0 * (num1 * (point1.X - point2.X) + num2 * (point1.Y - point2.Y));
            double num5 = (point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y) - radius * radius;
            double d = num4 * num4 - 4.0 * num3 * num5;

            if (num3 <= 1E-07 || d < 0.0)
                return false; // No intersection found.

            double num7 = (-num4 + Math.Sqrt(d)) / (2.0 * num3);
            solution1 = new Point(Math.Round(point1.X + num7 * num1, decimalPlaces), Math.Round(point1.Y + num7 * num2, decimalPlaces), 0.0);

            double num8 = (-num4 - Math.Sqrt(d)) / (2.0 * num3);
            solution2 = new Point(Math.Round(point1.X + num8 * num1, decimalPlaces), Math.Round(point1.Y + num8 * num2, decimalPlaces), 0.0);

            return true;
        }

        /// <summary>
        /// Calculates a point at the intersection of four <see cref="Point"/> objects.
        /// </summary>
        /// <param name="a">First point.</param>
        /// <param name="b">Second point.</param>
        /// <param name="c">Third point.</param>
        /// <param name="d">Fourth point.</param>
        /// <param name="intersectionPoint">The resulting intersection point.</param>
        /// <param name="decimalPlaces">Number of decimal places to round to. Set to 8 by default.</param>
        /// <returns><c>true</c> if can calculate intersection, <c>false</c> otherwise.</returns>
        public static bool FourPointIntersection(Point a, Point b, Point c, Point d, out Point intersectionPoint, int decimalPlaces = 8)
        {
            intersectionPoint = Point.Origin;

            // Line AB represented as a1x + b1y = c1
            double a1 = b.Y - a.Y;
            double b1 = a.X - b.X;
            double c1 = a1 * a.X + b1 * a.Y;

            // Line CD represented as a2x + b2y = c2
            double a2 = d.Y - c.Y;
            double b2 = c.X - d.X;
            double c2 = a2 * c.X + b2 * c.Y;

            double determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {
                intersectionPoint = Point.Origin;
                return false;
            }

            double x = Math.Round((b2 * c1 - b1 * c2) / determinant, decimalPlaces);
            double y = Math.Round((a1 * c2 - a2 * c1) / determinant, decimalPlaces);
            intersectionPoint = new Point(x, y);
            return true;
        }

        /// <summary>
        /// Calculates a return leg based on two <see cref="Point"/>s.
        /// </summary>
        /// <param name="point1">The base point.</param>
        /// <param name="point2">The second point for bearing.</param>
        /// <param name="leftLeg">If true, calculated point is left of line, else right of line.</param>
        /// <param name="distance">Leg distance.</param>
        /// <returns>A <see cref="Point"/> representing the new points location.</returns>
        public static Point CalculateRightAngleTurn(Point point1, Point point2, bool leftLeg = true, double distance = 2.0)
        {
            var forwardAngle = AngleHelpers.GetAngleBetweenPoints(point1, point2);

            if (leftLeg)
            {
                forwardAngle -= 90;
            }
            else
            {
                forwardAngle += 90;
            }

            var newPoint = AngleAndDistanceToPoint(forwardAngle, distance, point1);
            return newPoint;
        }

        /// <summary>
        /// Calculates the third point in a rectangle given 3 existing points.
        /// </summary>
        /// <param name="point1">The base point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="point3">The third point.</param>
        /// <remarks>Elevation is calculated as an average of the 3 points.</remarks>
        /// <returns>A <see cref="Point"/> representing the third point of a rectangle.</returns>
        public static Point CalculateRectanglePoint(Point point1, Point point2, Point point3)
        {
            var distance = GetDistanceBetweenPoints(point2, point3);
            var angle = AngleHelpers.GetAngleBetweenPoints(point2, point3);

            var newPoint = AngleAndDistanceToPoint(angle, distance, point1);
            return newPoint;
        }
    }
}
