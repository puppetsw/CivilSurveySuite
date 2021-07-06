// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.
// 
// Filename: MathHelpers.cs
// Date:     01/07/2021
// Author:   scott

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.Helpers
{
    public static class MathHelpers
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
        /// Inches less than 10 must have a preceding 0. 
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
        /// Converts <see cref="Angle"/> object to decimal degrees
        /// </summary>
        /// <param name="dms"></param>
        /// <returns></returns>
        public static double AngleToDecimalDegrees(Angle dms)
        {
            if (dms == null)
                return 0;

            double minutes = (double) dms.Minutes / 60;
            double seconds = (double) dms.Seconds / 3600;

            double decimalDegree = dms.Degrees + minutes + seconds;

            return decimalDegree;
        }

        /// <summary>
        /// Converts decimal degrees to radians
        /// </summary>
        /// <param name="decimalDegrees"></param>
        /// <returns>A double value containing the decimal degrees in radians.</returns>
        public static double DecimalDegreesToRadians(double decimalDegrees)
        {
            return decimalDegrees * (Math.PI / 180);
        }

        /// <summary>
        /// Converts decimal degrees to <see cref="Angle"/> object
        /// </summary>
        /// <param name="decimalDegrees"></param>
        /// <returns></returns>
        public static Angle DecimalDegreesToDMS(double decimalDegrees)
        {
            var degrees = Math.Floor(decimalDegrees);
            var minutes = Math.Floor((decimalDegrees - degrees) * 60);
            var seconds = Math.Round((((decimalDegrees - degrees) * 60) - minutes) * 60, 0);

            return new Angle() { Degrees = (int) degrees, Minutes = (int) minutes, Seconds = (int) seconds };
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
        /// <returns><see cref="Angle"/></returns>
        public static Angle AngleBetweenPoints(double x1, double x2, double y1, double y2)
        {
            double rad = Math.Atan2(x2 - x1, y2 - y1);

            if (rad < 0)
                rad += 2 * Math.PI; // if radians is less than 0 add 2PI

            double decDeg = Math.Abs(rad) * 180 / Math.PI;
            Angle dms = DecimalDegreesToDMS(decDeg);

            return dms;
        }

        /// <summary>
        /// Converts a list of <see cref="Angle"/> objects into a list of <see cref="Point2d"/> objects.
        /// </summary>
        /// <param name="bearingList"></param>
        /// <param name="basePoint"></param>
        /// <returns>collection of <see cref="Point2d"/></returns>
        public static List<Point2d> BearingAndDistanceToCoordinates(IEnumerable<TraverseObject> bearingList, Point2d basePoint)
        {
            var pointList = new List<Point2d>();
            //add basePoint
            pointList.Add(basePoint);

            int i = 0;
            foreach (TraverseObject item in bearingList)
            {
                var dec = AngleToDecimalDegrees(item.DMSBearing);
                var rad = DecimalDegreesToRadians(dec);

                double departure = item.Distance * Math.Sin(rad);
                double latitude = item.Distance * Math.Cos(rad);

                double newX = Math.Round(pointList[i].X + departure, 4);
                double newY = Math.Round(pointList[i].Y + latitude, 4);

                pointList.Add(new Point2d(newX, newY));
                i++;
            }

            return pointList;
        }

        /// <summary>
        /// Converts a <see cref="IEnumerable{T}"/> of <see cref="TraverseAngleObject"/> to a List of <see cref="Point2d"/>.
        /// </summary>
        /// <param name="angleList">A enumerable list containing the <see cref="TraverseAngleObject"/>'s.</param>
        /// <param name="basePoint">The base point.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="Point2d"/>.</returns>
        public static List<Point2d> AngleAndDistanceToCoordinates(IEnumerable<TraverseAngleObject> angleList, Point2d basePoint)
        {
            var newPointList = new List<Point2d> { basePoint };

            var lastBearing = new Angle(0);
            var i = 0;
            foreach (TraverseAngleObject item in angleList)
            {
                Angle nextBearing = lastBearing - new Angle(180);

                if (i == 0)
                {
                    nextBearing = new Angle(0);
                }
                else if (!item.DMSInternalAngle.IsEmpty)
                {
                    nextBearing -= item.DMSInternalAngle;
                }
                else if (!item.DMSAdjacentAngle.IsEmpty)
                {
                    nextBearing += item.DMSAdjacentAngle;
                }

                double dec = AngleToDecimalDegrees(nextBearing);
                double rad = DecimalDegreesToRadians(dec);

                double departure = item.Distance * Math.Sin(rad);
                double latitude = item.Distance * Math.Cos(rad);

                double newX = Math.Round(newPointList[i].X + departure, 4);
                double newY = Math.Round(newPointList[i].Y + latitude, 4);

                newPointList.Add(new Point2d(newX, newY));

                lastBearing = nextBearing;
                i++;
            }
            return newPointList;
        }
    }
}