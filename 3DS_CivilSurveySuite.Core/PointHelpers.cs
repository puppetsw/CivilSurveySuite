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
        public static double DistanceBetweenPoints(Point point1, Point point2, int decimalPlaces = 4)
        {
            return MathHelpers.DistanceBetweenPoints(point1.X, point2.X, point1.Y, point2.Y, decimalPlaces);
        }
        
        /// <summary>
        /// Gets the mid-point between two <see cref="Point"/>s.
        /// </summary>
        /// <param name="point1">First point.</param>
        /// <param name="point2">Second point.</param>
        /// <returns>A <see cref="Point"/> representing the mid-point between the two <see cref="Point"/>s.</returns>
        public static Point MidpointBetweenPoints(Point point1, Point point2)
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
        /// <returns>A <see cref="Point"/> containing the coordinates generated from the <see cref="Angle"/>
        /// and distance.</returns>
        public static Point AngleAndDistanceToPoint(Angle angle, double distance, Point basePoint)
        {
            double dec = angle.ToDecimalDegrees();
            double rad = MathHelpers.DecimalDegreesToRadians(dec);

            double departure = distance * Math.Sin(rad);
            double latitude = distance * Math.Cos(rad);

            double newX = Math.Round(basePoint.X + departure, 4);
            double newY = Math.Round(basePoint.Y + latitude, 4);

            return new Point(newX, newY);
        }
    }
}