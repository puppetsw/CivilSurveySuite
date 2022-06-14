using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Shared.Helpers;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Point = _3DS_CivilSurveySuite.Shared.Models.Point;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    /// <summary>
    /// Connects CogoPoints with linework.
    /// </summary>
    public class ConnectLineworkService : IConnectLineworkService
    {
        public string DescriptionKeyFile { get; set; }

        public Task ConnectCogoPoints(IReadOnlyList<DescriptionKey> descriptionKeys)
        {
            using (Transaction tr = AcadApp.StartLockedTransaction())
            {
                var desMapping = new Dictionary<string, DescriptionKeyMatch>();

                foreach (ObjectId pointId in C3DApp.ActiveDocument.CogoPoints)
                {
                    var cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;
                    if (cogoPoint == null)
                    {
                        continue;
                    }

                    foreach (DescriptionKey descriptionKey in descriptionKeys)
                    {
                        if (!DescriptionKeyMatch.IsMatch(cogoPoint.RawDescription, descriptionKey, AcadApp.Logger))
                        {
                            continue;
                        }

                        string description = DescriptionKeyMatch.Description(cogoPoint.RawDescription, descriptionKey);
                        string lineNumber = DescriptionKeyMatch.LineNumber(cogoPoint.RawDescription, descriptionKey);
                        string specialCode = DescriptionKeyMatch.SpecialCode(cogoPoint.RawDescription, descriptionKey);

                        DescriptionKeyMatch deskeyMatch;
                        if (desMapping.ContainsKey(description))
                        {
                            deskeyMatch = desMapping[description];
                        }
                        else
                        {
                            deskeyMatch = new DescriptionKeyMatch(descriptionKey);
                            desMapping.Add(description, deskeyMatch);
                        }

                        deskeyMatch.AddCogoPoint(cogoPoint.ToCivilPoint(), lineNumber, specialCode, AcadApp.Logger);
                    }
                }

                var bt = (BlockTable) tr.GetObject(AcadApp.ActiveDocument.Database.BlockTableId, OpenMode.ForRead);
                var btr = (BlockTableRecord) tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                foreach (var desKey in desMapping)
                {
                    DescriptionKeyMatch deskeyMatch = desKey.Value;

                    foreach (var joinablePoints in deskeyMatch.JoinablePoints)
                    {
                        var pointList = new List<Point3dCollection>();
                        Point3dCollection points = new Point3dCollection();

                        for (var i = 0; i < joinablePoints.Value.Count; i++)
                        {
                            var point = joinablePoints.Value[i];

                            try
                            {
                                if (point.HasSpecialCode)
                                {
                                    if (point.SpecialCode.Contains(".S") && points.Count != 0)
                                    {
                                        pointList.Add(points);
                                        points = new Point3dCollection();
                                    }

                                    switch (point.SpecialCode)
                                    {
                                        case ".SL":
                                        case ".L":
                                            points.Add(CalculateReturnLegPoint(point, joinablePoints.Value[i + 1], LegDirection.Left));
                                            break;
                                        case ".SR":
                                        case ".R":
                                            points.Add(CalculateReturnLegPoint(point, joinablePoints.Value[i + 1], LegDirection.Right));
                                            break;
                                    }
                                }
                            }
                            catch (IndexOutOfRangeException e)
                            {
                                AcadApp.Logger.Error(e, e.Message);
                            }
                            finally
                            {
                                points.Add(new Point3d(point.CivilPoint.Easting, point.CivilPoint.Northing, point.CivilPoint.Elevation));
                            }
                        }

                        pointList.Add(points);

                        string layerName = deskeyMatch.DescriptionKey.Layer;

                        if (!LayerUtils.HasLayer(layerName, tr))
                        {
                            LayerUtils.CreateLayer(layerName, tr);
                        }

                        foreach (var list in pointList)
                        {
                            if (deskeyMatch.DescriptionKey.Draw2D)
                            {
                                PolylineUtils.DrawPolyline2d(tr, btr, list, layerName);
                            }

                            if (deskeyMatch.DescriptionKey.Draw3D)
                            {
                                PolylineUtils.DrawPolyline3d(tr, btr, list, layerName);
                            }
                        }
                    }
                }
                tr.Commit();
            }
            return Task.CompletedTask;
        }

        private enum LegDirection
        {
            Left,
            Right
        }

        private static Point3d CalculateReturnLegPoint(JoinablePoint point1, JoinablePoint point2, LegDirection legDirection, double distance = 2)
        {
            var pt1 = new Point(point1.CivilPoint.Easting, point1.CivilPoint.Northing);
            var pt2 = new Point(point2.CivilPoint.Easting, point2.CivilPoint.Northing);

            var forwardAngle = AngleHelpers.GetAngleBetweenPoints(pt1, pt2);

            switch (legDirection)
            {
                case LegDirection.Left:
                    forwardAngle -= 90;
                    break;
                case LegDirection.Right:
                    forwardAngle += 90;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(legDirection), legDirection, null);
            }

            var newPt = PointHelpers.AngleAndDistanceToPoint(forwardAngle, distance, pt1);

            return newPt.ToPoint3d();
        }
    }
}
