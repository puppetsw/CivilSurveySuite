﻿using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite_ACADBase21;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.GraphicsInterface;

namespace _3DS_CivilSurveySuite.Commands
{
    /// <summary>
    /// Traverse utility with command line input
    /// </summary>
    public static class TraverseUtils
    {
        public static void DrawTraverse(IReadOnlyList<TraverseAngleObject> angleList)
        {
            var point = EditorUtils.GetBasePoint2d();

            if (point == null)
                return;

            var basePoint = point.Value.ToPoint();

            AutoCADActive.Editor.WriteMessage($"\n3DS> Base point set: X:{point.Value.X} Y:{point.Value.Y}");

            //get coordinates based on traverse data

            var pko = new PromptKeywordOptions("\n3DS> Accept and draw traverse?") { AppendKeywordsToMessage = true };
            pko.Keywords.Add(Keywords.Accept);
            pko.Keywords.Add(Keywords.Cancel);
            pko.Keywords.Add(Keywords.Redraw);

            var tg = new TransientGraphics(TransientDrawingMode.Highlight);

            try
            {
                // Lock ACAD document and start transaction as we are running from Palette.
                using (Transaction tr = AutoCADActive.StartLockedTransaction())
                {
                    var coordinates = MathHelpers.TraverseAngleObjectsToCoordinates(angleList, basePoint);

                    // Draw Transient Graphics of Traverse.
                    DrawTraverseGraphics(tg, coordinates);
                    var cancelled = false;
                    PromptResult prResult;
                    do
                    {
                        prResult = AutoCADActive.Editor.GetKeywords(pko);
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
                AutoCADActive.Editor.WriteMessage(e.Message);
            }
            finally
            {
                tg.ClearGraphics();
            }
        }

        public static void DrawTraverse(IReadOnlyList<TraverseObject> traverseList)
        { 
            var point = EditorUtils.GetBasePoint2d();

            if (point == null)
                return;

            AutoCADActive.Editor.WriteMessage($"\n3DS> Base point set: X:{point.Value.X} Y:{point.Value.Y}");

            var basePoint = point.Value.ToPoint();

            var pko = new PromptKeywordOptions("\n3DS> Accept traverse and draw linework? ") { AppendKeywordsToMessage = true };
            pko.Keywords.Add(Keywords.Accept);
            pko.Keywords.Add(Keywords.Cancel);
            pko.Keywords.Add(Keywords.Redraw);

            var tg = new TransientGraphics(TransientDrawingMode.Highlight);

            try
            {
                //lock acad document and start transaction
                using (Transaction tr = AutoCADActive.StartLockedTransaction())
                {
                    var coordinates = MathHelpers.TraverseObjectsToCoordinates(traverseList, basePoint);

                    //draw first transient traverse
                    DrawTraverseGraphics(tg, coordinates);

                    var cancelled = false;
                    PromptResult prResult;
                    do
                    {
                        prResult = AutoCADActive.Editor.GetKeywords(pko);
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
                AutoCADActive.Editor.WriteMessage(e.Message);
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
    }
}
