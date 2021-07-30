// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Point = _3DS_CivilSurveySuite.Model.Point;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    /// <summary>
    /// Traverse utility with command line input
    /// </summary>
    public class TraverseService : ITraverseService
    {
        public void DrawTraverse(IReadOnlyList<TraverseAngleObject> angleList)
        {
            var point = EditorUtils.GetBasePoint2d();

            if (point == null)
                return;

            var basePoint = point.Value.ToPoint();

            AcadUtils.Editor.WriteMessage($"\n3DS> Base point set: X:{point.Value.X} Y:{point.Value.Y}");

            //get coordinates based on traverse data

            var pko = new PromptKeywordOptions("\n3DS> Accept and draw traverse?") { AppendKeywordsToMessage = true };
            pko.Keywords.Add(Keywords.Accept);
            pko.Keywords.Add(Keywords.Cancel);
            pko.Keywords.Add(Keywords.Redraw);

            var tg = new TransientGraphics(TransientDrawingMode.Highlight);

            try
            {
                // Lock ACAD document and start transaction as we are running from Palette.
                using (Transaction tr = AcadUtils.StartLockedTransaction())
                {
                    var coordinates = MathHelpers.TraverseAngleObjectsToCoordinates(angleList, basePoint);

                    // Draw Transient Graphics of Traverse.
                    DrawTraverseGraphics(tg, coordinates);
                    var cancelled = false;
                    PromptResult prResult;
                    do
                    {
                        prResult = AcadUtils.Editor.GetKeywords(pko);
                        if (prResult.Status == PromptStatus.Keyword || prResult.Status == PromptStatus.OK)
                        {
                            switch (prResult.StringResult)
                            {
                                case Keywords.Redraw: //if redraw update the coordinates clear transients and redraw
                                    coordinates = MathHelpers.TraverseAngleObjectsToCoordinates(angleList, basePoint);
                                    DrawTraverseGraphics(tg, coordinates);
                                    break;
                                case Keywords.Accept:
                                    Lines.DrawLines(tr, coordinates.ToListOfPoint3d());
                                    cancelled = true;
                                    break;
                                case Keywords.Cancel:
                                    cancelled = true;
                                    break;
                            }
                        }
                    } while (prResult.Status != PromptStatus.Cancel && prResult.Status != PromptStatus.Error && !cancelled);

                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                AcadUtils.Editor.WriteMessage(e.Message);
            }
            finally
            {
                tg.ClearGraphics();
            }
        }

        public void DrawTraverse(IReadOnlyList<TraverseObject> traverseList)
        { 
            var point = EditorUtils.GetBasePoint2d();

            if (point == null)
                return;

            AcadUtils.Editor.WriteMessage($"\n3DS> Base point set: X:{point.Value.X} Y:{point.Value.Y}");

            var basePoint = point.Value.ToPoint();

            var pko = new PromptKeywordOptions("\n3DS> Accept traverse and draw linework? ") { AppendKeywordsToMessage = true };
            pko.Keywords.Add(Keywords.Accept);
            pko.Keywords.Add(Keywords.Cancel);
            pko.Keywords.Add(Keywords.Redraw);

            var tg = new TransientGraphics(TransientDrawingMode.Highlight);

            try
            {
                //lock acad document and start transaction
                using (Transaction tr = AcadUtils.StartLockedTransaction())
                {
                    var coordinates = MathHelpers.TraverseObjectsToCoordinates(traverseList, basePoint);

                    //draw first transient traverse
                    DrawTraverseGraphics(tg, coordinates);

                    var cancelled = false;
                    PromptResult prResult;
                    do
                    {
                        prResult = AcadUtils.Editor.GetKeywords(pko);
                        if (prResult.Status == PromptStatus.Keyword || prResult.Status == PromptStatus.OK)
                        {
                            switch (prResult.StringResult)
                            {
                                case "Redraw": //if redraw update the coordinates clear transients and redraw
                                    coordinates = MathHelpers.TraverseObjectsToCoordinates(traverseList, basePoint);
                                    DrawTraverseGraphics(tg, coordinates);
                                    break;
                                case "Accept":
                                    Lines.DrawLines(tr, coordinates.ToListOfPoint3d());
                                    cancelled = true;
                                    break;
                                case "Cancel":
                                    cancelled = true;
                                    break;
                            }
                        }
                    } while (prResult.Status != PromptStatus.Cancel && prResult.Status != PromptStatus.Error && !cancelled);

                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                AcadUtils.Editor.WriteMessage(e.Message);
            }
            finally
            {
                tg.ClearGraphics();
            }
        }

        private static void DrawTraverseGraphics(TransientGraphics graphics, IReadOnlyList<Point> coordinates)
        {
            // Clear existing graphics
            graphics.ClearGraphics();

            var points = coordinates.ToListOfPoint3d();

            graphics.DrawLines(points);

            // If the list count is greater than 2, we show the boxes
            if (coordinates.Count >= 2)
            {
                var endPoint = coordinates[coordinates.Count - 1].ToPoint3d();
                var startPoint = coordinates[0].ToPoint3d();

                graphics.DrawBox(endPoint, 4);
                graphics.DrawX(endPoint, 4);

                graphics.DrawBox(startPoint, 4);
                graphics.DrawX(startPoint, 4);
            }
        }



        public static void Traverse_RunCommand()
        {
            // Pick base point
            if (!EditorUtils.GetBasePoint3d(out Point3d basePoint, "\n3DS> Select base point: ")) 
                return;

            var cancelled = false;
            var traverse = new List<TraverseObject>();

            do
            {
                if (!EditorUtils.GetAngle(out Angle angle, "\n3DS> Bearing: ", basePoint))
                    cancelled = true;

                //TODO: Add feet/units conversion
                if (!EditorUtils.GetDistance(out double distance, "\n3DS> Distance: ", basePoint))
                    cancelled = true;

                var pko = new PromptKeywordOptions("\n3DS> Continue? ") { AppendKeywordsToMessage = true };
                pko.Keywords.Add(Keywords.Accept);
                pko.Keywords.Add(Keywords.Cancel);
                pko.Keywords.Add(Keywords.Change);
                pko.Keywords.Add(Keywords.Flip);

                var coordinates = MathHelpers.TraverseObjectsToCoordinates(traverse, basePoint.ToPoint());

                var innerCancelled = false;
                do
                {
                    //draw transient graphics.
                    var graphics = new TransientGraphics(TransientDrawingMode.Main);

                    graphics.DrawLines(coordinates.ToListOfPoint3d());

                    PromptResult prResult = AcadUtils.Editor.GetKeywords(pko);
                    try
                    {
                        // Draw current lines in traverse.
                        //graphics.DrawLine();
                        // Draw new line to be added.
                        var newLine = new TraverseObject(angle.ToDouble(), distance);
                        var newLineCoord = MathHelpers.AngleAndDistanceToPoint(angle, distance, basePoint.ToPoint());

                        graphics.DrawingMode = TransientDrawingMode.Highlight;
                        graphics.DrawLine(basePoint.ToPoint2d(), newLineCoord.ToPoint2d());

                        if (traverse.Count > 1)
                        {

                        }

                        switch (prResult.StringResult)
                        {
                            case Keywords.Accept:
                                traverse.Add(newLine);
                                //innerCancelled = true;
                                break;
                            //case Keywords.Cancel:
                            //    cancelled = true;
                            //    break;
                            case Keywords.Change:
                                innerCancelled = true;
                                break;
                            case Keywords.Flip:
                                angle = angle.Flip();
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        AcadUtils.Editor.WriteMessage(e.Message);
                    }
                    finally
                    {
                        graphics.Dispose();
                    }
                } while (!innerCancelled);
            } while (!cancelled);


            // Bearing
            // Distance
            // Show transient line
            // Ask to confirm or flip or cancel
            // Cancel removes the transient line and starts a bearing

            // If more than 2 in list show zoom to closure option.

        }

        private static void DrawTraverseTransientGraphic(TransientGraphics graphics, IReadOnlyList<TraverseObject> traverse, TraverseObject currentLine)
        {

        }

        public static void TraverseAngle_RunCommand()
        {

        }

    }
}
