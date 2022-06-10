///////////////////////////////////////////////////////////////////////////
//FileName: ConnectLineworkService.cs
//FileType: Visual C# Source file
//Author : Scott Whitney
//Created On : 7/8/2015 9:56:39 AM
//Copyright : Scott Whitney. All Rights Reserved.
//Description : Connects CogoPoints with linework.
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
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
                    // FIXED?: Seems like there could be a problem with the keys and stuff if they don't match up
                    // 10/6/22 I wish I was more descriptive with this issue.
                    // 10/6/22 Added upper invariant calls, hopefully solves this issue If I'm understanding what I wrote correctly.

                    // TODO: add way to check for special codes e.g. SL or RECT
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

                var processedList = ProcessSpecialCodes(desMapping);

                foreach (var desKey in desMapping)
                {
                    DescriptionKeyMatch deskeyMatch = desKey.Value;

                    AcadApp.Logger.Info($"Joining {deskeyMatch.DescriptionKey.Key}");

                    // TODO: Change to the processedList.
                    foreach (var joinablePoints in deskeyMatch.JoinablePoints)
                    {
                        Point3dCollection points = new Point3dCollection();
                        foreach (var point in joinablePoints.Value)
                        {
                            points.Add(new Point3d(point.CivilPoint.Easting, point.CivilPoint.Northing, point.CivilPoint.Elevation));
                            AcadApp.Logger.Info($"Connectable point: x:{point.CivilPoint.Easting}, y:{point.CivilPoint.Northing} DesKey: {point.CivilPoint.RawDescription}");
                        }

                        string layerName = deskeyMatch.DescriptionKey.Layer;

                        //Check if the layer exists, if not create it.
                        if (!LayerUtils.HasLayer(layerName, tr))
                        {
                            AcadApp.Logger.Info($"Creating layer: {layerName}");
                            LayerUtils.CreateLayer(layerName, tr);
                        }

                        if (deskeyMatch.DescriptionKey.Draw2D)
                        {
                            PolylineUtils.DrawPolyline2d(tr, btr, points, layerName);
                        }

                        if (deskeyMatch.DescriptionKey.Draw3D)
                        {
                            PolylineUtils.DrawPolyline3d(tr, btr, points, layerName);
                        }
                    }

                    AcadApp.Logger.Info($"Completed joining {deskeyMatch.DescriptionKey.Key}");
                }

                tr.Commit();
            }

            return Task.CompletedTask;
        }

        private static IEnumerable<JoinablePoint> ProcessSpecialCodes(Dictionary<string, DescriptionKeyMatch> desMapping)
        {
            var list = new List<JoinablePoint>();

            foreach (var desKey in desMapping)
            {
                DescriptionKeyMatch deskeyMatch = desKey.Value;
                foreach (var joinablePoint in deskeyMatch.JoinablePoints)
                {
                    for (var index = 0; index < joinablePoint.Value.Count; index++)
                    {
                        JoinablePoint point = joinablePoint.Value[index];

                        if (point.HasSpecialCode)
                        {
                            AcadApp.Logger.Info($"Special Code FOUND! {point.SpecialCode}");

                            // Check for valid special codes.
                            if (!IsValidSpecialCode(point.SpecialCode))
                            {
                                continue;
                            }

                            // Need to be able to look ahead and behind. Use for
                            // This is doing my head in at the moment.
                        }
                    }
                }
            }

            return list;
        }

        private static bool IsValidSpecialCode(string specialCode)
        {
            if (string.IsNullOrEmpty(specialCode))
            {
                throw new ArgumentNullException(nameof(specialCode));
            }

            switch (specialCode)
            {
                case ".SR":
                case ".SL":
                case ".T":
                    return true;
            }

            return false;
        }
    }
}
