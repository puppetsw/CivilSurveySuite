using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Autodesk.Civil.Settings;
using CivilSurveySuite.ACAD;
using CivilSurveySuite.Common.Helpers;
using CivilSurveySuite.Common.Models;

namespace CivilSurveySuite.CIVIL
{
    public static class CogoPointUtils
    {
        public static void CreatePoint(Transaction tr, Point3d position)
        {
            var cogoPoints = C3DApp.ActiveDocument.CogoPoints;
            var cogoPointId = cogoPoints.Add(position, true);

            EditorUtils.TryGetString("\nEnter raw description: ", out string rawDescription);

            var cogoPoint = tr.GetObject(cogoPointId, OpenMode.ForWrite) as CogoPoint;

            if (cogoPoint == null)
                throw new InvalidOperationException("cogoPoint was null.");

            cogoPoint.RawDescription = rawDescription;
            cogoPoint.DowngradeOpen();
        }

        /// <summary>
        /// Gets the <see cref="CogoPoint"/> objects associated with the <see cref="PointGroup"/>.
        /// </summary>
        /// <param name="pointGroup">The point group.</param>
        /// <returns>IEnumerable&lt;CogoPoint&gt;.</returns>
        public static IEnumerable<CogoPoint> GetFromPointGroup(PointGroup pointGroup)
        {
            var list = new List<CogoPoint>();
            using (var tr = AcadApp.StartTransaction())
            {
                foreach (uint pointNumber in pointGroup.GetPointNumbers())
                {
                    var cogoPoint = GetByPointNumber(tr, (int)pointNumber);
                    list.Add(cogoPoint);
                }
                tr.Commit();
            }
            return list;
        }

        /// <summary>
        /// Gets the <see cref="CogoPoint"/> objects associated with the <see cref="PointGroup"/>.
        /// </summary>
        /// <param name="pointGroupName">Name of the point group.</param>
        /// <returns>IEnumerable&lt;CogoPoint&gt;.</returns>
        public static IEnumerable<CogoPoint> GetFromPointGroup(string pointGroupName)
        {
            var list = new List<CogoPoint>();
            using (var tr = AcadApp.StartTransaction())
            {
                var pointGroup = PointGroupUtils.GetPointGroupByName(tr, pointGroupName);
                foreach (uint pointNumber in pointGroup.GetPointNumbers())
                {
                    var cogoPoint = GetByPointNumber(tr, (int)pointNumber);
                    list.Add(cogoPoint);
                }
                tr.Commit();
            }
            return list;
        }

        /// <summary>
        /// Gets a list of <see cref="CivilPoint"/> objects associated with the <see cref="PointGroup"/>
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="pointGroupName">Name of the point group.</param>
        /// <returns>IEnumerable&lt;CivilPoint&gt;.</returns>
        public static IEnumerable<CivilPoint> GetCivilPointsFromPointGroup(Transaction tr, string pointGroupName)
        {
            var list = new List<CivilPoint>();
            var pointGroup = PointGroupUtils.GetPointGroupByName(tr, pointGroupName);
            foreach (uint pointNumber in pointGroup.GetPointNumbers())
            {
                var cogoPoint = GetByPointNumber(tr, (int)pointNumber);

                if (cogoPoint == null)
                {
                    Debug.WriteLine($"WARNING: Unable to get CogoPoint for point number: {pointNumber}");
                    continue;
                }

                list.Add(cogoPoint.ToCivilPoint());
            }
            return list;
        }

        /// <summary>
        /// Changes a <see cref="CogoPoint"/>'s Raw Description to upper case.
        /// </summary>
        public static void RawDescriptionToUpper()
        {
            if (!EditorUtils.TryGetImpliedSelectionOfType<CogoPoint>(out var pointIds) &&
                !EditorUtils.TryGetSelectionOfType<CogoPoint>(
                    "\nSelect CogoPoints: ", "\nRemove CogoPoints: ", out pointIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in pointIds)
                {
                    var pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    pt.RawDescription = pt.RawDescription.ToUpper();
                    pt.DowngradeOpen();
                    pt.ApplyDescriptionKeys();
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Changes a <see cref="CogoPoint"/>'s DescriptionFormat to upper case.
        /// </summary>
        public static void DescriptionFormatToUpper()
        {
            if (!EditorUtils.TryGetImpliedSelectionOfType<CogoPoint>(out var pointIds) &&
                !EditorUtils.TryGetSelectionOfType<CogoPoint>(
                    "\nSelect CogoPoints: ", "\nRemove CogoPoints: ", out pointIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in pointIds)
                {
                    var pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    pt.DescriptionFormat = pt.DescriptionFormat.ToUpper();
                    pt.DowngradeOpen();
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// COMMAND: Select CogoPoints to scale the elevations by a given amount.
        /// </summary>
        public static void ScaleElevations()
        {
            // Use implied selection.
            if (!EditorUtils.TryGetImpliedSelectionOfType<CogoPoint>(out var pointIds) &&
                !EditorUtils.TryGetSelectionOfType<CogoPoint>(
                    "\nSelect CogoPoints: ", "\nRemove CogoPoints: ", out pointIds))
                return;

            if (!EditorUtils.TryGetDouble("\nScale amount", out var scaleAmount, true, 1.0d, false))
                return;

            if (scaleAmount == null)
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId id in pointIds)
                {
                    var pt = (CogoPoint) id.GetObject(OpenMode.ForRead);

                    pt.UpgradeOpen();

                    var elevation = pt.Elevation;
                    var scaledElevation = scaleAmount.Value * elevation;
                    pt.Elevation = scaledElevation;

                    pt.DowngradeOpen();
                }

                tr.Commit();
            }
        }

        /// <summary>
        /// Matches selected <see cref="CogoPoint"/>.LabelRotation to the selected line, polyline or 3d polyline.
        /// </summary>
        public static void LabelRotateMatch()
        {
            if (!EditorUtils.TryGetImpliedSelectionOfType<CogoPoint>(out var objectIds) &&
                !EditorUtils.TryGetSelectionOfType<CogoPoint>("\nSelect points: ", "\nRemove points: ", out objectIds))
                return;

            if (!EditorUtils.TryGetEntity("\nSelect line or polyline: ", "\nNot a valid line or a polyline: ",
                new[]
                {
                    typeof(Line),
                    typeof(Polyline),
                    typeof(Polyline2d),
                    typeof(Polyline3d)
                }, out var pickedPoint, out var perId))
                return;

            string lineType = perId.ObjectClass.DxfName;

            using var tr = AcadApp.StartTransaction();

            double angle = 0;

            switch (lineType)
            {
                case DxfNames.LWPOLYLINE:
                case DxfNames.POLYLINE:
                    var poly = perId.GetObject(OpenMode.ForRead) as Polyline;
                    if (poly != null)
                    {
                        angle = PolylineUtils.GetPolylineSegmentAngle(poly, pickedPoint);
                        break;
                    }

                    var poly3d = perId.GetObject(OpenMode.ForRead) as Polyline3d;
                    if (poly3d != null)
                    {
                        angle = PolylineUtils.GetPolyline3dSegmentAngle(poly3d, pickedPoint);
                    }

                    break;
                case DxfNames.LINE:
                    var line = (Line) perId.GetObject(OpenMode.ForRead);

                    // Check if the points of the line form and ordinary angle.
                    if (MathHelpers.IsOrdinaryAngle(line.StartPoint.ToPoint(), line.EndPoint.ToPoint()))
                    {
                        angle = line.Angle;
                    }
                    else
                    {
                        // if it isn't an ordinary angle, we flip it.
                        // because we don't really care if the radians are in clockwise or not
                        // we can just flip the angle without it being in the correct system.
                        angle = AngleHelpers.RadiansToAngle(line.Angle).Flip().ToRadians();
                    }

                    break;
            }

            AcadApp.Editor.WriteMessage("\nPolyline segment angle (radians): " + angle);

            foreach (ObjectId id in objectIds)
            {
                var pt = (CogoPoint) id.GetObject(OpenMode.ForRead);

                LabelStyle style;

                if (pt.LabelStyleId.IsNull)
                {
                    // Check point group for LabelStyle
                    var pointGroup = (PointGroup) tr.GetObject(pt.PrimaryPointGroupId, OpenMode.ForRead);
                    style = pointGroup.PointLabelStyleId.GetObject(OpenMode.ForRead) as LabelStyle;
                }
                else
                {
                    style = pt.LabelStyleId.GetObject(OpenMode.ForRead) as LabelStyle;
                }

                var textAngle = LabelUtils.GetFirstComponentAngle(style);

                // AcadApp.Editor.WriteMessage($"\nPoint label style current rotation (radians): {textAngle}");
                // AcadApp.Editor.WriteMessage($"\nRotating label to {angle} to match polyline segment");

                pt.UpgradeOpen();
                pt.LabelRotation = 0;
                pt.ResetLabelLocation();
                pt.LabelRotation -= textAngle;
                pt.LabelRotation += angle;
                pt.DowngradeOpen();
            }
            tr.Commit();
        }

        public static void LabelRotateMatchNear()
        {
            // Select a bunch of CogoPoints and rotate them based on
            // their position to the lines/polyline vertexes in their proximity.
        }

        public static void LabelResetSelection()
        {
            if (!EditorUtils.TryGetImpliedSelectionOfType<CogoPoint>(out var pointIds) &&
                !EditorUtils.TryGetSelectionOfType<CogoPoint>("\nSelect CogoPoint labels to reset: ", "\nRemove CogoPoint labels", out pointIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in pointIds)
                {
                    var cogoPoint = tr.GetObject(objectId, OpenMode.ForWrite) as CogoPoint;

                    if (cogoPoint == null)
                        continue;

                    cogoPoint.ResetLabelLocation();
                    cogoPoint.ResetLabelRotation();
                    cogoPoint.DowngradeOpen();
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Turns the label mask on or off.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void LabelMaskToggle(bool value)
        {
            var onOffText = value ? "on" : "off";

            if (!EditorUtils.TryGetSelectionOfType<CogoPoint>($"\nSelect CogoPoints to turn label mask(s) {onOffText}: ",
                    "\nRemove CogoPoint labels", out var objectIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var cogoPoint = tr.GetObject(objectId, OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint == null)
                        continue;

                    var labelStyle = tr.GetObject(cogoPoint.LabelStyleId, OpenMode.ForRead) as LabelStyle;
                    labelStyle.LabelMask(tr, value);
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Positions multiple <see cref="CogoPoint"/> labels below the first selected <see cref="CogoPoint"/>.
        /// Rotation is based on the first text component of the LabelStyle.
        /// Spacing is based on 10% of the label text height.
        /// </summary>
        public static void LabelStack()
        {
            if (!EditorUtils.TryGetEntityOfType<CogoPoint>("\nSelect first CogoPoint: ",
                    "\nRemove CogoPoint: ", out var entId, true))
                return;

            double labelOffset = 0;

            using (var tr = AcadApp.StartTransaction())
            {
                var baseCogoPoint = tr.GetObject(entId, OpenMode.ForRead) as CogoPoint;

                if (baseCogoPoint == null)
                    throw new InvalidOperationException("baseCogoPoint was null.");

                var labelStyle = tr.GetObject(baseCogoPoint.LabelStyleId, OpenMode.ForRead) as LabelStyle;
                var labelHeight = LabelUtils.GetHeight(labelStyle);

                //add option to use the cogoPoint rotation rather than the component rotation?
                //or ability to type the rotation in.
                var textAngle = LabelUtils.GetFirstComponentAngle(labelStyle);
                var angle = AngleHelpers.RadiansToAngle(textAngle).ToClockwise();

                var startLocation = baseCogoPoint.Location;
                var dX = baseCogoPoint.Location.X - baseCogoPoint.LabelLocation.X;
                var dY = baseCogoPoint.Location.Y - baseCogoPoint.LabelLocation.Y;

                while (true)
                {
                    if (!EditorUtils.TryGetEntityOfType<CogoPoint>("\nSelect CogoPoint: ",
                        "\nRemove CogoPoint: ", out var objectId, true))
                        break;

                    var cogoPoint = tr.GetObject(objectId, OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint == null)
                        throw new InvalidOperationException("cogoPoint was null.");

                    // There seems to be an issue with setting the label location of a dragged label.
                    // So we reset the label first, before we change it and use the deltaX and deltaY
                    // to calculate the offset back to the point below the first text.
                    cogoPoint.ResetLabelLocation();

                    // Add 10% of the label height as spacing between labels.
                    labelOffset += labelHeight + labelHeight / 10;

                    cogoPoint.UpgradeOpen();

                    // Calculate new label location.
                    var newPoint = PointHelpers.AngleAndDistanceToPoint(angle + 90, labelOffset, startLocation.ToPoint()).ToPoint3d();
                    cogoPoint.LabelLocation = new Point3d(newPoint.X - dX, newPoint.Y - dY, 0);

                    cogoPoint.DowngradeOpen();
                    AcadApp.Editor.Regen();
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Matches selected <see cref="CogoPoint"/>. MarkerRotation to the selected line, polyline or 3d polyline.
        /// </summary>
        public static void MarkerRotateMatch()
        {
            if (!EditorUtils.TryGetImpliedSelectionOfType<CogoPoint>(out var objectIds) &&
                !EditorUtils.TryGetSelectionOfType<CogoPoint>("\nSelect points: ", "\nRemove points: ", out objectIds))
                return;

            if (!EditorUtils.TryGetEntity(
                    "\nSelect line or polyline: ",
                    "\nNot a valid line or a polyline: ",
                    new[]
                    {
                        typeof(Line),
                        typeof(Polyline),
                        typeof(Polyline2d),
                        typeof(Polyline3d)
                    }, out var pickedPoint, out var perId))
                return;

            string typeOfLine = perId.ObjectClass.DxfName;
            using var tr = AcadApp.StartTransaction();
            double angle = 0;

            switch (typeOfLine)
            {
                case DxfNames.LWPOLYLINE:
                case DxfNames.POLYLINE:
                    var poly = perId.GetObject(OpenMode.ForRead) as Polyline;
                    if (poly != null)
                    {
                        angle = PolylineUtils.GetPolylineSegmentAngle(poly, pickedPoint);
                        break;
                    }

                    var poly3d = perId.GetObject(OpenMode.ForRead) as Polyline3d;
                    if (poly3d != null)
                    {
                        angle = PolylineUtils.GetPolyline3dSegmentAngle(poly3d, pickedPoint);
                    }

                    break;
                case DxfNames.LINE:
                    var line = (Line)perId.GetObject(OpenMode.ForRead);

                    // Check if the points of the line form and ordinary angle.
                    if (MathHelpers.IsOrdinaryAngle(line.StartPoint.ToPoint(), line.EndPoint.ToPoint()))
                    {
                        angle = line.Angle;
                    }
                    else
                    {
                        // if it isn't an ordinary angle, we flip it.
                        // because we don't really care if the radians are in clockwise or not
                        // we can just flip the angle without it being in the correct system.
                        angle = AngleHelpers.RadiansToAngle(line.Angle).Flip().ToRadians();
                    }

                    break;
            }

            foreach (ObjectId id in objectIds)
            {
                var pt = (CogoPoint)id.GetObject(OpenMode.ForRead);

                pt.UpgradeOpen();

                pt.MarkerRotation = 0;
                pt.MarkerRotation += angle;

                pt.DowngradeOpen();
            }

            tr.Commit();
        }

        [Obsolete("This method is obsolete. Use the ReplaceDuplicateService implementation.")]
        public static void Create_Trunks_At_Trees()
        {
            //Use settings to determine codes for TRNK and TRE
            //Add option to set style for tree and trunk?
            //Change this to a service with window.
            //Call it ReplaceTreeSymbols
            var counter = 0;

            using (Transaction tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId pointId in C3DApp.ActiveDocument.CogoPoints)
                {
                    var cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint is null)
                        continue;

                    if (!cogoPoint.RawDescription.Contains("TRE "))
                        continue;

                    ObjectId trunkPointId = C3DApp.ActiveDocument.CogoPoints.Add(cogoPoint.Location, true);
                    CogoPoint trunkPoint = trunkPointId.GetObject(OpenMode.ForWrite) as CogoPoint;

                    if (trunkPoint != null)
                    {
                        trunkPoint.RawDescription = cogoPoint.RawDescription.Replace("TRE ", "TRNK ");
                        trunkPoint.ApplyDescriptionKeys();

                        cogoPoint.UpgradeOpen();
                        cogoPoint.RawDescription = cogoPoint.RawDescription.Replace("TRE ", "TREE ");
                        cogoPoint.ApplyDescriptionKeys();
                    }
                    counter++;
                }
                tr.Commit();
            }

            string completeMessage = "Changed " + counter + " TRE points, and created " + counter + " TRNK points";
            AcadApp.Editor.WriteMessage(completeMessage);
        }

        public static void AddLineBreakToDescription(string lineBreakText = "{}")
        {
            // Where in the text do you want the linebreak?
            // Convert a text symbol to the new line? i.e. {NL}. NL for new line.
            // Check if the text has the lineBreakText key.
            if (!EditorUtils.TryGetSelectionOfType<CogoPoint>(
                    "\nSelect CogoPoint: ", "\nRemove CogoPoints: ", out var objectIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var cogoPoint = tr.GetObject(objectId, OpenMode.ForWrite) as CogoPoint;

                    if (cogoPoint == null)
                        throw new InvalidOperationException("Got unexpected objectId for CogoPoint.");

                    // Get the width of the label style used by the CogoPoint.
                    var labelStyle = tr.GetObject(cogoPoint.LabelStyleId, OpenMode.ForRead) as LabelStyle;

                    if (labelStyle == null)
                        throw new InvalidOperationException("Got unexpected objectId for CogoPoint's LabelStyle.");

                    // loop each text component in the label style and calculate the maximum width.
                    int maxWidth = LabelUtils.GetWidth(tr, labelStyle);
                    maxWidth += 5; // Seems like the labels have a bit extra 'space' on the width.
                                   // This might be related to the current scale and the text size?

                    // Check how many lineBreakText keys are in the description format.
                    // I didn't want to use Regex, but seems I can't escape it.
                    var occurrences = Regex.Matches(cogoPoint.DescriptionFormat, lineBreakText).Count;

                    for (int i = 0; i < occurrences; i++)
                    {
                        // calculate how many spaces to add for the occurrence.
                        // get position of lineBreakText in the description format.

                        int lineBreakTextPosition = cogoPoint.DescriptionFormat.IndexOf(lineBreakText, StringComparison.Ordinal);

                        int amountOfPadToAdd = maxWidth - lineBreakTextPosition;

                        if (amountOfPadToAdd < 0)
                            amountOfPadToAdd += maxWidth * i;

                        var padString = ""; // Is there a better way to do this?
                        padString = padString.PadLeft(amountOfPadToAdd + 5 * i);

                        string text = cogoPoint.DescriptionFormat.ReplaceFirst(lineBreakText, "");
                        text = text.Insert(lineBreakTextPosition, padString);
                        cogoPoint.DescriptionFormat = text;
                    }
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Moves a CogoPoint to a new location.
        /// </summary>
        /// <param name="deltaX">The X amount to move.</param>
        /// <param name="deltaY">The Y amount to move.</param>
        public static void Move_CogoPoint_Labels(double deltaX, double deltaY)
        {
            if (!EditorUtils.TryGetSelectionOfType<CogoPoint>(
                    "\nSelect CogoPoints to move: ", "\nRemove CogoPoints: ", out var objectIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var cogoPoint = tr.GetObject(objectId, OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint == null)
                        continue;

                    var currentLocation = cogoPoint.LabelLocation;

                    cogoPoint.UpgradeOpen();
                    cogoPoint.LabelLocation = new Point3d(currentLocation.X + deltaX, currentLocation.Y + deltaY, 0);
                    cogoPoint.DowngradeOpen();
                }
                tr.Commit();
            }
            AcadApp.Editor.Regen();
        }

        /// <summary>
        /// Convert a <see cref="CogoPoint"/> to a <see cref="CivilPoint"/>.
        /// </summary>
        /// <param name="cogoPoint">The <see cref="CogoPoint"/> to convert.</param>
        /// <returns>A <see cref="CivilPoint"/> representing the <see cref="CogoPoint"/>.</returns>
        public static CivilPoint ToCivilPoint(this CogoPoint cogoPoint)
        {
            return new CivilPoint
            {
                PointNumber = cogoPoint.PointNumber,
                Easting = cogoPoint.Easting,
                Northing = cogoPoint.Northing,
                Elevation = cogoPoint.Elevation,
                RawDescription = cogoPoint.RawDescription,
                DescriptionFormat = cogoPoint.DescriptionFormat,
                FullDescription = cogoPoint.FullDescription,
                ObjectId = cogoPoint.ObjectId.Handle.ToString(),
                PointName = cogoPoint.PointName
            };
        }

        public static IEnumerable<CivilPoint> ToListOfCivilPoints(this IEnumerable<CogoPoint> cogoPoints)
        {
            return cogoPoints.Select(cogoPoint => cogoPoint.ToCivilPoint()).ToList();
        }

        /// <summary>
        /// Inverses two <see cref="CogoPoint"/> entities by their point number.
        /// </summary>
        public static void Inverse_ByPointNumber()
        {
            if (!EditorUtils.TryGetInt("\nEnter first point number: ", out int firstPointNumber))
                return;

            if (!EditorUtils.TryGetInt("\nEnter second point number: ", out int secondPointNumber))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                var cogoPoint1 = GetByPointNumber(tr, firstPointNumber);
                var cogoPoint2 = GetByPointNumber(tr, secondPointNumber);

                if (cogoPoint1 != null && cogoPoint2 != null)
                {
                    PointUtils.Inverse(cogoPoint1.Location, cogoPoint2.Location);
                }

                tr.Commit();
            }
        }

        /// <summary>
        /// The UsedPt command displays a list of used point numbers in the command window.
        /// Usage
        /// Type UsedPt at the command line.The available point numbers in the drawing are displayed in the
        /// command window, as in the following example:
        /// </summary>
        public static void UsedPoints()
        {
            using (var tr = AcadApp.StartTransaction())
            {
                var pointNumbers = PointGroupUtils.GroupRange(tr, "_All Points");
                AcadApp.Editor.WriteMessage($"\n{pointNumbers}");
                tr.Commit();
            }
        }

        /// <summary>
        /// Gets a <see cref="CogoPoint"/> by it's point number.
        /// </summary>
        /// <param name="tr">Transaction.</param>
        /// <param name="pointNumber">The point number of the <see cref="CogoPoint"/>.</param>
        /// <returns>A <see cref="CogoPoint"/> matching the <param name="pointNumber">point number</param>
        /// otherwise null if no matching <see cref="CogoPoint"/> found.
        /// </returns>
        public static CogoPoint GetByPointNumber(Transaction tr, int pointNumber)
        {
            foreach (ObjectId objectId in C3DApp.ActiveDocument.CogoPoints)
            {
                var cogoPoint = tr.GetObject(objectId, OpenMode.ForRead) as CogoPoint;

                if (cogoPoint == null)
                    continue;

                if (cogoPoint.PointNumber == pointNumber)
                    return cogoPoint;
            }
            return null;
        }

        /// <summary>
        /// Provides a quick prompt allowing you to set the default for the next point number
        /// created (the current default is shown).
        /// </summary>
        public static void SetNextPointNumber()
        {
            using (var tr = AcadApp.StartTransaction())
            {
                if (EditorUtils.TryGetInt("\nSet Number: ", out int integer, true, GetNextPointNumber(tr)))
                    C3DApp.ActiveDocument.Settings.GetSettings<SettingsCmdCreatePoints>().PointIdentity.NextPointNumber.Value = (uint)integer;

                tr.Commit();
            }
        }

        /// <summary>
        /// Gets the next available point number in the active drawing.
        /// </summary>
        /// <param name="tr">The active <see cref="Transaction"/>.</param>
        /// <returns>A int representing the next point number.</returns>
        private static int GetNextPointNumber(Transaction tr)
        {
            var pointGroup = PointGroupUtils.GetPointGroupByName(tr, "_All Points");

            if (pointGroup == null)
                return 1;

            long num = 0;
            if (pointGroup.ObjectId.IsValid)
            {
                var uintList = new List<uint>(pointGroup.GetPointNumbers());
                if (uintList.Count > 0)
                {
                    uintList.Sort();
                    num = uintList[uintList.Count - 1];
                }
            }
            return (int)(num + 1L);
        }

        /// <summary>
        /// The ZoomPt command zooms the display to the specified point number.
        /// </summary>
        public static void ZoomPoint()
        {
            using (var tr = AcadApp.StartTransaction())
            {
                var cogoPoints = C3DApp.ActiveDocument.CogoPoints;

                if (!EditorUtils.TryGetInt("\nEnter point number: ", out int textInput))
                    return;

                CogoPoint zoomPt = null;
                foreach (ObjectId objectId in cogoPoints)
                {
                    var cogoPoint = tr.GetObject(objectId, OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint == null)
                    {
                        continue;
                    }

                    if (cogoPoint.PointNumber == textInput)
                    {
                        zoomPt = cogoPoint;
                        break;
                    }
                }

                if (zoomPt == null)
                {
                    AcadApp.Editor.WriteMessage("\nInvalid point number. ");
                }
                else
                {
                    EditorUtils.ZoomToEntity(zoomPt);
                }

                tr.Commit();
            }
        }

        /// <summary>
        /// Converts a string into a <see cref="UDPString"/>.
        /// </summary>
        /// <param name="udpName">The name of the UDPString</param>
        /// <returns>A <see cref="UDPString"/> if the <c>udpName</c> was a valid name.</returns>
        public static UDPString GetUDP(string udpName)
        {
            foreach (var udp in C3DApp.ActiveDocument.PointUDPs)
            {
                if (udp.Name == udpName)
                    return (UDPString)udp;
            }

            return null;
        }

        /// <summary>
        /// Gets a list of all the UDP names in the active drawing.
        /// </summary>
        /// <returns>A IEnumerable of UDP names.</returns>
        public static IEnumerable<string> GetUDPNames()
        {
            var list = new List<string>();
            foreach (var udp in C3DApp.ActiveDocument.PointUDPs)
            {
                list.Add(udp.Name);
            }
            return list;
        }

        public static void FullDescriptionToTextEntity()
        {
            if (!EditorUtils.TryGetEntityOfType<CogoPoint>("\nSelect CogoPoint: ", "\nPlease select CogoPoints only.", out var objectId))
            {
                return;
            }

            using (var tr = AcadApp.StartTransaction())
            {
                var cogoPoint = (CogoPoint)tr.GetObject(objectId, OpenMode.ForRead);
                TextUtils.CreateText(tr, cogoPoint.Location, cogoPoint.FullDescription);
                tr.Commit();
            }
        }

        public static void RawDescriptionToTextEntity()
        {
            if (!EditorUtils.TryGetEntityOfType<CogoPoint>("\nSelect CogoPoint: ", "\nPlease select CogoPoints only.", out var objectId))
            {
                return;
            }

            using (var tr = AcadApp.StartTransaction())
            {
                var cogoPoint = (CogoPoint)tr.GetObject(objectId, OpenMode.ForRead);
                TextUtils.CreateText(tr, cogoPoint.Location, cogoPoint.RawDescription);
                tr.Commit();
            }
        }
    }
}
