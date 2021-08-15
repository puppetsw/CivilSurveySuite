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
using Autodesk.AutoCAD.GraphicsInterface;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class PointUtils
    {
        private const int GraphicPixelSize = 6;

        /// <summary>
        /// Creates a <see cref="DBPoint"/> from an angle and distance.
        /// </summary>
        public static void Create_At_Angle_And_Distance(Action<Transaction, Point3d> createPointAction)
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
                graphics.DrawPlus(basePoint, GraphicPixelSize);
                graphics.DrawX(point.ToPoint3d(), GraphicPixelSize);
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
                            using (var tr = AcadApp.StartTransaction())
                            {
                                createPointAction(tr, point.ToPoint3d());
                                tr.Commit();
                            }

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
                            graphics.DrawX(point.ToPoint3d(), GraphicPixelSize);
                            graphics.DrawLine(basePoint, point.ToPoint3d());
                            break;
                    }
                } while (prResult.Status != PromptStatus.Cancel &&
                         prResult.Status != PromptStatus.Error && !cancelled);
            }
        }

        /// <summary>
        /// Creates a <see cref="DBPoint"/> at the intersection of two bearings from two base points.
        /// </summary>
        public static void Create_At_Intersection_Two_Bearings(Action<Transaction, Point3d> createPointAction)
        {
            var graphics = new TransientGraphics();
            try
            {
                if (!EditorUtils.GetPoint(out Point3d firstPoint, "\n3DS> Pick first point: "))
                    return;

                graphics.DrawPlus(firstPoint, GraphicPixelSize);

                if (!EditorUtils.GetAngle(out Angle firstAngle, "\n3DS> Enter first bearing: ", firstPoint))
                    return;

                var endPoint1 = PointHelpers.AngleAndDistanceToPoint(firstAngle, 1000, firstPoint.ToPoint());
                graphics.DrawLine(firstPoint, endPoint1.ToPoint3d());

                if (!EditorUtils.GetPoint(out Point3d secondPoint, "\n3DS> Pick second point: "))
                    return;

                graphics.DrawPlus(secondPoint, GraphicPixelSize);

                if (!EditorUtils.GetAngle(out Angle secondAngle, "\n3DS> Enter second bearing: ", secondPoint))
                    return;

                var endPoint2 = PointHelpers.AngleAndDistanceToPoint(secondAngle, 1000, secondPoint.ToPoint());
                graphics.DrawLine(secondPoint, endPoint2.ToPoint3d());

                var canIntersect = PointHelpers.AngleAngleIntersection(firstPoint.ToPoint(), firstAngle, secondPoint.ToPoint(), secondAngle, out Point intersectionPoint);

                if (!canIntersect)
                {
                    AcadApp.Editor.WriteMessage("\n3DS> No intersection found! ");
                    return;
                }

                AcadApp.Editor.WriteMessage($"\n3DS> Intersection found at X:{Math.Round(intersectionPoint.X, 4)} Y:{Math.Round(intersectionPoint.Y, 4)}");

                graphics.DrawX(intersectionPoint.ToPoint3d(), GraphicPixelSize);

                using (var tr = AcadApp.StartTransaction())
                {
                    //CreatePoint(tr, intersectionPoint.ToPoint3d());
                    createPointAction(tr, intersectionPoint.ToPoint3d());
                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                AcadApp.Editor.WriteMessage($"3DS> Command Exception: {e.Message}");
            }
            finally
            {
                graphics.Dispose();
            }
        }

        /// <summary>
        /// Creates a <see cref="DBPoint"/> at intersection of two distances.
        /// </summary>
        public static void Create_At_Intersection_Two_Distances(Action<Transaction, Point3d> createPointAction)
        {
            var graphics = new TransientGraphics();
            try
            {
                if (!EditorUtils.GetPoint(out Point3d firstPoint, "\n3DS> Pick first point: "))
                    return;

                graphics.DrawPlus(firstPoint, GraphicPixelSize);

                if (!EditorUtils.GetDistance(out double dist1, "\n3DS> Enter first distance: ", firstPoint))
                    return;

                graphics.DrawCircle(firstPoint, dist1);

                if (!EditorUtils.GetPoint(out Point3d secondPoint, "\n3DS> Pick second point: "))
                    return;

                graphics.DrawPlus(secondPoint, GraphicPixelSize);

                if (!EditorUtils.GetDistance(out double dist2, "\n3DS> Enter second distance: ", secondPoint))
                    return;

                graphics.DrawCircle(secondPoint, dist2);

                var canIntersect = PointHelpers.DistanceDistanceIntersection(firstPoint.ToPoint(), dist1, secondPoint.ToPoint(), dist2, out Point firstInt, out Point secondInt);

                if (!canIntersect)
                {
                    AcadApp.Editor.WriteMessage("\n3DS> No intersection found! ");
                    return;
                }

                graphics.DrawDot(firstInt.ToPoint3d(), GraphicPixelSize/2);
                graphics.DrawDot(secondInt.ToPoint3d(), GraphicPixelSize/2);
                AcadApp.Editor.WriteMessage($"\n3DS> First intersection found at X:{Math.Round(firstInt.X, 4)} Y:{Math.Round(firstInt.Y, 4)}");
                AcadApp.Editor.WriteMessage($"\n3DS> Second intersection found at X:{Math.Round(secondInt.X, 4)} Y:{Math.Round(secondInt.Y, 4)}");

                if (!EditorUtils.GetPoint(out Point3d pickedPoint, "\n3DS> Pick near desired intersection: "))
                    return;

                using (var tr = AcadApp.StartTransaction())
                {
                    graphics.ClearGraphics();
                    if (PointHelpers.DistanceBetweenPoints(pickedPoint.ToPoint(), firstInt) <= PointHelpers.DistanceBetweenPoints(pickedPoint.ToPoint(), secondInt))
                    {
                        //use first point
                        //CreatePoint(tr, firstInt.ToPoint3d());
                        graphics.DrawDot(firstInt.ToPoint3d(), GraphicPixelSize/2);
                        createPointAction(tr, firstInt.ToPoint3d());
                    }
                    else
                    {
                        //use second point
                        graphics.DrawDot(secondInt.ToPoint3d(), GraphicPixelSize/2);
                        createPointAction(tr, secondInt.ToPoint3d());
                    }

                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                AcadApp.Editor.WriteMessage($"3DS> Command Exception: {e.Message}");
            }
            finally
            {
                graphics.Dispose();
            }
        }

        /// <summary>
        /// Creates a <see cref="DBPoint"/> at the offset two lines with given distance.
        /// </summary>
        public static void Create_At_Offset_Two_Lines(Action<Transaction, Point3d> createPointAction)
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
                    graphics.DrawLine(firstLineToOffset, TransientDrawingMode.Highlight);

                    AcadApp.Editor.WriteMessage("\n3DS> Select second line to offset.");
                    Line secondLineToOffset = LineUtils.GetLineOrPolylineSegment(tr);

                    if (secondLineToOffset == null)
                        return;

                    // Highlight line.
                    graphics.DrawLine(secondLineToOffset, TransientDrawingMode.Highlight);

                    // Prompt for offset distance.
                    if (!EditorUtils.GetDistance(out double dist, "\n" + ResourceStrings.Offset_Distance))
                        return;

                    // Pick offset side.
                    if (!EditorUtils.GetPoint(out Point3d offsetPoint, "\n" + ResourceStrings.Pick_Offset_Side))
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
                                        createPointAction(tr, intersectionPoint.ToPoint3d());
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

        /// <summary>
        /// Creates a <see cref="DBPoint"/> at the production of a line and distance.
        /// </summary>
        public static void Create_At_Production_Of_Line_And_Distance(Action<Transaction, Point3d> createPointAction)
        {
            var graphics = new TransientGraphics();
            using (Transaction tr = AcadApp.StartTransaction())
            {
                try
                {
                    Line line = LineUtils.GetNearestPointOfLineOrPolylineSegment(tr, out Point3d basePoint);

                    if (line == null)
                        return;

                    graphics.DrawLine(line, TransientDrawingMode.Highlight);
                    graphics.DrawPlus(basePoint, GraphicPixelSize);

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
                    graphics.DrawPlus(basePoint, GraphicPixelSize);
                    graphics.DrawX(point.ToPoint3d(), GraphicPixelSize);
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
                                createPointAction(tr, point.ToPoint3d());
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
                                graphics.DrawX(point.ToPoint3d(), GraphicPixelSize);
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

        /// <summary>
        /// Inverses between points (pick), echoes coordinates, 
        /// azimuths, bearings, horz/vert distance and slope.
        /// </summary>
        public static void Inverse()
        {
            var graphics = new TransientGraphics();
            try
            {
                // Pick first point.
                if (!EditorUtils.GetPoint(out Point3d firstPoint, "\n3DS> Pick first point: "))
                    return;

                // Highlight first point.
                graphics.DrawX(firstPoint, GraphicPixelSize);

                // Pick second point.
                if (!EditorUtils.GetPoint(out Point3d secondPoint, "\n3DS> Pick second point: "))
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

        /// <summary>
        /// Does the same as <see cref="Inverse"/> but displays the information on the screen.
        /// </summary>
        public static void Inverse_ScreenDisplay()
        {
            var graphics = new TransientGraphics();
            try
            {
                while (true)
                {
                    bool loopPick = EditorUtils.GetPoint(out Point3d firstPoint, "\n3DS> Pick first point: ");

                    if (!loopPick)
                        break;

                    // Highlight first point.
                    graphics.DrawX(firstPoint, GraphicPixelSize);

                    // Pick second point.
                    if (!EditorUtils.GetPoint(out Point3d secondPoint, "\n3DS> Pick second point: "))
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
        /// Creates a <see cref="DBPoint"/> at the specified location.
        /// </summary>
        /// <param name="tr">The existing transaction.</param>
        /// <param name="position">The position to create the point at.</param>
        /// <remarks>Don't forget to commit the transaction after using.</remarks>
        public static void CreatePoint(Transaction tr, Point3d position)
        {
            // Open the Block table for read
            var bt = tr.GetObject(AcadApp.ActiveDocument.Database.BlockTableId, OpenMode.ForRead) as BlockTable;

            if (bt == null)
                return;

            // Open the Block table record Model space for write
            var btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

            if (btr == null)
                return;

            DBPoint acPoint = new DBPoint(position);
            acPoint.SetDatabaseDefaults();

            // Add the new object to the block table record and the transaction
            btr.AppendEntity(acPoint);
            tr.AddNewlyCreatedDBObject(acPoint, true);

            // Save the new object to the database
            // Don't commit, leave it up to the calling method.
            // tr.Commit();
        }

    }
}