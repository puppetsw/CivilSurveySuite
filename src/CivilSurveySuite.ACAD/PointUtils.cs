// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using CivilSurveySuite.Shared.Helpers;
using CivilSurveySuite.Shared.Models;
using CivilSurveySuite.UI.Helpers;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace CivilSurveySuite.ACAD
{
    public static class PointUtils
    {
        /// <summary>
        /// Creates a <see cref="DBPoint"/> from an angle and distance.
        /// </summary>
        public static void Create_At_Angle_And_Distance(Action<Transaction, Point3d> createAction)
        {
            if (!EditorUtils.TryGetPoint("\n3DS> Select a base point: ", out Point3d basePoint))
                return;

            if (!EditorUtils.TryGetAngle("\n3DS> Enter bearing (Format: DDD.MMSS): ", basePoint, out var angle))
                return;

            if (!EditorUtils.TryGetDistance("\n3DS> Distance: ", basePoint, out var dist))
                return;

            if (dist == null)
                return;

            AcadApp.Editor.WriteMessage($"\n3DS> Bearing: {angle}");
            AcadApp.Editor.WriteMessage($"\n3DS> Distance: {dist}");

            var pko = new PromptKeywordOptions("\n3DS> Flip bearing? ") { AppendKeywordsToMessage = true };
            pko.Keywords.Add(Keywords.ACCEPT);
            pko.Keywords.Add(Keywords.CANCEL);
            pko.Keywords.Add(Keywords.FLIP);

            Point point = PointHelpers.AngleAndDistanceToPoint(angle, dist.Value, basePoint.ToPoint());

            using (var graphics = new TransientGraphics())
            {
                graphics.DrawPlus(basePoint, Settings.GraphicsSize);
                graphics.DrawX(point.ToPoint3d(), Settings.GraphicsSize);
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
                        case Keywords.ACCEPT:
                            using (var tr = AcadApp.StartTransaction())
                            {
                                createAction(tr, point.ToPoint3d());
                                tr.Commit();
                            }

                            cancelled = true;
                            break;
                        case Keywords.CANCEL:
                            cancelled = true;
                            break;
                        case Keywords.FLIP:
                            angle = angle.Flip();
                            point = PointHelpers.AngleAndDistanceToPoint(angle, dist.Value, basePoint.ToPoint());
                            graphics.ClearGraphics();
                            graphics.DrawPlus(basePoint, Settings.GraphicsSize);
                            graphics.DrawX(point.ToPoint3d(), Settings.GraphicsSize);
                            graphics.DrawLine(basePoint, point.ToPoint3d());
                            break;
                    }
                } while (prResult.Status != PromptStatus.Cancel &&
                         prResult.Status != PromptStatus.Error && !cancelled);
            }
        }

        /// <summary>
        /// Creates at angle distance and slope.
        /// </summary>
        public static void Create_At_Angle_Distance_And_Slope(Action<Transaction, Point3d> createAction)
        {
            var graphics = new TransientGraphics();
            try
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Pick starting point: ", out Point3d basePoint))
                    return;

                graphics.DrawX(basePoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetAngle("\n3DS> Enter angle: ", basePoint, out var angle))
                    return;

                if (!EditorUtils.TryGetDistance("\n3DS> Enter distance: ", basePoint, out var distance))
                    return;

                if (distance == null)
                    return;

                var point = PointHelpers.AngleAndDistanceToPoint(angle, distance.Value, basePoint.ToPoint());
                graphics.DrawX(point.ToPoint3d(), Settings.GraphicsSize);
                graphics.DrawLine(basePoint, point.ToPoint3d());

                if (!EditorUtils.TryGetDouble("\n3DS> Enter slope (%): ", out var slope))
                    return;

                if (slope == null)
                    return;

                var newPoint = new Point(point.X, point.Y, basePoint.Z + distance.Value * (slope.Value / 100.0));

                using (var tr = AcadApp.StartTransaction())
                {
                    createAction(tr, newPoint.ToPoint3d());
                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                AcadApp.WriteErrorMessage(e);
            }
            finally
            {
                graphics.Dispose();
            }
        }

        /// <summary>
        /// Create a point at a picked location with elevation calculated at designated slope.
        /// </summary>
        public static void Create_At_Slope_At_Point(Action<Transaction, Point3d> createAction)
        {
            using (var graphics = new TransientGraphics())
            using (var tr = AcadApp.StartTransaction())
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Base point: ", out Point3d firstPoint))
                    return;

                graphics.DrawPlus(firstPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetPoint("\n3DS> New point location: ", out Point3d secondPoint))
                    return;

                graphics.DrawPlus(secondPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetDouble("\n3DS> Percent slope: ", out var slope, allowZero: false))
                    return;

                if (slope == null)
                    return;

                double distance = PointHelpers.GetDistanceBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
                double elevation = firstPoint.Z + distance * (slope.Value / 100.0);
                var newPoint = new Point3d(secondPoint.X, secondPoint.Y, elevation);

                createAction(tr, newPoint);

                tr.Commit();
            }
        }

        /// <summary>
        /// Places a point at the intersection of two bearings defined by four points.
        /// </summary>
        public static void Create_At_Intersection_Of_Four_Points(Action<Transaction, Point3d> createAction)
        {
            var graphics = new TransientGraphics();
            try
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Pick the first point: ", out Point3d firstPoint))
                    return;

                graphics.DrawPlus(firstPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetPoint("\n3DS> Pick the second point: ", out Point3d secondPoint))
                    return;

                graphics.DrawPlus(secondPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetPoint("\n3DS> Pick the third point: ", out Point3d thirdPoint))
                    return;

                graphics.DrawPlus(thirdPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetPoint("\n3DS> Pick the fourth point: ", out Point3d fourthPoint))
                    return;

                graphics.DrawPlus(fourthPoint, Settings.GraphicsSize);
                graphics.DrawLine(firstPoint, secondPoint);
                graphics.DrawLine(thirdPoint, fourthPoint);

                var canIntersect = PointHelpers.FourPointIntersection(firstPoint.ToPoint(), secondPoint.ToPoint(), thirdPoint.ToPoint(), fourthPoint.ToPoint(), out Point intersectionPoint);

                if (!canIntersect)
                {
                    AcadApp.WriteMessage("No intersection point found!");
                    return;
                }

                graphics.DrawDot(intersectionPoint.ToPoint3d(), Settings.GraphicsSize);

                using (var tr = AcadApp.StartTransaction())
                {
                    createAction(tr, intersectionPoint.ToPoint3d());
                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                AcadApp.WriteErrorMessage(e);
            }
            finally
            {
                graphics.Dispose();
            }
        }

        /// <summary>
        /// Creates a point at the intersection of two bearings from two base points.
        /// </summary>
        public static void Create_At_Intersection_Two_Bearings(Action<Transaction, Point3d> createAction)
        {
            var graphics = new TransientGraphics();
            try
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Pick first point: ", out Point3d firstPoint))
                    return;

                graphics.DrawPlus(firstPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetAngle("\n3DS> Enter first bearing: ", firstPoint, out var firstAngle))
                    return;

                var endPoint1 = PointHelpers.AngleAndDistanceToPoint(firstAngle, 1000, firstPoint.ToPoint());
                graphics.DrawLine(firstPoint, endPoint1.ToPoint3d());

                if (!EditorUtils.TryGetPoint("\n3DS> Pick second point: ", out Point3d secondPoint))
                    return;

                graphics.DrawPlus(secondPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetAngle("\n3DS> Enter second bearing: ", secondPoint, out var secondAngle))
                    return;

                var endPoint2 = PointHelpers.AngleAndDistanceToPoint(secondAngle, 1000, secondPoint.ToPoint());
                graphics.DrawLine(secondPoint, endPoint2.ToPoint3d());

                bool canIntersect = PointHelpers.AngleAngleIntersection(firstPoint.ToPoint(), firstAngle, secondPoint.ToPoint(), secondAngle, out var intersectionPoint);

                if (!canIntersect)
                {
                    AcadApp.Editor.WriteMessage("\n3DS> No intersection found! ");
                    return;
                }

                AcadApp.Editor.WriteMessage($"\n3DS> Intersection found at X:{Math.Round(intersectionPoint.X, 4)} Y:{Math.Round(intersectionPoint.Y, 4)}");

                graphics.DrawX(intersectionPoint.ToPoint3d(), Settings.GraphicsSize);

                using (var tr = AcadApp.StartTransaction())
                {
                    createAction(tr, intersectionPoint.ToPoint3d());
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
        /// Creates a point at intersection of two distances.
        /// </summary>
        public static void Create_At_Intersection_Two_Distances(Action<Transaction, Point3d> createAction)
        {
            var graphics = new TransientGraphics();
            try
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Pick first point: ", out Point3d firstPoint))
                    return;

                graphics.DrawPlus(firstPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetDistance("\n3DS> Enter first distance: ", firstPoint, out var dist1))
                    return;

                if (dist1 == null)
                    return;

                graphics.DrawCircle(firstPoint, dist1.Value);

                if (!EditorUtils.TryGetPoint("\n3DS> Pick second point: ", out Point3d secondPoint))
                    return;

                graphics.DrawPlus(secondPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetDistance("\n3DS> Enter second distance: ", secondPoint, out var dist2))
                    return;

                if (dist2 == null)
                    return;

                graphics.DrawCircle(secondPoint, dist2.Value);

                var canIntersect = PointHelpers.DistanceDistanceIntersection(firstPoint.ToPoint(), dist1.Value,
                    secondPoint.ToPoint(), dist2.Value, out Point firstInt, out Point secondInt);

                if (!canIntersect)
                {
                    AcadApp.Editor.WriteMessage("\n3DS> No intersection found! ");
                    return;
                }

                graphics.DrawDot(firstInt.ToPoint3d(), Settings.GraphicsSize/2);
                graphics.DrawDot(secondInt.ToPoint3d(), Settings.GraphicsSize/2);
                AcadApp.Editor.WriteMessage($"\n3DS> First intersection found at X:{Math.Round(firstInt.X, 4)} Y:{Math.Round(firstInt.Y, 4)}");
                AcadApp.Editor.WriteMessage($"\n3DS> Second intersection found at X:{Math.Round(secondInt.X, 4)} Y:{Math.Round(secondInt.Y, 4)}");

                if (!EditorUtils.TryGetPoint("\n3DS> Pick near desired intersection: ", out Point3d pickedPoint))
                    return;

                using (var tr = AcadApp.StartTransaction())
                {
                    graphics.ClearGraphics();
                    if (PointHelpers.GetDistanceBetweenPoints(pickedPoint.ToPoint(), firstInt) <= PointHelpers.GetDistanceBetweenPoints(pickedPoint.ToPoint(), secondInt))
                    {
                        //use first point
                        graphics.DrawDot(firstInt.ToPoint3d(), Settings.GraphicsSize/2);
                        createAction(tr, firstInt.ToPoint3d());
                    }
                    else
                    {
                        //use second point
                        graphics.DrawDot(secondInt.ToPoint3d(), Settings.GraphicsSize/2);
                        createAction(tr, secondInt.ToPoint3d());
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
        /// Creates a point at the offset two lines with given distance.
        /// </summary>
        public static void Create_At_Offset_Two_Lines(Action<Transaction, Point3d> createAction)
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
                    if (!EditorUtils.TryGetDistance("\n" + ResourceHelpers.GetLocalisedString("ACAD_OffsetDistance"), out var dist))
                        return;

                    if (dist == null)
                        return;

                    // Pick offset side.
                    if (!EditorUtils.TryGetPoint("\n" + ResourceHelpers.GetLocalisedString("ACAD_PickOffsetSide"), out Point3d offsetPoint))
                        return;

                    Line firstOffsetLine = LineUtils.Offset(firstLineToOffset, dist.Value, offsetPoint);
                    Line secondOffsetLine = LineUtils.Offset(secondLineToOffset, dist.Value, offsetPoint);
                    Point intersectionPoint = LineUtils.FindIntersectionPoint(firstOffsetLine, secondOffsetLine);

                    var pko = new PromptKeywordOptions("\n" + ResourceHelpers.GetLocalisedString("ACAD_AcceptPosition")) { AppendKeywordsToMessage = true, AllowNone = true };
                    pko.Keywords.Add(Keywords.ACCEPT);
                    pko.Keywords.Add(Keywords.CANCEL);
                    pko.Keywords.Default = Keywords.ACCEPT;

                    graphics.ClearGraphics();
                    graphics.DrawPlus(intersectionPoint.ToPoint3d(), Settings.GraphicsSize);

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
                                    case Keywords.ACCEPT:
                                        createAction(tr, intersectionPoint.ToPoint3d());
                                        cancelled = true;
                                        break;
                                    case Keywords.CANCEL:
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
        /// Creates a point at the intersection of a bearing from one point and distance from a second.
        /// </summary>
        public static void Create_At_Intersection_Of_Angle_And_Distance(Action<Transaction, Point3d> createAction)
        {
            var graphics = new TransientGraphics();
            try
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Pick first point: ", out Point3d firstPoint))
                    return;

                graphics.DrawPlus(firstPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetAngle("\n3DS> Enter angle: ", firstPoint, out var angle1))
                    return;

                var constructionPoint = PointHelpers.AngleAndDistanceToPoint(angle1, 32000, firstPoint.ToPoint());
                graphics.DrawLine(firstPoint, constructionPoint.ToPoint3d());

                if (!EditorUtils.TryGetPoint("\n3DS> Pick second point: ", out Point3d secondPoint))
                    return;

                graphics.DrawPlus(secondPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetDistance("\n3DS> Enter distance: ", secondPoint, out var dist))
                    return;

                if (dist == null)
                    return;

                graphics.DrawCircle(secondPoint, dist.Value);

                var canIntersect = PointHelpers.AngleDistanceIntersection(firstPoint.ToPoint(), angle1, secondPoint.ToPoint(), dist.Value, out Point firstInt, out Point secondInt);

                if (!canIntersect)
                {
                    AcadApp.Editor.WriteMessage("\n3DS> No intersection found! ");
                    return;
                }

                graphics.DrawDot(firstInt.ToPoint3d(), Settings.GraphicsSize/2);
                graphics.DrawDot(secondInt.ToPoint3d(), Settings.GraphicsSize/2);
                AcadApp.Editor.WriteMessage($"\n3DS> First intersection found at X:{Math.Round(firstInt.X, 4)} Y:{Math.Round(firstInt.Y, 4)}");
                AcadApp.Editor.WriteMessage($"\n3DS> Second intersection found at X:{Math.Round(secondInt.X, 4)} Y:{Math.Round(secondInt.Y, 4)}");

                if (!EditorUtils.TryGetPoint("\n3DS> Pick near desired intersection: ", out Point3d pickedPoint))
                    return;

                using (var tr = AcadApp.StartTransaction())
                {
                    graphics.ClearGraphics();
                    if (PointHelpers.GetDistanceBetweenPoints(pickedPoint.ToPoint(), firstInt) <= PointHelpers.GetDistanceBetweenPoints(pickedPoint.ToPoint(), secondInt))
                    {
                        //use first point
                        graphics.DrawDot(firstInt.ToPoint3d(), Settings.GraphicsSize/2);
                        createAction(tr, firstInt.ToPoint3d());
                    }
                    else
                    {
                        //use second point
                        graphics.DrawDot(secondInt.ToPoint3d(), Settings.GraphicsSize/2);
                        createAction(tr, secondInt.ToPoint3d());
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
        /// Creates a point at the production of a line and distance.
        /// </summary>
        public static void Create_At_Production_Of_Line_And_Distance(Action<Transaction, Point3d> createAction)
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
                    graphics.DrawPlus(basePoint, Settings.GraphicsSize);

                    Angle angle = LineUtils.GetAngleOfLine(line);

                    // If the basePoint is equal to the lines StartPoint, we want the angle to go in the
                    // opposite direction. So we Flip().
                    if (basePoint == line.StartPoint)
                        angle = angle.Flip();

                    if (!EditorUtils.TryGetDistance("\n" + ResourceHelpers.GetLocalisedString("ACAD_OffsetDistance"), basePoint, out var dist))
                        return;

                    if (dist == null)
                        return;

                    var pko = new PromptKeywordOptions("\n" + ResourceHelpers.GetLocalisedString("ACAD_AcceptPosition")) { AppendKeywordsToMessage = true, AllowNone = true };
                    pko.Keywords.Add(Keywords.ACCEPT);
                    pko.Keywords.Add(Keywords.CANCEL);
                    pko.Keywords.Add(Keywords.FLIP);
                    pko.Keywords.Default = Keywords.ACCEPT;

                    Point point = PointHelpers.AngleAndDistanceToPoint(angle, dist.Value, basePoint.ToPoint());

                    graphics.ClearGraphics();
                    graphics.DrawPlus(basePoint, Settings.GraphicsSize);
                    graphics.DrawX(point.ToPoint3d(), Settings.GraphicsSize);
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
                            case Keywords.NONE: // If user doesn't enter anything.
                            case Keywords.ACCEPT:
                                createAction(tr, point.ToPoint3d());
                                cancelled = true;
                                break;
                            case Keywords.CANCEL:
                                cancelled = true;
                                break;
                            case Keywords.FLIP:
                                angle = angle.Flip();
                                point = PointHelpers.AngleAndDistanceToPoint(angle, dist.Value, basePoint.ToPoint());
                                graphics.ClearGraphics();
                                graphics.DrawPlus(basePoint, Settings.GraphicsSize);
                                graphics.DrawX(point.ToPoint3d(), Settings.GraphicsSize);
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
        /// Creates a point at text location.
        /// </summary>
        public static void Create_At_Label_Location(Action<Transaction, Point3d> createAction, bool useTextAsElevation = false)
        {
            if (!EditorUtils.TryGetSelectionOfType<MText, DBText>("\n3DS> Select text entities: ",
                    "\n3DS> Remove text entities: ", out var selectedTextIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in selectedTextIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForRead) as Entity;
                    var textPoint = Point3d.Origin;

                    if (textEnt == null)
                        throw new InvalidOperationException("textEnt was null");

                    var contents = "";

                    var type = textEnt.GetType().ToString();

                    switch (type)
                    {
                        case "Autodesk.AutoCAD.DatabaseServices.DBText":
                            var dbText = textEnt as DBText;
                            if (dbText == null)
                                throw new InvalidOperationException("dbText was null.");
                            contents = dbText.TextString;
                            textPoint = dbText.Position;
                            break;
                        case "Autodesk.AutoCAD.DatabaseServices.MText":
                            var mText = textEnt as MText;
                            if (mText == null)
                                throw new InvalidOperationException("mText was null.");
                            contents = mText.Contents;
                            textPoint = mText.Location;
                            break;

                    }

                    double elevText = StringHelpers.ExtractDoubleFromString(contents);

                    var point = useTextAsElevation ? new Point3d(textPoint.X, textPoint.Y, elevText) :
                                                     new Point3d(textPoint.X, textPoint.Y, textPoint.Z);

                    createAction(tr, point);
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Creates points at an offset between points.
        /// </summary>
        public static void Create_At_Offset_Between_Points(Action<Transaction, Point3d> createAction)
        {
            var graphics = new TransientGraphics();
            try
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Pick first point: ", out Point3d firstPoint))
                    return;

                graphics.DrawX(firstPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetPoint("\n3DS> Pick second point: ", out Point3d secondPoint))
                    return;

                graphics.DrawX(secondPoint, Settings.GraphicsSize);
                graphics.DrawLine(firstPoint, secondPoint);

                var baseLine = AngleHelpers.GetAngleBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());

                do
                {
                    using (var tr = AcadApp.StartTransaction())
                    {
                        if (!EditorUtils.TryGetDistance("\n3DS> Enter distance along line: ", firstPoint, out var horizontalDist))
                            break;

                        if (horizontalDist == null)
                            return;

                        var basePoint = PointHelpers.AngleAndDistanceToPoint(baseLine, horizontalDist.Value, firstPoint.ToPoint());

                        if (!EditorUtils.TryGetDistance("\n3DS> Enter left offset: ", basePoint.ToPoint3d(), out var leftOffsetDist))
                            break;

                        if (leftOffsetDist == null)
                            return;

                        var leftOffsetPt = PointHelpers.AngleAndDistanceToPoint(baseLine - 90, leftOffsetDist.Value, basePoint);
                        graphics.DrawDot(leftOffsetPt.ToPoint3d(), Settings.GraphicsSize);
                        createAction(tr, leftOffsetPt.ToPoint3d());

                        if (!EditorUtils.TryGetDistance("\n3DS> Enter right offset: ", basePoint.ToPoint3d(), out var rightOffsetDist))
                            break;

                        if (rightOffsetDist == null)
                            return;

                        var rightOffsetPt = PointHelpers.AngleAndDistanceToPoint(baseLine + 90, rightOffsetDist.Value, basePoint);
                        graphics.DrawDot(rightOffsetPt.ToPoint3d(), Settings.GraphicsSize);
                        createAction(tr, rightOffsetPt.ToPoint3d());

                        tr.Commit();

                        graphics.ClearGraphics();
                        //redraw base graphics.
                        graphics.DrawX(firstPoint, Settings.GraphicsSize);
                        graphics.DrawX(secondPoint, Settings.GraphicsSize);
                        graphics.DrawLine(firstPoint, secondPoint);
                    }
                } while (true);
            }
            catch (Exception e)
            {
                AcadApp.WriteErrorMessage(e);
            }
            finally
            {
                graphics.Dispose();
            }
        }

        /// <summary>
        /// Creates at distance between points.
        /// </summary>
        /// <remarks>
        /// After picking the two points to define the line, the point numbers, slope, horizontal,
        /// and vertical distances are displayed. The points created do not have to lie between the
        /// chosen points. You can enter a negative distance to create a point back from the first
        /// point or a distance greater than the distance between the points to create a point beyond
        /// the second point.
        /// </remarks>
        public static void Create_At_Distance_Between_Points(Action<Transaction, Point3d> createAction)
        {
            using (var graphics = new TransientGraphics())
            using (var tr = AcadApp.StartTransaction())
            {
                if (!EditorUtils.TryGetPoint("\n3DS> First point: ", out Point3d firstPoint))
                    return;

                graphics.DrawPlus(firstPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetPoint("\n3DS> Second point: ", out Point3d secondPoint))
                    return;

                var deltaZ = secondPoint.Z - firstPoint.Z;
                var angle = AngleHelpers.GetAngleBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
                var distBetween = PointHelpers.GetDistanceBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
                var midPoint = PointHelpers.GetMidpointBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());

                graphics.DrawPlus(secondPoint, Settings.GraphicsSize);
                graphics.DrawLine(firstPoint, secondPoint);
                graphics.DrawArrow(midPoint.ToPoint3d(), angle, Settings.GraphicsSize);

                AcadApp.Editor.WriteMessage($"\n3DS> Total distance: {Math.Round(distBetween, 3)}");

                do
                {
                    if (!EditorUtils.TryGetDouble("\n3DS> Enter distance: ", out var distance, allowZero: false))
                        break;

                    if (distance == null)
                        return;

                    var point = PointHelpers.AngleAndDistanceToPoint(angle, distance.Value, firstPoint.ToPoint());
                    var elevation = firstPoint.Z + deltaZ * (distance / distBetween).Value;

                    var newPoint = new Point3d(point.X, point.Y, elevation);
                    graphics.DrawPlus(newPoint, Settings.GraphicsSize);
                    createAction(tr, newPoint);

                } while (true);

                tr.Commit();
            }
        }

        /// <summary>
        /// Add multiple points (with interpolated elevation) between two points.
        /// </summary>
        public static void Create_Between_Points(Action<Transaction, Point3d> createAction)
        {
            var graphics = new TransientGraphics();
            try
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Pick first point: ", out Point3d firstPoint))
                    return;

                graphics.DrawPlus(firstPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetPoint("\n3DS> Pick second point: ", out Point3d secondPoint))
                    return;

                graphics.DrawPlus(secondPoint, Settings.GraphicsSize);
                graphics.DrawLine(firstPoint, secondPoint);

                graphics.DrawChainage(firstPoint, secondPoint);

                // Calculate angle and distances from picked points.
                Angle angleBetweenPoints = AngleHelpers.GetAngleBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
                double distanceBetweenPoints = PointHelpers.GetDistanceBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
                double elevationDifference = secondPoint.Z - firstPoint.Z;

                using (var tr = AcadApp.StartTransaction())
                {
                    bool cancelled = false;
                    do
                    {
                        //TODO: Implement way to show point moving along line relative to mouse position for point creation.
                        /*
                        Having brain wave moment. Can use methods like intersect 2 bearings to calculate point
                        on the line relative to the mouse position. if we take the line and add 90°? depending
                        on which side of the line the mouse is. we can use the IsLeft() method.
                        need to pass in new graphics object so we can clear it each move.
                        write own event and handler. to pass points etc.
                        */

                        if (!EditorUtils.TryGetDistance("\n3DS> Enter distance: ", firstPoint, out var distance))
                            cancelled = true;

                        if (distance == null)
                            return;

                        var newPoint = PointHelpers.AngleAndDistanceToPoint(angleBetweenPoints, distance.Value, firstPoint.ToPoint());
                        var point3d = new Point3d(newPoint.X, newPoint.Y, firstPoint.Z + elevationDifference * (distance.Value / distanceBetweenPoints));

                        graphics.DrawDot(newPoint.ToPoint3d(), Settings.GraphicsSize);

                        createAction(tr, point3d);

                    } while (!cancelled);
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

        public static void Inverse(Point3d firstPoint, Point3d secondPoint)
        {
            var decimalPlaces = SystemVariables.LUPREC;

            var angle = AngleHelpers.GetAngleBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
            var distance = Math.Round(PointHelpers.GetDistanceBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint()), decimalPlaces);
            var delta = MathHelpers.DeltaPoint(firstPoint.ToPoint(), secondPoint.ToPoint(), decimalPlaces);
            var slope = Math.Round(Math.Abs(delta.Z / distance * 100), decimalPlaces);

            AcadApp.Editor.WriteMessage($"\n3DS> Angle: {angle} ({angle.Flip()})");
            AcadApp.Editor.WriteMessage($"\n3DS> Distance: {distance}");
            AcadApp.Editor.WriteMessage($"\n3DS> dX:{delta.X} dY:{delta.Y} dZ:{delta.Z}");
            AcadApp.Editor.WriteMessage($"\n3DS> Slope:{slope}%");
        }


        /// <summary>
        /// Inverses between points (pick), echoes coordinates,
        /// azimuths, bearings, horz/vert distance and slope.
        /// </summary>
        public static void Inverse_Pick()
        {
            var graphics = new TransientGraphics();
            try
            {
                // Pick first point.
                if (!EditorUtils.TryGetPoint("\n3DS> Pick first point: ", out Point3d firstPoint))
                    return;

                // Highlight first point.
                graphics.DrawX(firstPoint, Settings.GraphicsSize);

                // Pick second point.
                if (!EditorUtils.TryGetPoint("\n3DS> Pick second point: ", out Point3d secondPoint))
                    return;

                Inverse(firstPoint, secondPoint);
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
        /// Does the same as <see cref="Inverse_Pick"/> but displays the information on the screen.
        /// </summary>
        public static void Inverse_Pick_Display()
        {
            var graphics = new TransientGraphics();
            try
            {
                while (true)
                {
                    bool loopPick = EditorUtils.TryGetPoint("\n3DS> Pick first point: ", out Point3d firstPoint);

                    if (!loopPick)
                        break;

                    // Highlight first point.
                    graphics.DrawX(firstPoint, Settings.GraphicsSize);

                    // Pick second point.
                    if (!EditorUtils.TryGetPoint("\n3DS> Pick second point: ", out Point3d secondPoint))
                        return;

                    var decimalPlaces = SystemVariables.LUPREC;

                    var angle = AngleHelpers.GetAngleBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
                    var distance = Math.Round(PointHelpers.GetDistanceBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint()), decimalPlaces);
                    var delta = MathHelpers.DeltaPoint(firstPoint.ToPoint(), secondPoint.ToPoint(), decimalPlaces);
                    var slope = Math.Round(Math.Abs(delta.Z / distance * 100), decimalPlaces);

                    var midPoint = PointHelpers.GetMidpointBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
                    graphics.ClearGraphics();
                    graphics.DrawX(firstPoint, Settings.GraphicsSize);
                    graphics.DrawX(secondPoint, Settings.GraphicsSize);
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
        /// Inverses the perpendicular.
        /// </summary>
        public static void Inverse_Pick_Perpendicular()
        {
            var graphics = new TransientGraphics();
            try
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Pick first point: ", out Point3d firstPoint))
                    return;

                graphics.DrawPlus(firstPoint, Settings.GraphicsSize);

                if (!EditorUtils.TryGetPoint("\n3DS> Pick second point: ", out Point3d secondPoint))
                    return;

                graphics.DrawPlus(secondPoint, Settings.GraphicsSize);
                graphics.DrawLine(firstPoint, secondPoint);

                do
                {
                    if (!EditorUtils.TryGetPoint("\n3DS> Pick offset point: ", out Point3d pickedPoint))
                        break;

                    var canIntersect = PointHelpers.PerpendicularIntersection(firstPoint.ToPoint(), secondPoint.ToPoint(), pickedPoint.ToPoint(), out Point intersectionPoint);

                    if (!canIntersect)
                    {
                        AcadApp.Editor.WriteMessage("\n3DS> No intersection found. ");
                        continue;
                    }

                    graphics.DrawX(intersectionPoint.ToPoint3d(), Settings.GraphicsSize);
                    graphics.DrawX(pickedPoint, Settings.GraphicsSize);
                    graphics.DrawLine(pickedPoint, intersectionPoint.ToPoint3d());

                    var distance = Math.Round(PointHelpers.GetDistanceBetweenPoints(pickedPoint.ToPoint(), intersectionPoint), SystemVariables.LUPREC);
                    var angle = AngleHelpers.GetAngleBetweenPoints(pickedPoint.ToPoint(), intersectionPoint);

                    var midPt = PointHelpers.GetMidpointBetweenPoints(pickedPoint.ToPoint(), intersectionPoint);

                    graphics.DrawText(midPt.ToPoint3d(), $"bearing: {angle} \\P dist: {distance}", 1, angle);

                    AcadApp.Editor.WriteMessage($"\n3DS> Angle: {angle}");
                    AcadApp.Editor.WriteMessage($"\n3DS> Distance: {distance}");

                } while (true);
            }
            catch (Exception e)
            {
                AcadApp.WriteErrorMessage(e);
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
        /// <remarks>Don't forget to Commit(); the transaction after using.</remarks>
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
        }

    }
}