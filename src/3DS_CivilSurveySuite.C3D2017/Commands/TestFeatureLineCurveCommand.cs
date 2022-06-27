using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public class TestFeatureLineCurveCommand : IAcadCommand
    {
        public void Execute()
        {
            if (!EditorUtils.TryGetPoint("\nFirst point", out Point3d point1))
                return;

            if (!EditorUtils.TryGetPoint("\nSecond point", out Point3d point2))
                return;

            if (!EditorUtils.TryGetPoint("\nThird point", out Point3d point3))
                return;

            var bulge = CurveUtils.CalculateBulge(point1, point2, point3, 0);

            Point3dCollection points = new Point3dCollection();
            points.Add(point1);
            points.Add(point2);
            points.Add(point3);

            using (var tr = AcadApp.StartTransaction())
            {
                var bt = (BlockTable) tr.GetObject(AcadApp.ActiveDocument.Database.BlockTableId, OpenMode.ForRead);
                var btr = (BlockTableRecord) tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                var pline = new Polyline();

                pline.AddVertexAt(0, new Point2d(point1.X, point1.Y), bulge, 0, 0);
                // pline.AddVertexAt(0, new Point2d(point2.X, point2.Y), 0, 0, 0); // don't add midpoint
                pline.AddVertexAt(1, new Point2d(point3.X, point3.Y), 0, 0, 0);

                var plineId = btr.AppendEntity(pline);
                tr.AddNewlyCreatedDBObject(pline, true);



                var site = SiteUtils.GetSite(tr, "Site");

                var id = FeatureLine.Create("", plineId, site.ObjectId);

                var featureLine = (FeatureLine)tr.GetObject(id, OpenMode.ForWrite);
                featureLine.SetPointElevation(0, 100);

                var pt = featureLine.GetClosestPointTo(point2, true);
                featureLine.InsertElevationPoint(pt);


                tr.Commit();
            }

        }
    }
}
