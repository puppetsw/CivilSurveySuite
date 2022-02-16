// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.UI.Helpers;
using _3DS_CivilSurveySuite.UI.Models;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class TraverseUtils
    {
        private const int GRAPHICS_SIZE = 6;

        public static void Traverse()
        {
            var graphics = new TransientGraphics();
            try
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Select base point: ", out Point3d basePoint))
                    return;

                graphics.DrawPlus(basePoint, GRAPHICS_SIZE);

                if (!EditorUtils.TryGetAngle("\n3DS> Enter bearing: ", basePoint, out var angle))
                    return;

                AcadApp.Editor.WriteMessage($"\n3DS> {angle}");

                //TODO: Add feet/units conversion
                if (!EditorUtils.TryGetDistance("\n3DS> Distance: ", basePoint, out double distance))
                    return;

                var pko = new PromptKeywordOptions("\n3DS> Continue? ") { AppendKeywordsToMessage = true };
                pko.Keywords.Add(Keywords.ACCEPT);
                pko.Keywords.Add(Keywords.CANCEL);
                pko.Keywords.Add(Keywords.CHANGE);
                pko.Keywords.Add(Keywords.FLIP);

                Point newPoint = PointHelpers.AngleAndDistanceToPoint(angle, distance, basePoint.ToPoint());
                graphics.DrawLine(basePoint.ToPoint2d(), newPoint.ToPoint2d());
                graphics.DrawPlus(newPoint.ToPoint3d(), GRAPHICS_SIZE);
                graphics.DrawArrow(PointHelpers.GetMidpointBetweenPoints(basePoint.ToPoint(), newPoint).ToPoint3d(), angle, GRAPHICS_SIZE);

                PromptResult prResult = AcadApp.Editor.GetKeywords(pko);

                switch (prResult.StringResult)
                {
                    case Keywords.ACCEPT:
                        using (var tr = AcadApp.StartTransaction())
                        {
                            LineUtils.DrawLine(tr, basePoint, newPoint.ToPoint3d());
                            tr.Commit();
                        }
                        break;
                    case Keywords.CANCEL:
                        break;
                    case Keywords.CHANGE:
                        break;
                    case Keywords.FLIP:
                        break;
                }
            }
            catch (Exception e)
            {
                AcadApp.Editor.WriteMessage($"Exception: {e.Message}");
            }
            finally
            {
                graphics.Dispose();
            }
        }
    }
}