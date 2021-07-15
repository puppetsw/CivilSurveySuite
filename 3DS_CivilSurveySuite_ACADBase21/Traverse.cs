using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace _3DS_CivilSurveySuite_ACADBase21
{
    /// <summary>
    /// Traverse utility with command line input
    /// </summary>
    public class Traverse
    {
        public static void DrawTraverse(IReadOnlyList<TraverseAngleObject> angleList)
        {
            var point = EditorUtils.GetBasePoint2d();

            if (point == null)
                return;

            var basePoint = point.Value.ToPoint();

            AutoCADActive.Editor.WriteMessage($"\n3DS> Base point set: X:{point.Value.X} Y:{point.Value.Y}");

            //get coordinates based on traverse data
            var coordinates = MathHelpers.AngleAndDistanceToCoordinates(angleList, basePoint);

            var pko = new PromptKeywordOptions("\n3DS> Accept and draw traverse?") { AppendKeywordsToMessage = true };
            pko.Keywords.Add(Keywords.Accept);
            pko.Keywords.Add(Keywords.Cancel);
            pko.Keywords.Add(Keywords.Redraw);

            try
            {
                // Lock ACAD document and start transaction as we are running from Palette.
                using (Transaction tr = AutoCADActive.StartLockedTransaction())
                {
                    // Draw Transient Graphics of Traverse.
                    TransientGraphics.ClearTransientGraphics();
                    // Draw first transient traverse
                    TransientGraphics.DrawTransientTraverse(coordinates.ToListOfPoint2d());
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
                                    TransientGraphics.ClearTransientGraphics();
                                    coordinates = MathHelpers.AngleAndDistanceToCoordinates(angleList, basePoint);
                                    TransientGraphics.DrawTransientTraverse(coordinates.ToListOfPoint2d());
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
                TransientGraphics.ClearTransientGraphics();
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

            try
            {
                //lock acad document and start transaction
                using (Transaction tr = AutoCADActive.StartLockedTransaction())
                {
                    //draw first transient traverse
                    var coordinates = MathHelpers.BearingAndDistanceToCoordinates(traverseList, basePoint);

                    TransientGraphics.ClearTransientGraphics();
                    //draw first transient traverse
                    TransientGraphics.DrawTransientTraverse(coordinates.ToListOfPoint2d());

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
                                    TransientGraphics.ClearTransientGraphics();
                                    coordinates = MathHelpers.BearingAndDistanceToCoordinates(traverseList, basePoint);
                                    TransientGraphics.DrawTransientTraverse(coordinates.ToListOfPoint2d());
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
                TransientGraphics.ClearTransientGraphics();
            }
        }
    }
}
