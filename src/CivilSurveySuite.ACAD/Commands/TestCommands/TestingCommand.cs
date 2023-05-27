using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CivilSurveySuite.Shared.Services.Interfaces;

namespace CivilSurveySuite.ACAD
{
    public class TestingCommand : IAcadCommand
    {
        public void Execute()
        {
            if (!EditorUtils.TryGetEntityOfType<Curve>("", "", out var polylineId))
            {
                return;
            }

            if (!EditorUtils.TryGetSelectionOfType<DBPoint>("", "", out var pointIds))
            {
                return;
            }

            using (var tr = AcadApp.StartTransaction())
            {
                var bt = (BlockTable) tr.GetObject(AcadApp.ActiveDocument.Database.BlockTableId, OpenMode.ForRead);
                var btr = (BlockTableRecord) tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                var sourcePoints = new Point3dCollection();
                foreach (ObjectId pointId in pointIds)
                {
                    var point = (DBPoint)tr.GetObject(pointId, OpenMode.ForRead);
                    sourcePoints.Add(point.Position);
                }

                var polyline = (Polyline)tr.GetObject(polylineId, OpenMode.ForRead);

                var points = new Point3dCollection();
                for (int i = 0; i < polyline.EndParam; i++)
                {
                    var polyLineVertex = polyline.GetPointAtParameter(i);

                    var radiusPoint = polyline.SegmentRadiusPoint(i);
                    if (!radiusPoint.IsArc())
                    {
                        points.Add(new Point3d(polyLineVertex.X, polyLineVertex.Y, sourcePoints[i].Z));
                    }
                    else
                    {

                    }
                }

                using (var pLine3d = new Polyline3d(Poly3dType.SimplePoly, points, false))
                {
                    btr.AppendEntity(pLine3d);
                    tr.AddNewlyCreatedDBObject(pLine3d, true);
                }

                tr.Commit();
            }
        }
    }
}
