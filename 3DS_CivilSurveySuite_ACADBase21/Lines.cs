// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite_ACADBase21
{
    public class Lines
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

            var blockTable = (BlockTable) tr.GetObject(AutoCADApplicationManager.ActiveDocument.Database.BlockTableId, OpenMode.ForRead);
            var blockTableRecord = (BlockTableRecord) tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

            var line = new Line(startPoint, endPoint);

            blockTableRecord.AppendEntity(line);
            tr.AddNewlyCreatedDBObject(line, true);
        }
    }
}