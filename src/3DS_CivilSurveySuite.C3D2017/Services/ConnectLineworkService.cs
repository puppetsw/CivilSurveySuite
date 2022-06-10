// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
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

        // ReSharper disable once CognitiveComplexity
        public void ConnectCogoPoints(IReadOnlyList<DescriptionKey> descriptionKeys)
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

                        deskeyMatch.AddCogoPoint(cogoPoint.ToCivilPoint(), lineNumber);
                    }
                }

                var bt = (BlockTable) tr.GetObject(AcadApp.ActiveDocument.Database.BlockTableId, OpenMode.ForRead);
                var btr = (BlockTableRecord) tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                //TODO: add special code checks in here?
                foreach (var desKey in desMapping)
                {
                    DescriptionKeyMatch deskeyMatch = desKey.Value;

                    AcadApp.Logger.Info($"Joining {deskeyMatch.DescriptionKey.Key}");

                    foreach (var joinablePoints in deskeyMatch.JoinablePoints)
                    {
                        Point3dCollection points = new Point3dCollection();
                        foreach (CivilPoint point in joinablePoints.Value)
                        {
                            points.Add(new Point3d(point.Easting, point.Northing, point.Elevation));
                            AcadApp.Logger.Info($"Connectable point: x:{point.Easting}, y:{point.Northing} DesKey: {point.RawDescription}");
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
        }
    }
}
