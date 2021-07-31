// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.ACAD2017.Extensions;
using _3DS_CivilSurveySuite.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD2017.AcadUtils
{
    public static class Lines
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
                throw new ArgumentNullException(nameof(tr), "Transaction was null.");
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
    }
}