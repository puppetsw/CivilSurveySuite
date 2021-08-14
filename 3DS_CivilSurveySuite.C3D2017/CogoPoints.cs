// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;
using Point = _3DS_CivilSurveySuite.Model.Point;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.C3D2017.CogoPoints))]
namespace _3DS_CivilSurveySuite.C3D2017
{
    public class CogoPoints
    {
        private const int GraphicPixelSize = 6;

        [CommandMethod("3DS", "_3DSCPProduction", CommandFlags.Modal)]
        public void Create_At_Production_Of_Line_And_Distance()
        {
            var graphics = new TransientGraphics();
            using (Transaction tr = AcadApp.StartTransaction())
            {
                try
                {
                    Line line = LineUtils.GetNearestPointOfLineOrPolylineSegment(tr, out Point3d basePoint);

                    if (line == null)
                        return;

                    graphics.DrawLine(line);

                    Angle angle = LineUtils.GetAngleOfLine(line);

                    // If the basePoint is equal to the lines StartPoint, we want the angle to go in the
                    // opposite direction. So we Flip().
                    if (basePoint == line.StartPoint)
                        angle = angle.Flip();

                    if (!EditorUtils.GetDistance(out double dist, "\n" + ResourceStrings.Offset_Distance, basePoint))
                        return;

                    var pko = new PromptKeywordOptions("\n" + ResourceStrings.Accept_Position) { AppendKeywordsToMessage = true, AllowNone = true };
                    pko.Keywords.Add(Keywords.Accept);
                    pko.Keywords.Add(Keywords.Cancel);
                    pko.Keywords.Add(Keywords.Flip);
                    pko.Keywords.Default = Keywords.Accept;

                    Point point = PointHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());

                    graphics.ClearGraphics();
                    graphics.DrawPlus(point.ToPoint3d(), GraphicPixelSize);
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
                                point = PointHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());
                                graphics.ClearGraphics();
                                graphics.DrawPlus(point.ToPoint3d(), GraphicPixelSize);
                                graphics.DrawLine(basePoint, point.ToPoint3d());
                                break;
                        }
                    } while (prResult.Status != PromptStatus.Cancel && prResult.Status != PromptStatus.Error && !cancelled);

                    tr.Commit();
                }
                catch (Exception e)
                {
                    AcadApp.Editor.WriteMessage(e.ToString());
                }
                finally
                {
                    graphics.Dispose();
                }
            }
        }

        [CommandMethod("3DS", "_3DSCPOffsetTwoLines", CommandFlags.Modal)]
        public void Create_At_Offset_Two_Lines()
        {
            var graphics = new TransientGraphics();
            try
            {
                using (Transaction tr = AcadApp.StartTransaction())
                {
                    AcadApp.Editor.WriteMessage("\n3DS> Select first line to offset.");
                    Line firstLineToOffset = LineUtils.GetLineOrPolylineSegment(tr);
                    
                    if (firstLineToOffset == null)
                        return;

                    // Highlight line.
                    graphics.DrawLine(firstLineToOffset);

                    AcadApp.Editor.WriteMessage("\n3DS> Select second line to offset.");
                    Line secondLineToOffset = LineUtils.GetLineOrPolylineSegment(tr);

                    if (secondLineToOffset == null)
                        return;

                    // Highlight line.
                    graphics.DrawLine(secondLineToOffset);

                    //TODO: Create default message, so we don't need to pass it in. Unless we want to.
                    // Pick offset side.
                    if (!EditorUtils.GetPoint(out Point3d offsetPoint, "\n" + ResourceStrings.Pick_Offset_Side))
                        return;

                    // Prompt for offset distance.
                    if (!EditorUtils.GetDistance(out double dist, "\n" + ResourceStrings.Offset_Distance))
                        return;

                    Line firstOffsetLine = LineUtils.Offset(firstLineToOffset, dist, offsetPoint);
                    Line secondOffsetLine = LineUtils.Offset(secondLineToOffset, dist, offsetPoint);
                    Point intersectionPoint = LineUtils.FindIntersectionPoint(firstOffsetLine, secondOffsetLine);

                    var pko = new PromptKeywordOptions("\n" + ResourceStrings.Accept_Position) { AppendKeywordsToMessage = true, AllowNone = true };
                    pko.Keywords.Add(Keywords.Accept);
                    pko.Keywords.Add(Keywords.Cancel);
                    pko.Keywords.Default = Keywords.Accept;

                    graphics.ClearGraphics();
                    graphics.DrawPlus(intersectionPoint.ToPoint3d(), GraphicPixelSize);
                    
                    var cancelled = false;
                    do
                    {
                        PromptResult prResult = AcadApp.Editor.GetKeywords(pko);

                        switch (prResult.Status)
                        {
                            case PromptStatus.Cancel:
                            case PromptStatus.None:
                            case PromptStatus.Error:
                                cancelled = true;
                                break;
                            case PromptStatus.OK:
                            case PromptStatus.Keyword:
                                switch (prResult.StringResult)
                                {
                                    case Keywords.Accept:
                                        CogoPointUtils.CreateCogoPoint(intersectionPoint.ToPoint3d());
                                        cancelled = true;
                                        break;
                                    case Keywords.Cancel:
                                        cancelled = true;
                                        break;
                                }
                                break;
                        }

                    } while (!cancelled);
                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                AcadApp.Editor.WriteMessage(e.Message);
            }
            finally
            {
                graphics.Dispose();
            }
        }

        [CommandMethod("3DS", "_3DSCPAngleAndDistance", CommandFlags.Modal)]
        public void Create_At_Angle_And_Distance()
        {
            if (!EditorUtils.GetPoint(out Point3d basePoint, "\n3DS> Select a base point: "))
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

            Point point = PointHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());

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
                            point = PointHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());
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
        public void Create_Trunks_At_Trees()
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

        [CommandMethod("3DS", "_3DSRawDescriptionToUpper", CommandFlags.Modal)]
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
                    CogoPointUtils.RawDescriptionToUpperCase(ref pt);
                    pt.DowngradeOpen();
                }

                tr.Commit();
            }
        }

        [CommandMethod("3DS", "_3DSFullDescriptionToUpper", CommandFlags.Modal)]
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
                    CogoPointUtils.FullDescriptionToUpperCase(ref pt);
                    pt.DowngradeOpen();
                }

                tr.Commit();
            }
        }

        [CommandMethod("3DS", "_3DSCPLabelRotationMatch", CommandFlags.Modal)]
        public void Label_Rotate_Match()
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

        public void Label_Rotate_Match_All()
        {
            // select a bunch of cogopoints and rotate them based on their position to the selected lines/polylines vertexs

        }
    }
}