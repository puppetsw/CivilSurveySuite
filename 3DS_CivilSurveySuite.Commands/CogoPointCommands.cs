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
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Point = _3DS_CivilSurveySuite.Model.Point;

// ReSharper disable UnusedMember.Global

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Commands.CogoPointCommands))]
namespace _3DS_CivilSurveySuite.Commands
{
    public class CogoPointCommands
    {
        [CommandMethod("3DS", "_3DSCogoPointLabelRotate", CommandFlags.Modal)]
        public void CogoPointLabelRotate()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;

            var perLines = EditorUtils.GetEntity(new[] { typeof(Line), typeof(Polyline), typeof(Polyline2d), typeof(Polyline3d) }, "\n3DS> Select line or polyline.");

            if (perLines.Status != PromptStatus.OK)
                return;
            
            string lineType = perLines.ObjectId.ObjectClass.DxfName;
            using (Transaction tr = AutoCADActive.StartTransaction())
            {
                double angle = 0;

                switch (lineType)
                {
                    case DxfNames.LWPOLYLINE:
                    case DxfNames.POLYLINE:
                        var poly = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline;
                        if (poly != null)
                        {
                            angle = Polylines.GetPolylineSegmentAngle(poly, perLines.PickedPoint);
                            break;
                        }

                        var poly3d = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline3d;
                        if (poly3d != null)
                        {
                            angle = Polylines.GetPolyline3dSegmentAngle(poly3d, perLines.PickedPoint);
                        }

                        break;
                    case DxfNames.LINE:
                        var line = (Line) perLines.ObjectId.GetObject(OpenMode.ForRead);
                        angle = line.Angle;
                        break;
                }

                AutoCADActive.Editor.WriteMessage("Polyline segment angle (radians): " + angle);

                foreach (ObjectId id in pso.Value.GetObjectIds())
                {
                    var pt = (CogoPoint) id.GetObject(OpenMode.ForRead);
                    var style = pt.LabelStyleId.GetObject(OpenMode.ForRead) as LabelStyle;
                    double textAngle = Labels.GetLabelStyleComponentAngle(style);

                    AutoCADActive.Editor.WriteMessage($"Point label style current rotation (radians): {textAngle}");
                    AutoCADActive.Editor.WriteMessage($"Rotating label to {angle} to match polyline segment");

                    pt.UpgradeOpen();
                    pt.LabelRotation = 0;
                    pt.ResetLabelLocation();
                    pt.LabelRotation -= textAngle;
                    pt.LabelRotation += angle;
                    pt.DowngradeOpen();
                }
                tr.Commit();
            }
        }

        [CommandMethod("3DS", "_3DSCogoPointRawDescriptionToUpperCase", CommandFlags.Modal)]
        public void CogoPointRawDescriptionToUpper()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;
            
            using (Transaction tr = AutoCADActive.StartTransaction())
            {
                foreach (ObjectId objectId in pso.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    CogoPoints.RawDescriptionToUpperCase(ref pt);
                    pt.DowngradeOpen(); // Don't leave point in write mode?
                }

                tr.Commit();
            }
        }

        [CommandMethod("3DS", "_3DSCogoPointFullDescriptionToUpperCase", CommandFlags.Modal)]
        public void CogoPointFullDescriptionToUpper()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;
            
            using (Transaction tr = AutoCADActive.StartTransaction())
            {
                foreach (ObjectId objectId in pso.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    CogoPoints.FullDescriptionToUpperCase(ref pt);
                    pt.DowngradeOpen(); // Don't leave point in write mode?
                }

                tr.Commit();
            }
        }

        [CommandMethod("3DS", "_3DSCreateTrunkPointAtTrees", CommandFlags.Modal)]
        public void CogoPointCreateTrunksAtTrees()
        {
            //TODO: Use settings to determine codes for TRNK and TRE
            //TODO: Add option to set style for tree and trunk?
            var counter = 0;

            using (Transaction tr = AutoCADActive.StartTransaction())
            {
                foreach (ObjectId pointId in CivilActive.ActiveCivilDocument.CogoPoints)
                {
                    var cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint is null) 
                        continue;

                    if (!cogoPoint.RawDescription.Contains("TRE ")) 
                        continue;
                    
                    ObjectId trunkPointId = CivilActive.ActiveCivilDocument.CogoPoints.Add(cogoPoint.Location, true);
                    CogoPoint trunkPoint = trunkPointId.GetObject(OpenMode.ForWrite) as CogoPoint;

                    if (trunkPoint != null)
                    {
                        trunkPoint.RawDescription = cogoPoint.RawDescription.Replace("TRE ", "TRNK ");
                        trunkPoint.ApplyDescriptionKeys();

                        cogoPoint.UpgradeOpen();
                        cogoPoint.RawDescription = cogoPoint.RawDescription.Replace("TRE ", "TREE ");
                        cogoPoint.ApplyDescriptionKeys();
                    }
                    counter++;
                }
                tr.Commit();
            }

            string completeMessage = "Changed " + counter + " TRE points, and created " + counter + " TRNK points";
            AutoCADActive.Editor.WriteMessage(completeMessage);

        }

        [CommandMethod("3DS", "_3DSCreateCogoPointBearingAndDistance", CommandFlags.Modal)]
        public void CogoPoint_Create_At_Selected_Bearing_And_Distance()
        {
            if (!EditorUtils.GetBasePoint3d(out Point3d basePoint, "\n3DS> Select a base point: "))
                return;

            if (!EditorUtils.GetAngle(out Angle angle, "\n3DS> Get angle: ", basePoint))
                return;

            if (!EditorUtils.GetDistance(out double dist, "\n3DS> Offset distance: ", basePoint))
                return;

            AutoCADActive.Editor.WriteMessage($"\n3DS> Angle: {angle}");
            AutoCADActive.Editor.WriteMessage($"\n3DS> Distance: {dist}");

            var pko = new PromptKeywordOptions("\n3DS> Flip angle? ") { AppendKeywordsToMessage = true };
            pko.Keywords.Add(Keywords.Accept);
            pko.Keywords.Add(Keywords.Cancel);
            pko.Keywords.Add(Keywords.Flip);

            Point point = MathHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());

            using (var graphics = new TransientGraphics())
            {
                graphics.DrawTransientPoint(point.ToPoint3d());
                graphics.DrawTransientLine(basePoint, point.ToPoint3d());

                var cancelled = false;
                PromptResult prResult;
                do
                {
                    prResult = AutoCADActive.Editor.GetKeywords(pko);

                    if (prResult.Status != PromptStatus.Keyword && 
                        prResult.Status != PromptStatus.OK)
                        continue;

                    switch (prResult.StringResult)
                    {
                        case Keywords.Accept:
                            CogoPoints.CreateCogoPoint(point.ToPoint3d());
                            cancelled = true;
                            break;
                        case Keywords.Cancel:
                            cancelled = true;
                            break;
                        case Keywords.Flip:
                            angle = angle.Flip();
                            point = MathHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());
                            graphics.ClearTransientGraphics();
                            graphics.DrawTransientPoint(point.ToPoint3d());
                            graphics.DrawTransientLine(basePoint, point.ToPoint3d());
                            break;
                    }
                } while (prResult.Status != PromptStatus.Cancel && 
                         prResult.Status != PromptStatus.Error && !cancelled);
            }
        }

        [CommandMethod("3DS", "_3DSCreateCogoPointOffsetTwoLines", CommandFlags.Modal)]
        public void CogoPoint_Create_At_Offset_Two_Lines()
        {
            // Select first line
            if (!EditorUtils.GetEntity(out ObjectId firstLineId, out Point3d firstPickedPoint, new[] { typeof(Line), typeof(Polyline) }, "\n3DS> Select first Line or Polyline :"))
                return;
            
            // Select second line
            if (!EditorUtils.GetEntity(out ObjectId secondLineId, out Point3d secondPickedPoint, new[] { typeof(Line), typeof(Polyline) }, "\n3DS> Select second Line or Polyline :"))
                return;

            // Prompt for offset distance
            if (!EditorUtils.GetDistance(out double dist, "\n3DS> Offset distance: "))
                return;

            // Pick offset side
            if (!EditorUtils.GetBasePoint3d(out Point3d offsetPoint, "\n3DS> Select offset side: "))
                return;

            using (Transaction tr = AutoCADActive.StartTransaction())
            {
                Line firstOffsetLine = null;
                Line secondOffsetLine = null;
                switch (firstLineId.ObjectClass.DxfName)
                {
                    case DxfNames.LINE:
                        Line line = firstLineId.GetObject(OpenMode.ForRead) as Line;
                        firstOffsetLine = Lines.Offset(line, dist, offsetPoint);
                        break;
                    case DxfNames.LWPOLYLINE:
                    case DxfNames.POLYLINE:
                        break;
                }

                switch (secondLineId.ObjectClass.DxfName)
                {
                    case DxfNames.LINE:
                        Line line = secondLineId.GetObject(OpenMode.ForRead) as Line;
                        secondOffsetLine = Lines.Offset(line, dist, offsetPoint);
                        break;
                    case DxfNames.LWPOLYLINE:
                    case DxfNames.POLYLINE:
                        break;
                }

                var p1 = new Vector(firstOffsetLine.StartPoint.X, firstOffsetLine.StartPoint.Y);
                var p2 = new Vector(firstOffsetLine.EndPoint.X, firstOffsetLine.EndPoint.Y);
                var q1 = new Vector(secondOffsetLine.StartPoint.X, secondOffsetLine.StartPoint.Y);
                var q2 = new Vector(secondOffsetLine.EndPoint.X, secondOffsetLine.EndPoint.Y);

                MathHelpers.LineSegementsIntersect(p1, p2, q1, q2, out Model.Point intersectionPoint);
                AutoCADActive.Editor.WriteMessage($"Intersection found at: X:{intersectionPoint.X} Y:{intersectionPoint.Y}");
            }
        }

        [CommandMethod("3DSTestMethod")]
        public void TestMethod()
        {
            var result = AutoCADActive.Editor.GetNestedEntity("\n3DS> Select Polyline segment: ");

            if (result.Status != PromptStatus.OK)
                return;

            using (Transaction tr = AutoCADActive.StartTransaction())
            {
                Polyline polyline = result.ObjectId.GetObject(OpenMode.ForRead) as Polyline;

                var seg = Polylines.GetPolylineSegment(polyline, result);

                AutoCADActive.Editor.WriteMessage($"Segment index: {seg}");

                tr.Commit();
            }

        }

        



    }
}