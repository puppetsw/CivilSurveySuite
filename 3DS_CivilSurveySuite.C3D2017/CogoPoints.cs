// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.ACAD2017.Extensions;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Point = _3DS_CivilSurveySuite.Model.Point;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.C3D2017.CogoPoints))]
namespace _3DS_CivilSurveySuite.C3D2017
{
    public class CogoPoints
    {
        private const int GraphicPixelSize = 6;

        [CommandMethod("3DS", "_3DSCPProduction", CommandFlags.Modal)]
        public void CogoPoint_Create_At_Production_Of_Line_And_Distance()
        {
            using (Transaction tr = AcadApp.StartTransaction())
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
                        graphics.DrawDot(point.ToPoint3d(), 6);
                        graphics.DrawLine(basePoint, point.ToPoint3d());

                        var cancelled = false;
                        PromptResult prResult;
                        do
                        {
                            prResult = AcadApp.Editor.GetKeywords(pko);

                            if (prResult.Status != PromptStatus.Keyword &&
                                prResult.Status != PromptStatus.OK)
                                continue;

                            switch (prResult.StringResult)
                            {
                                case Keywords.None: // If user doesn't enter anything.
                                case Keywords.Accept:
                                    CogoPointUtils.CreateCogoPoint(point.ToPoint3d());
                                    cancelled = true;
                                    break;
                                case Keywords.Cancel:
                                    cancelled = true;
                                    break;
                                case Keywords.Flip:
                                    angle = angle.Flip();
                                    point = MathHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());
                                    graphics.ClearGraphics();
                                    graphics.DrawDot(point.ToPoint3d(), 6);
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
                    AcadApp.Editor.WriteMessage(e.ToString());
                }
            }
        }
        
        [CommandMethod("3DS", "_3DSCPOffsetTwoLines", CommandFlags.Modal)]
        public void CogoPoint_Create_At_Offset_Two_Lines()
        {
            ObjectId firstLineId;
            ObjectId secondLineId;

            if (!EditorUtils.GetNestedEntity(out PromptNestedEntityResult firstLineResult, "\n3DS> Select first line or polyline to offset: "))
                return;

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

            using (Transaction tr = AcadApp.StartTransaction())
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
                    var segmentId = PolylineUtils.GetPolylineSegment(polyline, firstLineResult);
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
                    var segmentId = PolylineUtils.GetPolylineSegment(polyline, secondLineResult);
                    var segment = polyline.GetLineSegment2dAt(segmentId);
                    secondLineToOffset = new Line(segment.StartPoint.ToPoint().ToPoint3d(), segment.EndPoint.ToPoint().ToPoint3d());
                }

                Curve firstOffsetLine = LineUtils.Offset(firstLineToOffset, dist, offsetPoint);
                Curve secondOffsetLine = LineUtils.Offset(secondLineToOffset, dist, offsetPoint);

                if (firstOffsetLine == null || secondOffsetLine == null)
                {
                    AcadApp.Editor.WriteMessage("\n3DS> Please select a line or polyline only.");
                    return;
                }

                var p1 = new Vector(firstOffsetLine.StartPoint.X, firstOffsetLine.StartPoint.Y);
                var p2 = new Vector(firstOffsetLine.EndPoint.X, firstOffsetLine.EndPoint.Y);
                var q1 = new Vector(secondOffsetLine.StartPoint.X, secondOffsetLine.StartPoint.Y);
                var q2 = new Vector(secondOffsetLine.EndPoint.X, secondOffsetLine.EndPoint.Y);

                MathHelpers.LineSegementsIntersect(p1, p2, q1, q2, out Point intersectionPoint);
                AcadApp.Editor.WriteMessage($"\n3DS> Intersection found at: X:{intersectionPoint.X} Y:{intersectionPoint.Y}");

                var pko = new PromptKeywordOptions("\n3DS> Accept point position? ") { AppendKeywordsToMessage = true, AllowNone = true };
                pko.Keywords.Add(Keywords.Accept);
                pko.Keywords.Add(Keywords.Cancel);
                pko.Keywords.Default = Keywords.Accept;

                var cancelled = false;
                PromptResult prResult;
                TransientGraphics graphics = new TransientGraphics(TransientDrawingMode.Main);
                do
                {
                    prResult = AcadApp.Editor.GetKeywords(pko);

                    try
                    {
                        if (prResult.Status != PromptStatus.Keyword &&
                            prResult.Status != PromptStatus.OK)
                            continue;




                    }
                    catch (Exception e)
                    {
                        AcadApp.Editor.WriteMessage(e.Message);
                    }
                    finally
                    {
                        graphics.Dispose();
                    }


                } while (prResult.Status != PromptStatus.Cancel &&
                         prResult.Status != PromptStatus.Error && !cancelled);




                CogoPointUtils.CreateCogoPoint(intersectionPoint.ToPoint3d());

                tr.Commit();
            }
        }

        [CommandMethod("3DS", "_3DSCPAngleAndDistance", CommandFlags.Modal)]
        public void CogoPoint_Create_At_Angle_And_Distance()
        {
            if (!EditorUtils.GetBasePoint3d(out Point3d basePoint, "\n3DS> Select a base point: "))
                return;

            if (!EditorUtils.GetAngle(out Angle angle, "\n3DS> Enter bearing (Format: DDD.MMSS): ", basePoint, "\n3DS> Pick bearing on screen: "))
                return;

            if (!EditorUtils.GetDistance(out double dist, "\n3DS> Distance: ", basePoint))
                return;

            AcadApp.Editor.WriteMessage($"\n3DS> Bearing: {angle}");
            AcadApp.Editor.WriteMessage($"\n3DS> Distance: {dist}");

            var pko = new PromptKeywordOptions("\n3DS> Flip bearing? ") { AppendKeywordsToMessage = true };
            pko.Keywords.Add(Keywords.Accept);
            pko.Keywords.Add(Keywords.Cancel);
            pko.Keywords.Add(Keywords.Flip);

            Point point = MathHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());

            using (var graphics = new TransientGraphics())
            {
                graphics.ClearGraphics();
                graphics.DrawPlus(basePoint, GraphicPixelSize);
                graphics.DrawDot(point.ToPoint3d(), GraphicPixelSize);
                graphics.DrawLine(basePoint, point.ToPoint3d());

                var cancelled = false;
                PromptResult prResult;
                do
                {
                    prResult = AcadApp.Editor.GetKeywords(pko);

                    if (prResult.Status != PromptStatus.Keyword && 
                        prResult.Status != PromptStatus.OK)
                        continue;

                    switch (prResult.StringResult)
                    {
                        case Keywords.Accept:
                            CogoPointUtils.CreateCogoPoint(point.ToPoint3d());
                            cancelled = true;
                            break;
                        case Keywords.Cancel:
                            cancelled = true;
                            break;
                        case Keywords.Flip:
                            angle = angle.Flip();
                            point = MathHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());
                            graphics.ClearGraphics();
                            graphics.DrawPlus(basePoint, GraphicPixelSize);
                            graphics.DrawDot(point.ToPoint3d(), GraphicPixelSize);
                            graphics.DrawLine(basePoint, point.ToPoint3d());
                            break;
                    }
                } while (prResult.Status != PromptStatus.Cancel && 
                         prResult.Status != PromptStatus.Error && !cancelled);
            }
        }

        [CommandMethod("3DS", "_3DSCPTrunkPointAtTrees", CommandFlags.Modal)]
        public void CogoPoint_CreateTrunksAtTrees()
        {
            //TODO: Use settings to determine codes for TRNK and TRE
            //TODO: Add option to set style for tree and trunk?
            var counter = 0;

            using (Transaction tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId pointId in C3DApp.ActiveCivilDocument.CogoPoints)
                {
                    var cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint is null) 
                        continue;

                    if (!cogoPoint.RawDescription.Contains("TRE ")) 
                        continue;
                    
                    ObjectId trunkPointId = C3DApp.ActiveCivilDocument.CogoPoints.Add(cogoPoint.Location, true);
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
            AcadApp.Editor.WriteMessage(completeMessage);
        }

        [CommandMethod("3DS", "_3DSCPRawDescriptionToUpper", CommandFlags.Modal)]
        public void RawDescription_ToUpper()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;
            
            using (Transaction tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in pso.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    CogoPointUtils.FullDescriptionToUpperCase(ref pt);
                    pt.DowngradeOpen();
                }

                tr.Commit();
            }
        }

        [CommandMethod("3DS", "_3DSCPFullDescriptionToUpper", CommandFlags.Modal)]
        public void FullDescription_ToUpper()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;
            
            using (Transaction tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in pso.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    CogoPointUtils.RawDescriptionToUpperCase(ref pt);
                    pt.DowngradeOpen();
                }

                tr.Commit();
            }
        }

        [CommandMethod("3DS", "_3DSCPLabelRotate", CommandFlags.Modal)]
        public void CogoPoint_Label_Rotate()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;

            var perLines = EditorUtils.GetEntity(new[] { typeof(Line), typeof(Polyline), typeof(Polyline2d), typeof(Polyline3d) }, "\n3DS> Select line or polyline.");

            if (perLines.Status != PromptStatus.OK)
                return;
            
            string lineType = perLines.ObjectId.ObjectClass.DxfName;
            using (Transaction tr = AcadApp.StartTransaction())
            {
                double angle = 0;

                switch (lineType)
                {
                    case DxfNames.LWPOLYLINE:
                    case DxfNames.POLYLINE:
                        var poly = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline;
                        if (poly != null)
                        {
                            angle = PolylineUtils.GetPolylineSegmentAngle(poly, perLines.PickedPoint);
                            break;
                        }

                        var poly3d = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline3d;
                        if (poly3d != null)
                        {
                            angle = PolylineUtils.GetPolyline3dSegmentAngle(poly3d, perLines.PickedPoint);
                        }

                        break;
                    case DxfNames.LINE:
                        var line = (Line) perLines.ObjectId.GetObject(OpenMode.ForRead);
                        angle = line.Angle;
                        break;
                }

                AcadApp.Editor.WriteMessage("Polyline segment angle (radians): " + angle);

                foreach (ObjectId id in pso.Value.GetObjectIds())
                {
                    var pt = (CogoPoint) id.GetObject(OpenMode.ForRead);
                    var style = pt.LabelStyleId.GetObject(OpenMode.ForRead) as LabelStyle;
                    double textAngle = LabelUtils.GetLabelStyleComponentAngle(style);

                    AcadApp.Editor.WriteMessage($"Point label style current rotation (radians): {textAngle}");
                    AcadApp.Editor.WriteMessage($"Rotating label to {angle} to match polyline segment");

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
    }
}