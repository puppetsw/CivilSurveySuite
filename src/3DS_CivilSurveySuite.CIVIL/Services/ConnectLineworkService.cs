using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.ACAD;
using _3DS_CivilSurveySuite.Shared.Extensions;
using _3DS_CivilSurveySuite.Shared.Helpers;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.CIVIL.Services
{
    public class SurveyPointList : List<SurveyPoint> { }

    public class ConnectLineworkService : IConnectLineworkService
    {
        public string DescriptionKeyFile { get; set; }

        public double MidOrdinate { get; set; } = 0.01;

        private const string TEMPORARY_SITE_NAME = "Survey Import";

        //DEVIL CODE 666%
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
                                            // Don't need to do anything here.
                                            // It's handled by the SurveyPoint constructor.
                                            break;
                                        }
                                    }
                                }
                                points.Add(point);
                            }
                            catch (IndexOutOfRangeException e)
                            {
                                AcadApp.Logger?.Info($"Coding error at: PT#{point.CivilPoint.PointNumber}, DES:{point.CivilPoint.RawDescription}");
                                AcadApp.Logger?.Error(e, e.Message);
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
                            bool hasCurve = false;
                            bool isClosed = false;
                            var pointCollection = new Point3dCollection();
                            var midPointCollection = new Point3dCollection();
                            var polyline = new Polyline();
                            for (var index = 0; index < list.Count; index++)
                            {
                                var surveyPoint = list[index];

                                if (surveyPoint.IsProcessed)
                                {
                                    continue;
                                }

                                if (surveyPoint.Closed)
                                {
                                    isClosed = true;
                                }

                                if (surveyPoint.StartCurve)
                                {
                                    hasCurve = true;
                                    AcadApp.Logger?.Info($"Start Curve: {surveyPoint.CivilPoint.PointNumber}, {surveyPoint.CivilPoint.RawDescription}");
                                    AcadApp.Logger?.Info($"End Curve: {surveyPoint.CivilPoint.PointNumber}, {surveyPoint.CivilPoint.RawDescription}");

                                    surveyPoint.IsProcessed = true;

                                    var startPoint = surveyPoint.CivilPoint.ToPoint();
                                    var midPoint = list[index + 1].CivilPoint.ToPoint();
                                    var endPoint = list[index + 2].CivilPoint.ToPoint();

                                    midPointCollection.Add(midPoint.ToPoint3d());

                                    // Set the midpoint of arc to processed.
                                    // So that it doesn't get added again.
                                    list[index + 1].IsProcessed = true;

                                    var arc = new CircularArc2d(startPoint.ToPoint2d(), midPoint.ToPoint2d(), endPoint.ToPoint2d());
                                    var bulge = arc.GetArcBulge();

                                    pointCollection.Add(surveyPoint.CivilPoint.ToPoint().ToPoint3d());

                                    polyline.AddVertexAt(index, surveyPoint.CivilPoint.ToPoint().ToPoint2d(), bulge, 0, 0);
                                    index++;
                                    polyline.AddVertexAt(index, endPoint.ToPoint2d(), 0, 0, 0);
                                }
                                else
                                {
                                    surveyPoint.IsProcessed = true;
                                    pointCollection.Add(surveyPoint.CivilPoint.ToPoint().ToPoint3d());
                                    polyline.AddVertexAt(index, surveyPoint.CivilPoint.ToPoint().ToPoint2d(), 0, 0, 0);
                                }
                            }

                            // Draw the polylines.
                            if (deskeyMatch.DescriptionKey.Draw2D)
                            {
                                PolylineUtils.DrawPolyline2d(tr, btr, pointCollection, layerName, isClosed);
                            }

                            if (deskeyMatch.DescriptionKey.Draw3D && !hasCurve)
                            {
                                PolylineUtils.DrawPolyline3d(tr, btr, pointCollection, layerName, isClosed);
                            }

                            // Draw featureline if curve is found.
                            if (deskeyMatch.DescriptionKey.Draw3D && hasCurve)
                            {
                                var polylineId = btr.AppendEntity(polyline);
                                tr.AddNewlyCreatedDBObject(polyline, true);

                                if (!SiteUtils.TryCreateSite(tr, TEMPORARY_SITE_NAME, out var siteId))
                                {
                                    continue;
                                }

                                AcadApp.Logger?.Info("TEMPORARY SITE CREATED.");

                                var featureLineId = FeatureLine.Create("", polylineId, siteId);
                                var featureLine = (FeatureLine)tr.GetObject(featureLineId, OpenMode.ForWrite);

                                // Set layer.
                                featureLine.Layer = layerName;

                                // Set elevations.
                                for (int i = 0; i < featureLine.GetPoints(FeatureLinePointType.AllPoints).Count; i++)
                                {
                                    featureLine.SetPointElevation(i, pointCollection[i].Z);
                                }

                                // Add midpoint for curve.
                                for (int i = 0; i < midPointCollection.Count; i++)
                                {
                                    // Get position of the featureline
                                    var pointOnFeatureLine = featureLine.GetClosestPointTo(midPointCollection[i], false);
                                    // Create a new point using the correct height and position.
                                    var midPoint = new Point3d(pointOnFeatureLine.X, pointOnFeatureLine.Y, midPointCollection[i].Z);
                                    featureLine.InsertElevationPoint(midPoint);
                                }

                                // Delete the temporary polyline.
                                polyline.Erase();

                                if (!featureLine.TryConvertTo(tr, out var polyline3d, MidOrdinate))
                                {
                                    AcadApp.Logger?.Warn("Error converting feature line to Polyline.");
                                    continue;
                                }

                                featureLine.Erase();

                                btr.AppendEntity(polyline3d);
                                tr.AddNewlyCreatedDBObject(polyline3d, true);
                            }
                        }
                    }
                }

                // Clean up
                AcadApp.Logger?.Warn(
                    SiteUtils.TryDeleteSite(tr, TEMPORARY_SITE_NAME) ?
                        "TEMPORARY SITE DELETED." :
                        "TEMPORARY SITE NOT DELETED.");

                tr.Commit();
            }
            return Task.CompletedTask;
        }
    }
}
