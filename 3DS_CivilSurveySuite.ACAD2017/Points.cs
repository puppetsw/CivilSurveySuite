// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.ACAD2017.Points))]
namespace _3DS_CivilSurveySuite.ACAD2017
{
    public class Points
    {
        private const int GraphicPixelSize = 6;

        [CommandMethod("3DS", "_3DSPtProdDist", CommandFlags.Modal)]
        public void AcadPoint_Create_At_Production_Of_Line_And_Distance()
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

                    Angle angle = AngleHelpers.AngleBetweenPoints(line.StartPoint.ToPoint(), line.EndPoint.ToPoint());

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

                    Point point = PointHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());

                    using (var graphics = new TransientGraphics())
                    {
                        graphics.DrawCircle(point.ToPoint3d());
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
                                    PointUtils.CreatePoint(point.ToPoint3d());
                                    cancelled = true;
                                    break;
                                case Keywords.Cancel:
                                    cancelled = true;
                                    break;
                                case Keywords.Flip:
                                    angle = angle.Flip();
                                    point = PointHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());
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
                    AcadApp.Editor.WriteMessage(e.ToString());
                }
            }
        }

        [CommandMethod("3DS", "_3DSPtOffsetLn", CommandFlags.Modal)]
        public void AcadPoint_Create_At_Offset_Two_Lines()
        {
            if (!EditorUtils.GetNestedEntity(out PromptNestedEntityResult firstLineResult, "\n3DS> Select first line or polyline to offset: "))
                return;

            if (!firstLineResult.ObjectId.IsType(new[] { typeof(Polyline), typeof(Line) }))
                return;

            ObjectId firstLineId = firstLineResult.ObjectId;
                
            if (!EditorUtils.GetNestedEntity(out PromptNestedEntityResult secondLineResult, "\n3DS> Select second line or polyline to offset: "))
                return;

            if (!secondLineResult.ObjectId.IsType(new[] { typeof(Polyline), typeof(Line) }))
                return;

            ObjectId secondLineId = secondLineResult.ObjectId;

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

                PointUtils.CreatePoint(intersectionPoint.ToPoint3d());

                tr.Commit();
            }
        }

        [CommandMethod("3DS", "_3DSPtBrgDist", CommandFlags.Modal)]
        public void AcadPoint_Create_At_Angle_And_Distance()
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

            Point point = PointHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());

            using (var graphics = new TransientGraphics())
            {
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
                            PointUtils.CreatePoint(point.ToPoint3d());
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

        /// <summary>
        /// Inverses between points (pick or number), echoes coordinates, 
        /// azimuths, bearings, horz/vert distance and slope.
        /// </summary>
        [CommandMethod("3DS", "_3DSPtInverse", CommandFlags.Modal)]
        public void AcadPoint_Inverse_CommandLine()
        {
            var graphics = new TransientGraphics();
            try
            {
                // Pick first point.
                if (!EditorUtils.GetBasePoint3d(out Point3d firstPoint, "\n3DS> Pick first point: "))
                    return;

                // Highlight first point.
                graphics.DrawX(firstPoint, GraphicPixelSize);

                // Pick second point.
                if (!EditorUtils.GetBasePoint3d(out Point3d secondPoint, "\n3DS> Pick second point: "))
                    return;
            
                var angle = AngleHelpers.AngleBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
                var distance = PointHelpers.DistanceBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
                var delta = MathHelpers.DeltaPoint(firstPoint.ToPoint(), secondPoint.ToPoint());
                var slope = Math.Round(Math.Abs(delta.Z / distance * 100), 3);

                AcadApp.Editor.WriteMessage($"\n3DS> Angle: {angle} ({angle.Flip()})");
                AcadApp.Editor.WriteMessage($"\n3DS> Distance: {distance}");
                AcadApp.Editor.WriteMessage($"\n3DS> dX:{delta.X} dY:{delta.Y} dZ:{delta.Z}");
                AcadApp.Editor.WriteMessage($"\n3DS> Slope:{slope}%");
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

        [CommandMethod("3DS", "_3DSPtInverseDisp", CommandFlags.Modal)]
        public void AcadPoint_Inverse_ScreenDisplay()
        {
            var graphics = new TransientGraphics();
            try
            {
                while (true)
                {
                    bool loopPick = EditorUtils.GetBasePoint3d(out Point3d firstPoint, "\n3DS> Pick first point: ");

                    if (!loopPick)
                        break;

                    // Highlight first point.
                    graphics.DrawX(firstPoint, GraphicPixelSize);

                    // Pick second point.
                    if (!EditorUtils.GetBasePoint3d(out Point3d secondPoint, "\n3DS> Pick second point: "))
                        return;
            
                    var angle = AngleHelpers.AngleBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
                    var distance = PointHelpers.DistanceBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
                    var delta = MathHelpers.DeltaPoint(firstPoint.ToPoint(), secondPoint.ToPoint());
                    var slope = Math.Round(Math.Abs(delta.Z / distance * 100), 3);

                    var midPoint = PointHelpers.MidpointBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
                    graphics.ClearGraphics();
                    graphics.DrawX(firstPoint, GraphicPixelSize);
                    graphics.DrawX(secondPoint, GraphicPixelSize);
                    graphics.DrawLine(firstPoint, secondPoint);
                    graphics.DrawText(midPoint.ToPoint3d(), $"bearing: {angle} \\P dist: {distance} \\P dX:{delta.X} dY:{delta.Y} dZ:{delta.Z} \\P Slope:{slope}%", 1.0, angle.GetOrdinaryAngle());
                }
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

        /// <summary>
        /// Places a point at the intersection of two bearings from two base points.
        /// </summary>
        [CommandMethod("3DS", "_3DSPtIntBrg", CommandFlags.Modal)]
        public void AcadPoint_Create_At_Intersection_Two_Bearings()
        {
            if (!EditorUtils.GetBasePoint3d(out Point3d firstPoint, "\n3DS> Pick first point: "))
                return;

            if (!EditorUtils.GetAngle(out Angle firstAngle, "\n3DS> Enter first bearing: ", firstPoint))
                return;

            if (!EditorUtils.GetBasePoint3d(out Point3d secondPoint, "\n3DS> Pick second point: "))
                return;

            if (!EditorUtils.GetAngle(out Angle secondAngle, "\n3DS> Enter second bearing: ", secondPoint))
                return;

            var pt = PointHelpers.AngleAngleIntersection(firstPoint.ToPoint(), firstAngle, secondPoint.ToPoint(), secondAngle);

            PointUtils.CreatePoint(pt.ToPoint3d());
        }

        //intersection
        [CommandMethod("INS")]
        public void InterSectionPoint()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            Line pl1 = null;
            Line pl2 = null;
            Entity ent = null;
            PromptEntityOptions peo = null;
            PromptEntityResult per = null;
            using (Transaction tx = db.TransactionManager.StartTransaction())
            {
//Select first polyline
                peo = new PromptEntityOptions("Select firtst line:");
                per = ed.GetEntity(peo);
                if (per.Status != PromptStatus.OK)
                {
                    return;
                }

//Get the polyline entity
                ent = (Entity)tx.GetObject(per.ObjectId, OpenMode.ForRead);
                if (ent is Line)
                {
                    pl1 = ent as Line;
                }

//Select 2nd polyline
                peo = new PromptEntityOptions("\n Select Second line:");
                per = ed.GetEntity(peo);
                if (per.Status != PromptStatus.OK)
                {
                    return;
                }

                ent = (Entity)tx.GetObject(per.ObjectId, OpenMode.ForRead);
                if (ent is Line)
                {
                    pl2 = ent as Line;
                }

                Point3dCollection pts3D = new Point3dCollection();
//Get the intersection Points between line 1 and line 2
                pl1.IntersectWith(pl2, Intersect.OnBothOperands, pts3D, IntPtr.Zero, IntPtr.Zero);
                foreach (Point3d pt in pts3D)
                {
// ed.WriteMessage("\n intersection point :",pt);
// ed.WriteMessage("Point number: ", pt.X, pt.Y, pt.Z);

                    Application.ShowAlertDialog("\n Intersection Point: " + "\nX = " + pt.X + "\nY = " + pt.Y + "\nZ = " + pt.Z);
                }

                tx.Commit();
            }

        }
    }
}