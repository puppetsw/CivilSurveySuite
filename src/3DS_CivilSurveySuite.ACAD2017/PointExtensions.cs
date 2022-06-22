﻿// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Linq;
using _3DS_CivilSurveySuite.Shared.Models;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class PointExtensions
    {
        public static Point2d ToPoint2d(this Point point) => new Point2d(point.X, point.Y);

        public static Point2d ToPoint2d(this Point3d point) => new Point2d(point.X, point.Y);

        public static Point3d ToPoint3d(this Point point) => new Point3d(point.X, point.Y, point.Z);

        public static Point3d ToFlattenedPoint3d(this Point point) => new Point3d(point.X, point.Y, 0.0);

        public static Point3d ToPoint3d(this Point2d point, double elevation = 0) => new Point3d(point.X, point.Y, elevation);

        public static Point ToPoint(this Point2d point) => new Point(point.X, point.Y);

        public static Point ToPoint(this Point3d point) => new Point(point.X, point.Y, point.Z);

        public static List<Point2d> ToListOfPoint2d(this IEnumerable<Point> pointList)
        {
            return pointList.Select(point => new Point2d(point.X, point.Y)).ToList();
        }

        public static List<Point2d> ToListOfPoint2d(this IEnumerable<Point3d> points)
        {
            return points.Select(point => new Point2d(point.X, point.Y)).ToList();
        }

        public static List<Point3d> ToListOfPoint3d(this IEnumerable<Point> pointList)
        {
            return pointList.Select(point => new Point3d(point.X, point.Y, point.Z)).ToList();
        }

        public static List<Point3d> ToListOfPoint3d(this IEnumerable<Point2d> points, double elevation = 0)
        {
            return points.Select(point => new Point3d(point.X, point.Y, elevation)).ToList();
        }

        public static bool IsValid(this Point3d point)
        {
            if (Math.Abs(point.X) < 1E+20 && Math.Abs(point.Y) < 1E+20 && Math.Abs(point.Z) < 1E+20)
                return point.X + point.Y + point.Z != 0.0;
            return false;
        }

        public static Point3dCollection ToPoint3dCollection(this List<SurveyPoint> surveyPoints)
        {
            Point3dCollection points = new Point3dCollection();
            foreach (SurveyPoint surveyPoint in surveyPoints)
            {
                points.Add(new Point3d(surveyPoint.CivilPoint.Easting, surveyPoint.CivilPoint.Northing, surveyPoint.CivilPoint.Elevation));
            }

            return points;
        }
    }
}