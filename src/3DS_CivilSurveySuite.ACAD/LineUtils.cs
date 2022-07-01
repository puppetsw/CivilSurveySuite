﻿// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Shared.Helpers;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.UI.Helpers;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD
{
    public static class LineUtils
    {
        public static void DrawLines(Transaction tr, IReadOnlyList<Point3d> coordinates)
        {
            var i = 1;
            foreach (Point3d point in coordinates)
            {
                if (coordinates.Count == i)
                {
                    break;
                }

                DrawLine(tr, point, coordinates[i]);
                i++;
            }
        }

        public static void DrawLines(Transaction tr, IReadOnlyList<Point2d> coordinates)
        {
            var i = 1;
            foreach (Point2d point in coordinates)
            {
                if (coordinates.Count == i)
                {
                    break;
                }

                DrawLine(tr, new Point3d(point.X, point.Y, 0), new Point3d(coordinates[i].X, coordinates[i].Y, 0));
                i++;
            }
        }

        public static void DrawLine(Transaction tr, Point3d startPoint, Point3d endPoint)
        {
            if (tr == null)
            {
                throw new ArgumentNullException(nameof(tr), @"Transaction was null.");
            }

            var blockTable = (BlockTable) tr.GetObject(AcadApp.ActiveDocument.Database.BlockTableId, OpenMode.ForRead);
            var blockTableRecord = (BlockTableRecord) tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

            var line = new Line(startPoint, endPoint);

            blockTableRecord.AppendEntity(line);
            tr.AddNewlyCreatedDBObject(line, true);
        }

        public static Line Offset(Line originalLine, double offsetDistance, Point3d pickedSide)
        {
            // Work out direction/angle of originalLine
            var startPoint = originalLine.StartPoint.ToPoint();
            var endPoint = originalLine.EndPoint.ToPoint();
            var pickedPoint = pickedSide.ToPoint();

            int side = MathHelpers.IsLeft(startPoint, endPoint, pickedPoint);

            if (side == 0) // If pickedSide is on the line exit.
                return null;

            offsetDistance *= side;

            var pointDbObjectCollection = originalLine.GetOffsetCurves(offsetDistance);

            if (pointDbObjectCollection.Count < 1)
                return null;

            return pointDbObjectCollection[0] as Line;
        }

        public static Line GetLineOrPolylineSegment(Transaction tr)
        {
            if (!EditorUtils.TryGetNestedEntity("\n3DS> Select Line or Polyline segment:", out var firstLineResult))
                return null;

            if (!firstLineResult.ObjectId.IsType(new[] { typeof(Polyline), typeof(Line) }))
                return null;

            Line line = null;

            if (EditorUtils.IsType(firstLineResult.ObjectId, typeof(Line)))
            {
                line = tr.GetObject(firstLineResult.ObjectId, OpenMode.ForRead) as Line;
            }

            if (EditorUtils.IsType(firstLineResult.ObjectId, typeof(Polyline)))
            {
                var polyline = (Polyline)tr.GetObject(firstLineResult.ObjectId, OpenMode.ForRead);
                var segmentId = PolylineUtils.GetPolylineSegment(polyline, firstLineResult);
                var segment = polyline.GetLineSegment2dAt(segmentId);
                line = new Line(segment.StartPoint.ToPoint3d(), segment.EndPoint.ToPoint3d());
            }

            return line == null ? null : line;
        }

        public static Line GetNearestPointOfLineOrPolylineSegment(Transaction tr, out Point3d endPoint)
        {
            endPoint = default;

            if (!EditorUtils.TryGetNestedEntity("\n" + ResourceHelpers.GetLocalisedString("ACAD_SelectLineOrPolyline"), out var lineResult))
                return null;

            if (!lineResult.ObjectId.IsType(new[] { typeof(Polyline), typeof(Line) }))
                return null;

            Line line = null;

            if (lineResult.ObjectId.IsType<Line>())
            {
                line = tr.GetObject(lineResult.ObjectId, OpenMode.ForRead) as Line;
                endPoint = line.GetClosestEndPoint(lineResult.PickedPoint);
            }

            if (lineResult.ObjectId.IsType<Polyline>())
            {
                var polyline = tr.GetObject(lineResult.ObjectId, OpenMode.ForRead) as Polyline;
                line = polyline.GetLineSegmentFromPolyline(lineResult.PickedPoint);
                endPoint = line.GetClosestEndPoint(lineResult.PickedPoint);
            }

            return line;
        }

        public static Angle GetAngleOfLine(Line line)
        {
            return AngleHelpers.GetAngleBetweenPoints(line.StartPoint.ToPoint(), line.EndPoint.ToPoint());
        }

        public static Point FindIntersectionPoint(Line line1, Line line2)
        {
            var p1 = new Vector(line1.StartPoint.X, line1.StartPoint.Y);
            var p2 = new Vector(line1.EndPoint.X, line1.EndPoint.Y);
            var q1 = new Vector(line2.StartPoint.X, line2.StartPoint.Y);
            var q2 = new Vector(line2.EndPoint.X, line2.EndPoint.Y);

            MathHelpers.LineSegementsIntersect(p1, p2, q1, q2, out Point intersectionPoint);

            return intersectionPoint;
        }

        public static Point3d GetClosestEndPoint(this Line line, Point3d pickedPoint)
        {
            Point3d closetPoint = line.GetClosestPointTo(pickedPoint, false);
            double param = Math.Round(line.GetParameterAtPoint(closetPoint));

            if (param < line.Length / 2)
            {
                return line.StartPoint;
            }

            return line.EndPoint;
        }



        public static void Draw_Leg_Line()
        {
            var graphics = new TransientGraphics();
            try
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Pick first point on line: ", out Point3d firstPoint))
                    return;

                graphics.DrawPlus(firstPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetPoint("\n3DS> Pick second point on line: ", out Point3d secondPoint))
                    return;

                graphics.DrawPlus(secondPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetDistance("\n3DS> Enter leg distance: ", out double distance))
                    return;

                // Get angle and add 90° to it.
                var angle = AngleHelpers.GetAngleBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint()) + 90;
                var newPoint = PointHelpers.AngleAndDistanceToPoint(angle, distance, firstPoint.ToPoint());

                graphics.DrawLine(firstPoint, newPoint.ToPoint3d());

                var pko = new PromptKeywordOptions("\n3DS> Accept leg? ") { AppendKeywordsToMessage = true, AllowNone = true };
                pko.Keywords.Add(Keywords.ACCEPT);
                pko.Keywords.Add(Keywords.FLIP);
                pko.Keywords.Default = Keywords.ACCEPT;

                PromptResult prResult = AcadApp.Editor.GetKeywords(pko);

                using (var tr = AcadApp.StartTransaction())
                {
                    switch (prResult.StringResult)
                    {
                        case Keywords.ACCEPT:
                            DrawLine(tr, firstPoint, newPoint.ToPoint3d());
                            break;
                        case Keywords.FLIP:
                            angle = angle.Flip();
                            newPoint = PointHelpers.AngleAndDistanceToPoint(angle, distance, firstPoint.ToPoint());
                            graphics.DrawLine(firstPoint, newPoint.ToPoint3d());
                            DrawLine(tr, firstPoint, newPoint.ToPoint3d());
                            break;
                    }
                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                AcadApp.Editor.WriteMessage($"\n3DS> Command exception: {e.Message}");
            }
            finally
            {
                graphics.Dispose();
            }
        }







    }
}