// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class PointCreateOnProductionOfLine
    {
        public static void RunCommand()
        {
            using (Transaction tr = AcadUtils.StartTransaction())
            {
                if (!EditorUtils.GetNestedEntity(out PromptNestedEntityResult lineResult, "\n3DS> Select line or polyline: "))
                    return;

                if (!lineResult.ObjectId.IsType(new[] { typeof(Polyline), typeof(Line) }))
                    return;

                Point3d basePoint = default;
                Line line = null;

                try
                {
                    if (lineResult.ObjectId.IsType<Line>())
                    {
                        line = lineResult.ObjectId.GetObject(OpenMode.ForRead) as Line;
                        basePoint = line.GetClosestEndPoint(lineResult.PickedPoint);
                    }

                    if (lineResult.ObjectId.IsType<Polyline>())
                    {
                        var polyline = lineResult.ObjectId.GetObject(OpenMode.ForRead) as Polyline;
                        line = polyline.GetLineSegmentFromPolyline(lineResult.PickedPoint);
                        basePoint = line.GetClosestEndPoint(lineResult.PickedPoint);
                    }

                    if (line == null)
                        return;

                    Angle angle = MathHelpers.AngleBetweenPoints(line.StartPoint.ToPoint(), line.EndPoint.ToPoint());

                    if (basePoint == line.StartPoint)
                    {
                        angle = angle.Flip();
                    }

                    if (!EditorUtils.GetDistance(out double dist, "\n3DS> Offset distance: ", basePoint))
                        return;

                    var pko = new PromptKeywordOptions("\n3DS> Accept point position? ") { AppendKeywordsToMessage = true, AllowNone = true };
                    pko.Keywords.Add(Keywords.Accept);
                    pko.Keywords.Add(Keywords.Cancel);
                    pko.Keywords.Add(Keywords.Flip);
                    pko.Keywords.Default = Keywords.Accept;

                    Point point = MathHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());

                    using (var graphics = new TransientGraphics())
                    {
                        graphics.DrawCircle(point.ToPoint3d());
                        graphics.DrawLine(basePoint, point.ToPoint3d());

                        var cancelled = false;
                        PromptResult prResult;
                        do
                        {
                            prResult = AcadUtils.Editor.GetKeywords(pko);

                            if (prResult.Status != PromptStatus.Keyword &&
                                prResult.Status != PromptStatus.OK)
                                continue;

                            switch (prResult.StringResult)
                            {
                                case Keywords.None: // If user doesn't enter anything.
                                case Keywords.Accept:
                                    Points.CreatePoint(point.ToPoint3d());
                                    cancelled = true;
                                    break;
                                case Keywords.Cancel:
                                    cancelled = true;
                                    break;
                                case Keywords.Flip:
                                    angle = angle.Flip();
                                    point = MathHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());
                                    graphics.ClearGraphics();
                                    graphics.DrawCircle(point.ToPoint3d());
                                    graphics.DrawLine(basePoint, point.ToPoint3d());
                                    break;
                            }
                        } while (prResult.Status != PromptStatus.Cancel &&
                                 prResult.Status != PromptStatus.Error && !cancelled);
                    }

                    tr.Commit();
                }
                catch (Exception e)
                {
                    AcadUtils.Editor.WriteMessage(e.ToString());
                }
            }
        }
    }
}