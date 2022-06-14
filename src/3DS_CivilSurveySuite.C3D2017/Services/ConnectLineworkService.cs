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
                            if (point.HasSpecialCode)
                            {
                                if (point.SpecialCode.Contains(".S") && points.Count != 0)
                                {
                                    pointList.Add(points);
                                    points = new Point3dCollection();
                                }

                                if (point.SpecialCode.Contains(".SL") || point.SpecialCode.Contains(".L"))
                                {
                                    // Calculate left point.
                                    // Look ahead two points
                                    var point1 = new Point(point.CivilPoint.Easting, point.CivilPoint.Northing);
                                    var nextPt = joinablePoints.Value[i + 1];

                                    var point2 = new Point(nextPt.CivilPoint.Easting, nextPt.CivilPoint.Northing);
                                    var forwardAngle = AngleHelpers.GetAngleBetweenPoints(point1, point2);
                                    forwardAngle -= 90;

                                    // Calculate left point.
                                    const double distance = 2.0;
                                    var pt = PointHelpers.AngleAndDistanceToPoint(forwardAngle, distance, point1);

                                    points.Add(pt.ToPoint3d());
                                }
                            }

                            points.Add(new Point3d(point.CivilPoint.Easting, point.CivilPoint.Northing, point.CivilPoint.Elevation));
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
    }
}
