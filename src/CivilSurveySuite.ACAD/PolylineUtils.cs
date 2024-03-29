﻿using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using CivilSurveySuite.Common.Helpers;
using CivilSurveySuite.Common.Models;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace CivilSurveySuite.ACAD
{
    public static class PolylineUtils
    {
        /// <summary>
        /// Creates a point at the midpoint between two selected <see cref="Polyline"/> entities.
        /// </summary>
        /// <param name="createAction">The create point action.</param>
        /// <remarks>This can be useful for using the BestFit Alignment tool in Civil 3D when the
        /// surveyor only provided edge of pavement shots. At the command prompt for selecting
        /// the polylines you may enter T to bring up the Settings dialog.</remarks>
        public static void MidPointBetweenPolylines(Action<Transaction, Point3d> createAction)
        {
            using (var graphics = new TransientGraphics())
            using (var tr = AcadApp.StartTransaction())
            {
                if (!EditorUtils.TryGetEntityOfType<Curve>("\nSelect first Polyline: ",
                        "\nSelect a Polyline: ", out var firstObjectId))
                {
                    tr.Commit();
                    return;
                }

                var curve1 = (Curve)tr.GetObject(firstObjectId, OpenMode.ForRead);
                graphics.DrawLine(curve1, TransientDrawingMode.Highlight);

                if (!EditorUtils.TryGetEntityOfType<Curve>("\nSelect second Polyline: ",
                        "\nSelect a Polyline: ", out var secondObjectId))
                {
                    tr.Commit();
                    return;
                }

                var curve2 = (Curve)tr.GetObject(secondObjectId, OpenMode.ForRead);
                graphics.DrawLine(curve2, TransientDrawingMode.Highlight);

                do
                {
                    if (!EditorUtils.TryGetPoint("\nPick a point: ", out Point3d pickedPoint))
                        break;

                    var p1 = curve1.GetClosestPointTo(pickedPoint, true);
                    var p2 = curve2.GetClosestPointTo(pickedPoint, true);

                    graphics.DrawLine(p1, p2);

                    var calcMidPoint = PointHelpers.GetMidpointBetweenPoints(p1.ToPoint(), p2.ToPoint()).ToPoint3d();

                    graphics.DrawDot(calcMidPoint, Settings.GraphicsSize);

                    createAction(tr, calcMidPoint);

                } while (true);

                tr.Commit();
            }
        }

        /// <summary>
        /// Converts a <see cref="Polyline"/> to <see cref="Polyline3d"/>
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="sourcePoints"></param>
        /// <param name="tr"></param>
        /// <param name="polyline3d"></param>
        /// <param name="midOrdinate"></param>
        /// <returns></returns>
        public static bool ConvertToPolyline3d(this Polyline polyline, Point3dCollection sourcePoints,
            Transaction tr, out Polyline3d polyline3d, double midOrdinate = 0.01)
        {
            if (midOrdinate <= 0)
            {
                midOrdinate = 0.01;
            }

            Point3dCollection points = new Point3dCollection();

            for (int i = 0; i < polyline.EndParam; i++)
            {
                Point3d point1 = sourcePoints[i];
                Point3d point2 = Point3d.Origin;

                if (i != (int)polyline.EndParam)
                {
                    point2 = sourcePoints[i + 1];
                }

                points.Add(point1);

                var radiusPoint = polyline.SegmentRadiusPoint(i);
                if (!radiusPoint.IsArc())
                {
                    if (point2 != Point3d.Origin)
                    {
                        points.Add(point2);
                    }

                    continue;
                }

                var arc = new CircularArc2d(polyline.GetPointAtParameter(i).ToPoint2d(), radiusPoint.Radius);

                double stepDistance = CircularArcExtensions.ArcLengthByMidOrdinate(Math.Abs(radiusPoint.Radius), midOrdinate);
                double distanceAtParameter1 = polyline.GetDistanceAtParameter(i);
                double distanceAtParameter2 = polyline.GetDistanceAtParameter(i + 1);
                while ((distanceAtParameter1 += stepDistance) < distanceAtParameter2)
                {
                    Point3d pointAtDist = polyline.GetPointAtDist(distanceAtParameter1);

                    //firstPoint.Z + elevationDifference * (distance / distanceBetweenPoints)
                    var height = point1.Z + (point1.Z - point2.Z) * (distanceAtParameter1 / arc.GetLength(0, 1));
                    points.Add(new Point3d(pointAtDist.X, pointAtDist.Y, height));
                }
            }

            polyline3d = new Polyline3d(Poly3dType.SimplePoly, points, false);
            polyline3d.Layer = polyline.Layer;
            return true;
        }

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
        //FIXED: Make option to return readable angle (page-up like in Civil 3D). //Not an option.
        //FIXED: When polyline selected is the first segment, the angle is incorrect.
        //FIXED: Debug this and find out what's happening at start/end of polylines.
        //FIXED: There is something weird happening with the distances to the polyline segments.
        public static double GetPolylineSegmentAngle(Polyline polyline, Point3d pickedPoint)
        {
            var segmentStart = 0;
            var closestPoint = polyline.GetClosestPointTo(pickedPoint, false);

            for (var i = 1; i < polyline.NumberOfVertices - 1; i++)
            {
                if (!polyline.OnSegmentAt(i, closestPoint.ToPoint2d(), 0))
                    continue;

                // If the closest point is on the segment, then we're done.
                segmentStart = i;
                break;
            }

            var segment = polyline.GetLineSegment2dAt(segmentStart);
            bool ordinaryAngle = MathHelpers.IsOrdinaryAngle(segment.StartPoint.ToPoint(), segment.EndPoint.ToPoint());

            if (!ordinaryAngle)
            {
                // if it isn't an ordinary angle, we flip it.
                return AngleHelpers.RadiansToAngle(segment.Direction.Angle).Flip().ToRadians();
            }

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
            for (int j = 0; j <= polyline3d.EndParam; j++)
            {
                Point3d point = polyline3d.GetPointAtParameter(j);
                polyline.AddVertexAt(j, new Point2d(point.X, point.Y), 0, 0, 0);
            }

            return GetPolylineSegmentAngle(polyline, pickedPoint);
        }

        public static ObjectId DrawPolyline3d(Transaction tr, BlockTableRecord btr, Point3dCollection points, string layerName, bool closed = false)
        {
            ObjectId id;
            using (var pLine3d = new Polyline3d(Poly3dType.SimplePoly, points, closed) { Layer = layerName })
            {
                id = btr.AppendEntity(pLine3d);
                tr.AddNewlyCreatedDBObject(pLine3d, true);
            }
            return id;
        }

        public static ObjectId DrawPolyline2d(Transaction tr, BlockTableRecord btr, Point3dCollection points, string layerName, bool closed = false)
        {
            ObjectId id;
            using (var pLine2d = new Polyline2d(Poly2dType.SimplePoly, points, 0, closed, 0, 0, null))
            {
                using (var pLine = new Polyline())
                {
                    pLine.ConvertFrom(pLine2d, false);
                    pLine.Layer = layerName;
                    pLine.Elevation = 0;
                    id = btr.AppendEntity(pLine);
                    tr.AddNewlyCreatedDBObject(pLine, true);
                }
            }
            return id;
        }

        /// <summary>
        /// Gets the segment index of the selected polyline segment.
        /// </summary>
        /// <param name="polyline">The polyline.</param>
        /// <param name="nestedEntity">The nested entity.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="ArgumentNullException">polyline</exception>
        /// <exception cref="ArgumentNullException">nestedEntity</exception>
        public static int GetPolylineSegment(Polyline polyline, PromptNestedEntityResult nestedEntity)
        {
            if (polyline == null)
                throw new ArgumentNullException(nameof(polyline));

            if (nestedEntity == null)
                throw new ArgumentNullException(nameof(nestedEntity));

            // Transform picked point from current UCS to WCS.
            Point3d wcsPickedPoint = nestedEntity.PickedPoint.TransformBy(AcadApp.Editor.CurrentUserCoordinateSystem);

            // Get the closest point to picked point on the polyline.
            // If the polyline is nested, it's needed to transform the picked point using the
            // the transformation matrix that is applied to the polyline by its containers.
            var pointOnPolyline = nestedEntity.GetContainers().Length == 0 ?
                polyline.GetClosestPointTo(wcsPickedPoint, false) : // Not nested polyline.
                polyline.GetClosestPointTo(wcsPickedPoint.TransformBy(nestedEntity.Transform.Inverse()), false); // Nested polyline

            // Get the selected segment index.
            return (int)polyline.GetParameterAtPoint(pointOnPolyline);
        }

        /// <summary>
        /// Gets the <see cref="Line"/> segment from polyline.
        /// </summary>
        /// <param name="polyline">The polyline.</param>
        /// <param name="pickedPoint">The picked point.</param>
        /// <returns>Line.</returns>
        public static Line GetLineSegmentFromPolyline(this Polyline polyline, Point3d pickedPoint)
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

            var segment = polyline.GetLineSegmentAt(segmentStart);
            return new Line(segment.StartPoint, segment.EndPoint);
        }

        public static RadiusPoint SegmentRadiusPoint(this Polyline polyline, double param)
        {
            var radiusPoint = new RadiusPoint();
            double bulgeAt = polyline.GetBulgeAt((int) param);

            if (Math.Abs(bulgeAt) > 0.0)
            {
                var secondDerivative = polyline.GetSecondDerivative(param);

                int bulgeDirection;
                if (bulgeAt > 0.0)
                {
                    bulgeDirection = -1;
                }
                else
                {
                    bulgeDirection = 1;
                }

                radiusPoint.Radius = secondDerivative.Length * bulgeDirection;

                var num2 = secondDerivative.AngleOnPlane(GeometryUtils.PlaneXY);
                var pointAtParameter = polyline.GetPointAtParameter(param);

                radiusPoint.Point = new Point(
                    pointAtParameter.X - Math.Cos(num2) * radiusPoint.Radius,
                    pointAtParameter.Y - Math.Sin(num2) * radiusPoint.Radius, 0.0);
            }

            return radiusPoint;
        }

        /*private static Point3d GetPointAtParameter3ds(this Curve oCurve, double param)
        {
            // If not closed and startpoint is not equal endpoint
            // and param is greater than or equal to endparam
            // return curve endpoint.
            if (!oCurve.Closed && oCurve.StartPoint != oCurve.EndPoint && param >= oCurve.EndParam)
            {
                return oCurve.EndPoint;
            }

            // if param = 0 and curve is an alignment
            // return the start point.
            if (param == 0.0 && oCurve.ObjectId.ObjectClass.DxfName.Contains("ALIGNMENT"))
            {
                return oCurve.StartPoint;
            }

            // else use the normal parameter method.
            return oCurve.GetPointAtParameter(param);
        }*/
    }
}
