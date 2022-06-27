using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Shared.Extensions;
using _3DS_CivilSurveySuite.Shared.Helpers;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class SurveyPointList : List<SurveyPoint>
    {
        public ObjectId Polyline3d { get; set; }

        public ObjectId Polyline2d { get; set; }
    }

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

                    foreach (var surveyPoints in deskeyMatch.SurveyPoints)
                    {
                        var pointList = new List<SurveyPointList>();
                        var points = new SurveyPointList();

                        for (var i = 0; i < surveyPoints.Value.Count; i++)
                        {
                            var point = surveyPoints.Value[i];

                            try
                            {
                                if (point.HasSpecialCode)
                                {
                                    if (point.SpecialCode.Equals(".S") && points.Count != 0)
                                    {
                                        pointList.Add(points);
                                        points = new SurveyPointList();
                                    }

                                    switch (point.SpecialCode)
                                    {
                                        case ".SL":
                                        case ".L":
                                        {
                                            var point1 = point.CivilPoint.ToPoint();
                                            var point2 = surveyPoints.Value[i + 1].CivilPoint.ToPoint();
                                            var newPoint = PointHelpers.CalculateRightAngleTurn(point1, point2);

                                            var cloned = (SurveyPoint)point.Clone();
                                            cloned.CivilPoint.Easting = newPoint.X;
                                            cloned.CivilPoint.Northing = newPoint.Y;
                                            cloned.CivilPoint.Elevation = newPoint.Z;

                                            points.Add(cloned);
                                            break;
                                        }
                                        case ".SR":
                                        case ".R":
                                        {
                                            var point1 = point.CivilPoint.ToPoint();
                                            var point2 = surveyPoints.Value[i + 1].CivilPoint.ToPoint();
                                            var newPoint = PointHelpers.CalculateRightAngleTurn(point1, point2, false);

                                            var cloned = (SurveyPoint)point.Clone();
                                            cloned.CivilPoint.Easting = newPoint.X;
                                            cloned.CivilPoint.Northing = newPoint.Y;
                                            cloned.CivilPoint.Elevation = newPoint.Z;

                                            points.Add(cloned);
                                            break;
                                        }
                                        case ".RECT":
                                        {
                                            var point1 = point.CivilPoint.ToPoint();
                                            var point2 = surveyPoints.Value[i - 1].CivilPoint.ToPoint();
                                            var point3 = surveyPoints.Value[i - 2].CivilPoint.ToPoint();
                                            var newPoint = PointHelpers.CalculateRectanglePoint(point1, point2, point3);
                                            var averageZ = (point1.Z + point2.Z + point3.Z) / 3;

                                            var cloned = (SurveyPoint)point.Clone();
                                            cloned.CivilPoint.Easting = newPoint.X;
                                            cloned.CivilPoint.Northing = newPoint.Y;
                                            cloned.CivilPoint.Elevation = averageZ;

                                            points.Add(point);
                                            points.Add(cloned);
                                            continue;
                                        }
                                        case ".CLS":
                                        {

                                            break;
                                        }
                                    }
                                }
                                points.Add(point);
                            }
                            catch (IndexOutOfRangeException e)
                            {
                                AcadApp.Logger.Info($"Coding error at: PT#{point.CivilPoint.PointNumber}, DES:{point.CivilPoint.RawDescription}");
                                AcadApp.Logger.Error(e, e.Message);
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
                            var pointCollection = new Point3dCollection();
                            for (var index = 0; index < list.Count; index++)
                            {
                                var surveyPoint = list[index];

                                if (surveyPoint.IsProcessed)
                                {
                                    continue;
                                }

                                if (surveyPoint.StartCurve)
                                {
                                    AcadApp.Logger.Info($"Start Curve: {surveyPoint.CivilPoint.PointNumber}, {surveyPoint.CivilPoint.RawDescription}");

                                    surveyPoint.IsProcessed = true;
                                    pointCollection.Add(surveyPoint.CivilPoint.ToPoint().ToPoint3d());

                                    if (!list[index + 2].EndCurve)
                                    {
                                        continue;
                                    }

                                    // Convert ARC to 3d polyline with grading

                                    AcadApp.Logger.Info($"End Curve: {surveyPoint.CivilPoint.PointNumber}, {surveyPoint.CivilPoint.RawDescription}");

                                    var startPoint = surveyPoint.CivilPoint.ToPoint();
                                    var midPoint = list[index + 1].CivilPoint.ToPoint();
                                    var endPoint = list[index + 2].CivilPoint.ToPoint();
                                    var arc = new CircularArc3d(startPoint.ToFlatPoint3d(), midPoint.ToFlatPoint3d(), endPoint.ToFlatPoint3d());
                                    var arcPoints = arc.CurvePoints();

                                    foreach (var arcPoint in arcPoints)
                                    {
                                        // Grade in reverse?
                                        //BUG: Grade ignores midpoint
                                        //double distance = curve.GetDistAtPoint(curve.GetClosestpointTo(pickedPoint));
                                        var gradedPoint = arcPoint.ToPoint().SetElevationOnGrade(endPoint, startPoint).ToPoint3d();
                                        pointCollection.Add(gradedPoint);
                                    }

                                    // Set the midpoint of arc to processed.
                                    // So that it doesn't get added again.
                                    list[index + 1].IsProcessed = true;
                                }
                                else
                                {
                                    surveyPoint.IsProcessed = true;
                                    pointCollection.Add(surveyPoint.CivilPoint.ToPoint().ToPoint3d());
                                }
                            }

                            if (deskeyMatch.DescriptionKey.Draw2D)
                            {
                                list.Polyline2d = PolylineUtils.DrawPolyline2d(tr, btr, pointCollection, layerName);
                            }

                            if (deskeyMatch.DescriptionKey.Draw3D)
                            {
                                list.Polyline3d = PolylineUtils.DrawPolyline3d(tr, btr, pointCollection, layerName);
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
