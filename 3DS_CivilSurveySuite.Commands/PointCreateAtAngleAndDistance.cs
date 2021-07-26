// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite_ACADBase21;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.Commands
{
    public static class PointCreateAtAngleAndDistance
    {
        public static void RunCommand()
        {
            if (!EditorUtils.GetBasePoint3d(out Point3d basePoint, "\n3DS> Select a base point: "))
                return;

            if (!EditorUtils.GetAngle(out Angle angle, "\n3DS> Pick angle: ", basePoint))
                return;

            if (!EditorUtils.GetDistance(out double dist, "\n3DS> Offset distance: ", basePoint))
                return;

            AutoCADActive.Editor.WriteMessage($"\n3DS> Angle: {angle}");
            AutoCADActive.Editor.WriteMessage($"\n3DS> Distance: {dist}");

            var pko = new PromptKeywordOptions("\n3DS> Flip angle? ") { AppendKeywordsToMessage = true };
            pko.Keywords.Add(Keywords.Accept);
            pko.Keywords.Add(Keywords.Cancel);
            pko.Keywords.Add(Keywords.Flip);

            Point point = MathHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());

            using (var graphics = new TransientGraphics())
            {
                graphics.DrawCircle(point.ToPoint3d());
                graphics.DrawLine(basePoint, point.ToPoint3d());

                var cancelled = false;
                PromptResult prResult;
                do
                {
                    prResult = AutoCADActive.Editor.GetKeywords(pko);

                    if (prResult.Status != PromptStatus.Keyword && 
                        prResult.Status != PromptStatus.OK)
                        continue;

                    switch (prResult.StringResult)
                    {
                        case Keywords.Accept:
                            Points.CreatePoint(point.ToPoint3d());
                            cancelled = true;
                            break;
                        case Keywords.Cancel:
                            cancelled = true;
                            break;
                        case Keywords.Flip:
                            angle = angle.Flip();
                            point = MathHelpers.AngleAndDistanceToPoint(angle, dist, basePoint.ToPoint());
                            graphics.ClearGraphics();
                            graphics.DrawCircle(point.ToPoint3d());
                            graphics.DrawLine(basePoint, point.ToPoint3d());
                            break;
                    }
                } while (prResult.Status != PromptStatus.Cancel && 
                         prResult.Status != PromptStatus.Error && !cancelled);
            }
        }
    }
}