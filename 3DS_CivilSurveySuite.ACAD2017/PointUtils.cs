// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class PointUtils
    {
        //TODO: Move transaction out of this method and take it as a parameter.
        public static void CreatePoint(Point3d position)
        {
            using (Transaction tr = AcadApp.StartTransaction())
            {
                // Open the Block table for read
                var bt = tr.GetObject(AcadApp.ActiveDocument.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
 
                if (bt == null)
                    return;

                // Open the Block table record Model space for write
                var btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
 
                if (btr == null)
                    return;

                DBPoint acPoint = new DBPoint(position);
                acPoint.SetDatabaseDefaults();
 
                // Add the new object to the block table record and the transaction
                btr.AppendEntity(acPoint);
                tr.AddNewlyCreatedDBObject(acPoint, true);
 
                // Save the new object to the database
                tr.Commit();
            }
        }
    }
}