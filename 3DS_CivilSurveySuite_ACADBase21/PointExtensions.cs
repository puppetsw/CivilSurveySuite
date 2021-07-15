// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using System.Linq;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite_ACADBase21
{
    public static class PointExtensions
    {
        public static Point2d ToPoint2d(this Point point)
        {
            return new Point2d(point.X, point.Y);
        }

        public static Point3d ToPoint3d(this Point point)
        {
            return new Point3d(point.X, point.Y, point.Z);
        }

        public static Point ToPoint(this Point2d point)
        {
            return new Point(point.X, point.Y);
        }

        public static Point ToPoint(this Point3d point)
        {
            return new Point(point.X, point.Y, point.Z);
        }

        public static List<Point2d> ToListOfPoint2d(this IEnumerable<Point> pointList)
        {
            return pointList.Select(point => new Point2d(point.X, point.Y)).ToList();
        }

        public static List<Point3d> ToListOfPoint3d(this IEnumerable<Point> pointList)
        {
            return pointList.Select(point => new Point3d(point.X, point.Y, point.Z)).ToList();
        }

    }
}