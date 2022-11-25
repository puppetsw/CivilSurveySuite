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
    /// written by Edward Whitney, my dad.
    /// </summary>
    public class TraverseCommand : IAcadCommand
    {
        private const int GRAPHICS_SIZE = 6;

        string _unitsString;

        private enum UnitType
        {
            Metre,
            Feet,
            Link
        }

        private UnitType _currentUnitType = UnitType.Metre;

        private double? ApplyUnitConversion(double? distance)
        {
            if (distance == null)
                return null;

            switch (_currentUnitType)
            {
                case UnitType.Metre:
                    return distance.Value;
                case UnitType.Feet:
                    return MathHelpers.ConvertFeetToMeters(distance.Value);
                case UnitType.Link:
                    return MathHelpers.ConvertLinkToMeters(distance.Value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetUnitsString()
        {
            switch (_currentUnitType)
            {
                case UnitType.Metre:
                    _unitsString = "metres";
                    break;
                case UnitType.Feet:
                    _unitsString = "feet and inches";
                    break;
                case UnitType.Link:
                    _unitsString = "links";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Traverse()
        {
            var graphics = new TransientGraphics();

            try
            {
                if (!EditorUtils.TryGetPoint("\n3DS> Specify base point: ", out Point3d basePoint))
                    return;

                graphics.DrawPlus(basePoint, GRAPHICS_SIZE);

                do
                {
                    if (!EditorUtils.TryGetAngle("\n3DS> Specify bearing or: ", basePoint, out var bearing))
                        return;

                    if (bearing == null)
                        break;

                    graphics.DrawLine(basePoint, PointHelpers.AngleAndDistanceToPoint(bearing, 1000000, basePoint.ToPoint()).ToPoint3d());

                    double? distance;
                    bool selectingUnits = true;
                    do
                    {
                        SetUnitsString();

                        if (!EditorUtils.TryGetDistance($"\n3DS> Specify distance in {_unitsString} or: ", basePoint, new[] { "Units" }, "Units",
                                out var keyword, out distance))
                            return;

                        if (distance == null)
                            return;

                        graphics.Undo();
                        graphics.DrawLine(basePoint, PointHelpers.AngleAndDistanceToPoint(bearing, distance.Value, basePoint.ToPoint()).ToPoint3d(), useDashedLine: true);

                        if (!string.IsNullOrEmpty(keyword))
                        {
                            if (keyword == "Units")
                            {
                                var pkoUnits = new PromptKeywordOptions($"\n3DS> Specify units: ") { AppendKeywordsToMessage = true };
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
                                        AcadApp.Editor.WriteMessage("\n----------------------------------------------------");
                                        AcadApp.Editor.WriteMessage("\nFeet and inches are represented as a decimal number.");
                                        AcadApp.Editor.WriteMessage("\nExample: 5 feet and 2 inches, expected input: 5.02.");
                                        AcadApp.Editor.WriteMessage("\nInches less than 10 must have a preceding zero.");
                                        AcadApp.Editor.WriteMessage("\n----------------------------------------------------");
                                        break;
                                    case Keywords.LINK:
                                        _currentUnitType = UnitType.Link;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            selectingUnits = false;
                        }
                    } while (selectingUnits);

                    graphics.Undo();

                    bool traverseAccepted = false;
                    do
                    {
                        var pko = new PromptKeywordOptions("\n3DS> Accept and continue? ")
                        {
                            AppendKeywordsToMessage = true
                        };
                        pko.Keywords.Add(Keywords.ACCEPT);
                        pko.Keywords.Add(Keywords.CHANGE);
                        pko.Keywords.Add(Keywords.FLIP);
                        pko.Keywords.Default = Keywords.ACCEPT;

                        var calculatedDistance = ApplyUnitConversion(distance);

                        if (calculatedDistance == null)
                            throw new InvalidOperationException("distance was null");

                        Point newPoint = PointHelpers.AngleAndDistanceToPoint(bearing, calculatedDistance.Value, basePoint.ToPoint());

                        graphics.DrawLine(basePoint.ToPoint2d(), newPoint.ToPoint2d());
                        graphics.DrawPlus(newPoint.ToPoint3d(), GRAPHICS_SIZE);

                        PromptResult prResult = AcadApp.Editor.GetKeywords(pko);

                        switch (prResult.StringResult)
                        {
                            case Keywords.ACCEPT:
                            {
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
                            }
                            case Keywords.CHANGE:
                            {
                                graphics.ClearGraphics();
                                graphics.DrawPlus(basePoint, GRAPHICS_SIZE);
                                traverseAccepted = true;
                                break;
                            }
                            case Keywords.FLIP:
                            {
                                bearing += 180;
                                graphics.ClearGraphics();
                                graphics.DrawPlus(basePoint, GRAPHICS_SIZE);
                                break;
                            }
                            case "":
                                return;
                        }
                    } while (!traverseAccepted);
                } while (true);
            }
            catch (Exception e)
            {
                Ioc.Default.GetInstance<ILogger>()?.Error(e, e.Message);
                AcadApp.Editor.WriteMessage($"Exception: {e.Message}");
            }
            finally
            {
                graphics.ClearGraphics();
                graphics.Dispose();
            }
        }

        public void Execute()
        {
            Traverse();
        }
    }
}