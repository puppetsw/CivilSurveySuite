using System;
using _3DS_CivilSurveySuite.Shared.Helpers;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace _3DS_CivilSurveySuite.ACAD
{
    /// <summary>
    /// Slightly improved version of the Traverse lisp command
    /// written by Edward Whitney, My dad.
    /// </summary>
    public class TraverseCommand : IAcadCommand
    {
        private const int GRAPHICS_SIZE = 6;

        private enum UnitType
        {
            Metre,
            Feet,
            Link
        }

        private UnitType _currentUnitType = UnitType.Metre;

        private double ApplyUnitConversion(double distance)
        {
            switch (_currentUnitType)
            {
                case UnitType.Metre:
                    return distance;
                case UnitType.Feet:
                    return MathHelpers.ConvertFeetToMeters(distance);
                case UnitType.Link:
                    return MathHelpers.ConvertLinkToMeters(distance);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Traverse()
        {
            var graphics = new TransientGraphics();

            try
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Select base point: ", out Point3d basePoint))
                {
                    return;
                }

                graphics.DrawPlus(basePoint, GRAPHICS_SIZE);

                do
                {
                    if (!EditorUtils.TryGetAngle("\n3DS> Enter bearing: ", basePoint, out var angle))
                    {
                        return;
                    }

                    AcadApp.Editor.WriteMessage($"\n3DS> {angle}");

                    double distance;
                    bool selectingUnits = true;
                    do
                    {
                        if (!EditorUtils.TryGetDistance("\n3DS> Distance: ", basePoint, new[] { "Units" }, "Units",
                                out var keyword, out distance))
                        {
                            return;
                        }

                        if (!string.IsNullOrEmpty(keyword))
                        {
                            if (keyword == "Units")
                            {
                                var pkoUnits = new PromptKeywordOptions("\n3DS> Select distance units: ") { AppendKeywordsToMessage = true };
                                pkoUnits.Keywords.Add(Keywords.METRE);
                                pkoUnits.Keywords.Add(Keywords.FEET);
                                pkoUnits.Keywords.Add(Keywords.LINK);

                                PromptResult prUnitResult = AcadApp.Editor.GetKeywords(pkoUnits);

                                switch (prUnitResult.StringResult)
                                {
                                    case Keywords.METRE:
                                        _currentUnitType = UnitType.Metre;
                                        break;
                                    case Keywords.FEET:
                                        _currentUnitType = UnitType.Feet;
                                        AcadApp.Editor.WriteMessage("\n3DS> Feet and inches are represented as a decimal.");
                                        AcadApp.Editor.WriteMessage("\nExample: 5 feet and 2 inches, expected input: 5.02.");
                                        AcadApp.Editor.WriteMessage("\nInches less than 10 must have a preceding zero.");
                                        break;
                                    case Keywords.LINK:
                                        _currentUnitType = UnitType.Link;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            AcadApp.Editor.WriteMessage($"\n3DS> {distance} Units: {_currentUnitType}");
                            selectingUnits = false;
                        }
                    } while (selectingUnits);

                    bool traverseAccepted = false;
                    do
                    {
                        var pko = new PromptKeywordOptions("\n3DS> Accept and continue traverse? ")
                        {
                            AppendKeywordsToMessage = true
                        };
                        pko.Keywords.Add(Keywords.ACCEPT);
                        pko.Keywords.Add(Keywords.CHANGE);
                        pko.Keywords.Add(Keywords.FLIP);
                        pko.Keywords.Default = Keywords.ACCEPT;

                        var calculatedDistance = ApplyUnitConversion(distance);
                        Point newPoint = PointHelpers.AngleAndDistanceToPoint(angle, calculatedDistance, basePoint.ToPoint());

                        graphics.DrawLine(basePoint.ToPoint2d(), newPoint.ToPoint2d());
                        graphics.DrawPlus(newPoint.ToPoint3d(), GRAPHICS_SIZE);
                        graphics.DrawArrow(PointHelpers.GetMidpointBetweenPoints(basePoint.ToPoint(), newPoint).ToPoint3d(),
                            angle, GRAPHICS_SIZE);

                        PromptResult prResult = AcadApp.Editor.GetKeywords(pko);

                        switch (prResult.StringResult)
                        {
                            case Keywords.ACCEPT:
                                using (var tr = AcadApp.StartTransaction())
                                {
                                    LineUtils.DrawLine(tr, basePoint, newPoint.ToPoint3d());
                                    tr.Commit();
                                }

                                basePoint = newPoint.ToPoint3d();
                                graphics.ClearGraphics();
                                graphics.DrawPlus(basePoint, GRAPHICS_SIZE);
                                traverseAccepted = true;
                                break;
                            case Keywords.CHANGE:
                                graphics.ClearGraphics();
                                graphics.DrawPlus(basePoint, GRAPHICS_SIZE);
                                traverseAccepted = true;
                                break;
                            case Keywords.FLIP:
                                angle += 180;
                                graphics.ClearGraphics();
                                graphics.DrawPlus(basePoint, GRAPHICS_SIZE);
                                break;
                        }
                    } while (!traverseAccepted);
                } while (true);
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

        public void Execute()
        {
            Traverse();
        }
    }
}