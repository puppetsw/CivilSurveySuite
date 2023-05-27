using System;
using System.Diagnostics;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using CivilSurveySuite.Shared.Helpers;
using CivilSurveySuite.Shared.Models;
using CivilSurveySuite.Shared.Services.Interfaces;
using CivilSurveySuite.UI.Helpers;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace CivilSurveySuite.ACAD
{
    /// <summary>
    /// Slightly improved version of the original Traverse lisp routine
    /// written and created by Ted Whitney.
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

        /// <summary>
        /// Gets the converted distance value.
        /// </summary>
        /// <returns>System.Nullable&lt;System.Double&gt;.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private double? GetDistance(Point3d basePoint, out string keyword)
        {
            keyword = string.Empty;

            switch (_currentUnitType)
            {
                case UnitType.Metre:
                    return !EditorUtils.TryGetDistance(
                        string.Format(ResourceHelpers.GetLocalisedString("SpecifyDistanceInOr"), GetUnitsString()),
                        basePoint, new[] { ResourceHelpers.GetLocalisedString("Units") },
                        ResourceHelpers.GetLocalisedString("Units"), out keyword, out double? distance)
                        ? null
                        : distance;
                case UnitType.Feet:
                    return GetFeetAndInches(basePoint, out keyword);
                case UnitType.Link:
                    return GetLinks(basePoint, out keyword);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private double? GetLinks(Point3d basePoint, out string keyword)
        {
            if (!EditorUtils.TryGetDistance(
                    string.Format(ResourceHelpers.GetLocalisedString("SpecifyDistanceInOr"), GetUnitsString()),
                    basePoint, new[] { ResourceHelpers.GetLocalisedString("Units") },
                    ResourceHelpers.GetLocalisedString("Units"), out keyword, out double? links))
                return null;

            if (links == null)
                return null;

            double? distance = MathHelpers.ConvertLinkToMeters(links.Value);
            return distance;
        }

        private double? GetFeetAndInches(Point3d basePoint, out string keyword)
        {
            if (!EditorUtils.TryGetDistance(
                    string.Format(ResourceHelpers.GetLocalisedString("SpecifyDistanceInOr"), GetUnitsString()),
                    basePoint, new[] { ResourceHelpers.GetLocalisedString("Units") },
                    ResourceHelpers.GetLocalisedString("Units"), out keyword, out double? feet))
                return null;

            if (feet == null || !string.IsNullOrEmpty(keyword))
                return null;

            if (!EditorUtils.TryGetDouble(ResourceHelpers.GetLocalisedString("SpecifyInches"), out double? inches,
                    useDefaultValue: true, defaultValue: 0))
                return null;

            double? distance = MathHelpers.FeetToMeters(feet);

            if (inches != null)
                distance += MathHelpers.InchesToMeters(inches);

            return distance;
        }

        private string GetUnitsString()
        {
            switch (_currentUnitType)
            {
                case UnitType.Metre:
                    return ResourceHelpers.GetLocalisedString("Metres").ToLower();
                case UnitType.Feet:
                    return ResourceHelpers.GetLocalisedString("Feet").ToLower();
                case UnitType.Link:
                    return ResourceHelpers.GetLocalisedString("Links").ToUpper();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Traverse()
        {
            var graphics = new TransientGraphics();

            try
            {
                if (!EditorUtils.TryGetPoint(ResourceHelpers.GetLocalisedString("SpecifyBasePoint"), out Point3d basePoint))
                    return;

                AcadApp.Editor.WriteMessage("\n");

                graphics.DrawPlus(basePoint, GRAPHICS_SIZE);

                do
                {
                    if (!EditorUtils.TryGetAngle(ResourceHelpers.GetLocalisedString("SpecifyBearingOr"), basePoint, out var bearing))
                        return;

                    if (bearing == null)
                        break;

                    graphics.DrawLine(basePoint, PointHelpers.AngleAndDistanceToPoint(bearing, 1000000, basePoint.ToPoint()).ToPoint3d());

                    double? distance;
                    bool selectingUnits = true;
                    do
                    {
                        distance = GetDistance(basePoint, out var keyword);

                        if (distance > 0)
                        {
                            graphics.Undo();
                            graphics.DrawLine(basePoint, PointHelpers.AngleAndDistanceToPoint(bearing, distance.Value, basePoint.ToPoint()).ToPoint3d(), useDashedLine: true);
                        }

                        if (!string.IsNullOrEmpty(keyword))
                        {
                            if (string.Equals(keyword, ResourceHelpers.GetLocalisedString("Units"), StringComparison.CurrentCultureIgnoreCase))
                            {
                                var pkoUnits = new PromptKeywordOptions(ResourceHelpers.GetLocalisedString("SpecifyUnits"))
                                {
                                    AppendKeywordsToMessage = true
                                };
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
                        var pko = new PromptKeywordOptions(ResourceHelpers.GetLocalisedString("AcceptAndContinue"))
                        {
                            AppendKeywordsToMessage = true
                        };
                        pko.Keywords.Add(Keywords.ACCEPT);
                        pko.Keywords.Add(Keywords.CHANGE);
                        pko.Keywords.Add(Keywords.FLIP);
                        pko.Keywords.Default = Keywords.ACCEPT;

                        Debug.Assert(distance != null);
                        Point newPoint = PointHelpers.AngleAndDistanceToPoint(bearing, distance.Value, basePoint.ToPoint());

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
                            default:
                                return;
                        }
                    } while (!traverseAccepted);
                } while (true);
            }
            catch (Exception e)
            {
                Ioc.Default.GetInstance<ILogger>().Error(e, e.Message);
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