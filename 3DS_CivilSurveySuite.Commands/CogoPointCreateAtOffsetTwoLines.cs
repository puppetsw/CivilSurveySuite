// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite_ACADBase21;
using _3DS_CivilSurveySuite_C3DBase21;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace _3DS_CivilSurveySuite.Commands
{
    public static class CogoPointCreateAtOffsetTwoLines
    {
        public static void RunCommand()
        {
            ObjectId firstLineId;
            ObjectId secondLineId;

            if (!EditorUtils.GetNestedEntity(out PromptNestedEntityResult firstLineResult, "\n3DS> Select first line or polyline to offset: "))
                return;

            //if (!EditorUtils.IsOfType(firstLineResult.ObjectId, new[] { typeof(Polyline), typeof(Line) }))
              //  return;

            if (!firstLineResult.ObjectId.IsType(new[] { typeof(Polyline), typeof(Line) }))
                return;

            firstLineId = firstLineResult.ObjectId;
                
            if (!EditorUtils.GetNestedEntity(out PromptNestedEntityResult secondLineResult, "\n3DS> Select second line or polyline to offset: "))
                return;

            if (!secondLineResult.ObjectId.IsType(new[] { typeof(Polyline), typeof(Line) }))
                return;

            secondLineId = secondLineResult.ObjectId;

            // Pick offset side
            if (!EditorUtils.GetBasePoint3d(out Point3d offsetPoint, "\n3DS> Select offset side: "))
                return;

            // Prompt for offset distance
            if (!EditorUtils.GetDistance(out double dist, "\n3DS> Offset distance: "))
                return;

            using (Transaction tr = AutoCADActive.StartTransaction())
            {
                Line firstLineToOffset = null;
                Line secondLineToOffset = null;

                if (EditorUtils.IsType(firstLineId, typeof(Line)))
                {
                    firstLineToOffset = firstLineId.GetObject(OpenMode.ForRead) as Line;
                }

                if (EditorUtils.IsType(firstLineId, typeof(Polyline)))
                {
                    var polyline = firstLineId.GetObject(OpenMode.ForRead) as Polyline;
                    var segmentId = Polylines.GetPolylineSegment(polyline, firstLineResult);
                    var segment = polyline.GetLineSegment2dAt(segmentId);
                    firstLineToOffset = new Line(segment.StartPoint.ToPoint().ToPoint3d(), segment.EndPoint.ToPoint().ToPoint3d());
                }

                if (EditorUtils.IsType(secondLineId, typeof(Line)))
                {
                    secondLineToOffset = secondLineId.GetObject(OpenMode.ForRead) as Line;
                }

                if (EditorUtils.IsType(secondLineId, typeof(Polyline)))
                {
                    var polyline = secondLineId.GetObject(OpenMode.ForRead) as Polyline;
                    var segmentId = Polylines.GetPolylineSegment(polyline, secondLineResult);
                    var segment = polyline.GetLineSegment2dAt(segmentId);
                    secondLineToOffset = new Line(segment.StartPoint.ToPoint().ToPoint3d(), segment.EndPoint.ToPoint().ToPoint3d());
                }

                Curve firstOffsetLine = Lines.Offset(firstLineToOffset, dist, offsetPoint);
                Curve secondOffsetLine = Lines.Offset(secondLineToOffset, dist, offsetPoint);

                if (firstOffsetLine == null || secondOffsetLine == null)
                {
                    AutoCADActive.Editor.WriteMessage("\n3DS> Please select a line or polyline only.");
                    return;
                }

                var p1 = new Vector(firstOffsetLine.StartPoint.X, firstOffsetLine.StartPoint.Y);
                var p2 = new Vector(firstOffsetLine.EndPoint.X, firstOffsetLine.EndPoint.Y);
                var q1 = new Vector(secondOffsetLine.StartPoint.X, secondOffsetLine.StartPoint.Y);
                var q2 = new Vector(secondOffsetLine.EndPoint.X, secondOffsetLine.EndPoint.Y);

                MathHelpers.LineSegementsIntersect(p1, p2, q1, q2, out Point intersectionPoint);
                AutoCADActive.Editor.WriteMessage($"\n3DS> Intersection found at: X:{intersectionPoint.X} Y:{intersectionPoint.Y}");

                var pko = new PromptKeywordOptions("\n3DS> Accept point position? ") { AppendKeywordsToMessage = true, AllowNone = true };
                pko.Keywords.Add(Keywords.Accept);
                pko.Keywords.Add(Keywords.Cancel);
                pko.Keywords.Default = Keywords.Accept;

                var cancelled = false;
                PromptResult prResult;
                TransientGraphics graphics = new TransientGraphics(TransientDrawingMode.Main);
                do
                {
                    prResult = AutoCADActive.Editor.GetKeywords(pko);

                    try
                    {
                        if (prResult.Status != PromptStatus.Keyword &&
                            prResult.Status != PromptStatus.OK)
                            continue;




                    }
                    catch (Exception e)
                    {
                        AutoCADActive.Editor.WriteMessage(e.Message);
                    }
                    finally
                    {
                        graphics.Dispose();
                    }


                } while (prResult.Status != PromptStatus.Cancel &&
                         prResult.Status != PromptStatus.Error && !cancelled);




                CogoPoints.CreateCogoPoint(intersectionPoint.ToPoint3d());

                tr.Commit();
            }
        }
    }
}