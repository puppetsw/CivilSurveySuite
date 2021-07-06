// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite_ACADBase21
{
    public class Polylines
    {
        public static Polyline Square(Point2d basePoint, double squareSize, int lineWidth = 0)
        {
            var pointTopLeft = new Point2d(basePoint.X - squareSize, basePoint.Y + squareSize);
            var pointTopRight = new Point2d(basePoint.X + squareSize, basePoint.Y + squareSize);
            var pointBottomRight = new Point2d(basePoint.X + squareSize, basePoint.Y - squareSize);
            var pointBottomLeft = new Point2d(basePoint.X - squareSize, basePoint.Y - squareSize);

            var pLine = new Polyline();
            pLine.AddVertexAt(0, pointTopLeft, 0, lineWidth, lineWidth);
            pLine.AddVertexAt(1, pointTopRight, 0, lineWidth, lineWidth);
            pLine.AddVertexAt(2, pointBottomRight, 0, lineWidth, lineWidth);
            pLine.AddVertexAt(3, pointBottomLeft, 0, lineWidth, lineWidth);
            pLine.AddVertexAt(4, pointTopLeft, 0, lineWidth, lineWidth);
            pLine.AddVertexAt(5, pointBottomRight, 0, lineWidth, lineWidth);
            pLine.AddVertexAt(6, pointBottomLeft, 0, lineWidth, lineWidth);
            pLine.AddVertexAt(7, pointTopRight, 0, lineWidth, lineWidth);

            return pLine;
        }

        /// <summary>
        /// Gets the angle of the segment closest to the picked point from a polyline
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="pickedPoint"></param>
        /// <returns>A double representing the angle of the polyline segment.</returns>
        //BUG: When polyline selected is the first segement, the angle is incorrect.
        public static double GetPolylineSegmentAngle(Polyline polyline, Point3d pickedPoint)
        {
            var segmentStart = 0;

            Point3d closestPoint = polyline.GetClosestPointTo(pickedPoint, false);
            double len = polyline.GetDistAtPoint(closestPoint);

            for (var i = 1; i < polyline.NumberOfVertices - 1; i++)
            {
                Point3d pt1 = polyline.GetPoint3dAt(i);
                double l1 = polyline.GetDistAtPoint(pt1);

                Point3d pt2 = polyline.GetPoint3dAt(i + 1);
                double l2 = polyline.GetDistAtPoint(pt2);

                if (len > l1 && len < l2)
                {
                    segmentStart = i;
                    break;
                }
            }

            LineSegment2d segment = polyline.GetLineSegment2dAt(segmentStart);
            return segment.Direction.Angle;
        }

        /// <summary>
        /// Gets the angle of the segment closest to the picked point from a 3d polyline
        /// </summary>
        /// <param name="polyline3d"></param>
        /// <param name="pickedPoint"></param>
        /// <returns>angle of line segment as double</returns>
        public static double GetPolyline3dSegmentAngle(Polyline3d polyline3d, Point3d pickedPoint)
        {
            // Take the 3d Polyline and convert it to 2d.
            var polyline = new Polyline();
            for (int j = 0; j < polyline3d.EndParam; j++)
            {
                Point3d point = polyline3d.GetPointAtParameter(j);
                polyline.AddVertexAt(j, new Point2d(point.X, point.Y), 0, 0, 0);
            }

            return GetPolylineSegmentAngle(polyline, pickedPoint);
        }


    }
}