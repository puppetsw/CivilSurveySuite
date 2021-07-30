// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public class ConnectLineworkService : IConnectLineworkService
    {
        public void ConnectCogoPoints(IReadOnlyList<DescriptionKey> descriptionKeys)
        {
            using (Transaction tr = AcadUtils.StartLockedTransaction())
            {
                var desMapping = new Dictionary<string, DescriptionKeyMatch>();

                foreach (ObjectId pointId in C3DUtils.ActiveCivilDocument.CogoPoints)
                {
                    //BUG: Seems like there could be a problem with the keys and stuff if they don't match up
                    //TODO: add way to check for special codes e.g. SL or RECT
                    CogoPoint cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;
                    /* in each DescriptionKeyMatch add a collection of join-able points separated by line number.
                       using dictionaries with the key being the raw description and then the line number.?
                     */

                    if (cogoPoint == null)
                    {
                        continue;
                    }

                    foreach (DescriptionKey descriptionKey in descriptionKeys)
                    {
                        if (DescriptionKeyMatch.IsMatch(cogoPoint.RawDescription, descriptionKey))
                        {
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

                            deskeyMatch.AddCogoPoint(cogoPoint, lineNumber);
                        }
                    }
                }

                var bt = (BlockTable) tr.GetObject(AcadUtils.ActiveDocument.Database.BlockTableId, OpenMode.ForRead);
                var btr = (BlockTableRecord) tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                //TODO: add special code checks in here?
                foreach (var desKey in desMapping)
                {
                    DescriptionKeyMatch deskeyMatch = desKey.Value;

                    foreach (var joinablePoints in deskeyMatch.JoinablePoints)
                    {
                        Point3dCollection points = new Point3dCollection();
                        foreach (CogoPoint point in joinablePoints.Value)
                        {
                            points.Add(point.Location);
                        }

                        string layerName = deskeyMatch.DescriptionKey.Layer;

                        //Check if the layer exists, if not create it.
                        if (!Layers.HasLayer(layerName, tr))
                        {
                            Layers.CreateLayer(layerName, tr);
                        }

                        if (deskeyMatch.DescriptionKey.Draw2D)
                        {
                            Polylines.DrawPolyline2d(tr, btr, points, layerName);
                        }

                        if (deskeyMatch.DescriptionKey.Draw3D)
                        {
                            Polylines.DrawPolyline3d(tr, btr, points, layerName);
                        }
                    }
                }

                tr.Commit();
            }
        }
    }
}