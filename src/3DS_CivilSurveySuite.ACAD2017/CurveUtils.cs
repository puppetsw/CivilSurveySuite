// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using _3DS_CivilSurveySuite.Shared.Helpers;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class CurveUtils
    {
        private const double TOLERANCE = 1E-08;

        [Obsolete("Method is obsolete. Use CircularArc3dExtensions.GetArcBulge")]
        public static double CalculateBulge(Point3d startPt, Point3d midPt, Point3d endPt, double tolerance) =>
            CalculateBulge(new Point2d(startPt.X, startPt.Y), new Point2d(midPt.X, midPt.Y), new Point2d(endPt.X, endPt.Y), tolerance);

        [Obsolete("Method is obsolete. Use CircularArc3dExtensions.GetArcBulge")]
        public static double CalculateBulge(Point2d startPt, Point2d midPt, Point2d endPt, double tolerance)
        {
            if (tolerance < TOLERANCE)
            {
                tolerance = TOLERANCE;
            }

            double bulge = 0.0;
            LineSegment2d lineSegment2d = new LineSegment2d(startPt, endPt);

            if (lineSegment2d.Length < 0.0)
            {
                return bulge;
            }

            Point2d midPoint = lineSegment2d.MidPoint;
            double distanceTo = midPt.GetDistanceTo(midPoint);

            if (distanceTo > tolerance)
            {
                bulge = 2.0 * distanceTo / lineSegment2d.Length * PointHelpers.DeflectionDirection(startPt.ToPoint(), endPt.ToPoint(), midPt.ToPoint());
            }

            return bulge;
        }
    }
}