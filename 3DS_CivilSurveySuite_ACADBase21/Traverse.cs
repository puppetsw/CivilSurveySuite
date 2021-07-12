using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite_ACADBase21;
using _3DS_CivilSurveySuite_ACADBase21.Abstraction;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Exception = System.Exception;

[assembly: CommandClass(typeof(Traverse))]
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

            var basePoint = new Point(point.Value.X, point.Value.Y); //HACK: Ignores 3d point

            AutoCADApplicationManager.Editor.WriteMessage($"\nBase point set: X:{point.Value.X} Y:{point.Value.Y}");

            //get coordinates based on traverse data
            var coordinates = MathHelpers.AngleAndDistanceToCoordinates(angleList, basePoint);

            var pko = new PromptKeywordOptions("\nAccept and draw traverse?") { AppendKeywordsToMessage = true };
            pko.Keywords.Add(Keywords.Accept);
            pko.Keywords.Add(Keywords.Cancel);
            pko.Keywords.Add(Keywords.Redraw);

            try
            {
                // Lock ACAD document and start transaction as we are running from Palette.
                using (Transaction tr = AutoCADApplicationManager.StartLockedTransaction())
                {
                    // Draw Transient Graphics of Traverse.
                    var cancelled = false;
                    PromptResult prResult;
                    do
                    {
                        prResult = AutoCADApplicationManager.Editor.GetKeywords(pko);
                        if (prResult.Status == PromptStatus.Keyword || prResult.Status == PromptStatus.OK)
                        {
                            switch (prResult.StringResult)
                            {
                                case Keywords.Redraw: //if redraw update the coordinates clear transients and redraw
                                    coordinates = MathHelpers.AngleAndDistanceToCoordinates(angleList, basePoint);
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
                Console.WriteLine(e); //TODO: Handle exception better at later date.
                throw;
            }
            finally
            {
            }
        }

        public static void DrawTraverse(IReadOnlyList<TraverseObject> traverseList)
        { 
            var point = EditorUtils.GetBasePoint2d();

            if (point == null)
                return;

            AutoCADApplicationManager.Editor.WriteMessage($"\nBase point set: X:{point.Value.X} Y:{point.Value.Y}");

            var basePoint = new Point(point.Value.X, point.Value.Y); //HACK: Ignores 3d point

            var pko = new PromptKeywordOptions("\n3DS> Accept traverse and draw linework? ")
            {
                AppendKeywordsToMessage = true
            };
            pko.Keywords.Add("Accept");
            pko.Keywords.Add("Cancel");
            pko.Keywords.Add("Redraw");

            //lock acad document and start transaction
            using (Transaction tr = AutoCADApplicationManager.StartLockedTransaction())
            {
                //draw first transient traverse
                var coordinates = MathHelpers.BearingAndDistanceToCoordinates(traverseList, basePoint);
                var cancelled = false;
                PromptResult prResult;
                do
                {
                    prResult = AutoCADApplicationManager.Editor.GetKeywords(pko);
                    if (prResult.Status == PromptStatus.Keyword || prResult.Status == PromptStatus.OK)
                    {
                        switch (prResult.StringResult)
                        {
                            case "Redraw": //if redraw update the coordinates clear transients and redraw
                                coordinates = MathHelpers.BearingAndDistanceToCoordinates(traverseList, basePoint);
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
    }
}
