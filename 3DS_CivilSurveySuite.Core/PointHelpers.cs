// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.Core
{
    public static class PointHelpers
    {
        /// <summary>
        /// Gets distance between two coordinates.
        /// </summary>
        /// <param name="point1">The first coordinate.</param>
        /// <param name="point2">The second coordinate.</param>
        /// <param name="decimalPlaces">The number of decimal places to round to.</param>
        /// <returns>A double representing the distance between the two coordinates.</returns>
        public static double GetDistanceBetweenPoints(Point point1, Point point2, int decimalPlaces = 4)
        {
            return MathHelpers.GetDistanceBetweenPoints(point1.X, point2.X, point1.Y, point2.Y, decimalPlaces);
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
        /// <param name="decimalPlaces"></param>
        /// <returns>A <see cref="Point"/> containing the coordinates generated from the <see cref="Angle"/>
        /// and distance.</returns>
        public static Point AngleAndDistanceToPoint(Angle angle, double distance, Point basePoint, int decimalPlaces = 4)
        {
            double dec = angle.ToDecimalDegrees(15);
            double rad = MathHelpers.DecimalDegreesToRadians(dec);

            double departure = distance * Math.Sin(rad);
            double latitude = distance * Math.Cos(rad);

            double newX = Math.Round(basePoint.X + departure, decimalPlaces);
            double newY = Math.Round(basePoint.Y + latitude, decimalPlaces);

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

            Angle internalA;
            if (angle1.Degrees > inverseAng.Degrees)
                internalA = angle1 - inverseAng;
            else
                internalA = inverseAng - angle1;

            Angle internalB; 
            if (angle2.Degrees > inverseAng.Flip().Degrees)
                internalB = angle2 - inverseAng.Flip();
            else
                internalB = inverseAng.Flip() - angle2;

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

            intersectionPoint = AngleAndDistanceToPoint(angle2, dist1, point2, 5);
            return true;
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
                // Borrowed from C3DTools.
                // TODO: Write own calculations.
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
            var point1A = AngleAndDistanceToPoint(angle1, 32000, point1, 15);

            solution1 = Point.Origin;
            solution2 = Point.Origin;

            double num1 = point1A.X - point1.X;
            double num2 = point1A.Y - point1.Y;
            double num3 = num1 * num1 + num2 * num2;
            double num4 = 2.0 * (num1 * (point1.X - point2.X) + num2 * (point1.Y - point2.Y));
            double num5 = (point1.X - point2.X) * (point1.X - point2.X) + (point1.Y - point2.Y) * (point1.Y - point2.Y) - radius * radius;
            double d = num4 * num4 - 4.0 * num3 * num5;

            if (num3 <= 1E-07 | d < 0.0)
                return false; // No intersection found.

            //if (d == 0.0) // Only one intersection? // Not needed?
            //{
            //    double num6 = -num4 / (2.0 * num3);
            //    solution1 = new Point(Math.Round(point1.X + num6 * num1, decimalPlaces), Math.Round(point1.Y + num6 * num2, decimalPlaces), 0.0);
            //    solution2 = Point.Origin;
            //    return true;
            //}

            double num7 = (-num4 + Math.Sqrt(d)) / (2.0 * num3);
            solution1 = new Point(Math.Round(point1.X + num7 * num1, decimalPlaces), Math.Round(point1.Y + num7 * num2, decimalPlaces), 0.0);

            double num8 = (-num4 - Math.Sqrt(d)) / (2.0 * num3);
            solution2 = new Point(Math.Round(point1.X + num8 * num1, decimalPlaces), Math.Round(point1.Y + num8 * num2, decimalPlaces), 0.0);

            return true;
        }
    }
}