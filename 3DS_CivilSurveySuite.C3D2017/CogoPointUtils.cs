// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Autodesk.Civil.Settings;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class CogoPointUtils
    {
        //TODO: add more parameters for setting elevation etc.
        public static void CreatePoint(Transaction tr, Point3d position)
        {
            var cogoPoints = C3DApp.ActiveDocument.CogoPoints;
            var cogoPointId = cogoPoints.Add(position, true);

            if (!EditorUtils.GetString(out string rawDescription, "\n3DS> Enter raw description: "))
                return;

            var cogoPoint = tr.GetObject(cogoPointId, OpenMode.ForWrite) as CogoPoint;

            if (cogoPoint == null)
                throw new NullReferenceException(nameof(cogoPoint));

            cogoPoint.RawDescription = rawDescription;
            cogoPoint.DowngradeOpen();
        }

        /// <summary>
        /// Gets the <see cref="CogoPoint"/> objects associated with the <see cref="PointGroup"/>.
        /// </summary>
        /// <param name="pointGroup">The point group.</param>
        /// <returns>IEnumerable&lt;CogoPoint&gt;.</returns>
        public static IEnumerable<CogoPoint> GetCogoPointsInPointGroup(PointGroup pointGroup)
        {
            var list = new List<CogoPoint>();
            using (var tr = AcadApp.StartTransaction())
            {
                foreach (uint pointNumber in pointGroup.GetPointNumbers())
                {
                    var cogoPoint = GetCogoPointByPointNumber(tr, (int)pointNumber);
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
        public static IEnumerable<CogoPoint> GetCogoPointsInPointGroup(string pointGroupName)
        {
            var list = new List<CogoPoint>();
            using (var tr = AcadApp.StartTransaction())
            {
                var pointGroup = PointGroupUtils.GetPointGroupByName(tr, pointGroupName);
                foreach (uint pointNumber in pointGroup.GetPointNumbers())
                {
                    var cogoPoint = GetCogoPointByPointNumber(tr, (int)pointNumber);
                    list.Add(cogoPoint);
                }
                tr.Commit();
            }
            return list;
        }

        /// <summary>
        /// Gets a list of <see cref="CivilPoint"/> objects associated with the <see cref="PointGroup"/>
        /// </summary>
        /// <param name="pointGroupName">Name of the point group.</param>
        /// <returns>IEnumerable&lt;CivilPoint&gt;.</returns>
        public static IEnumerable<CivilPoint> GetCivilPointsInPointGroup(string pointGroupName)
        {
            var list = new List<CivilPoint>();
            using (var tr = AcadApp.StartTransaction())
            {
                var pointGroup = PointGroupUtils.GetPointGroupByName(tr, pointGroupName);
                foreach (uint pointNumber in pointGroup.GetPointNumbers())
                {
                    var cogoPoint = GetCogoPointByPointNumber(tr, (int)pointNumber);
                    list.Add(cogoPoint.ToCivilPoint());
                }
                tr.Commit();
            }
            return list;
        }

        /// <summary>
        /// Changes a <see cref="CogoPoint"/>'s Draw Description to upper case.
        /// </summary>
        public static void RawDescription_ToUpper()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;

            using (Transaction tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in pso.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    pt.RawDescription = pt.RawDescription.ToUpper();
                    pt.DowngradeOpen();
                }

                tr.Commit();
            }
        }

        /// <summary>
        /// Changes a <see cref="CogoPoint"/>'s Full Description to upper case.
        /// </summary>
        public static void FullDescription_ToUpper()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;

            using (Transaction tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in pso.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    pt.DescriptionFormat = pt.DescriptionFormat.ToUpper();
                    pt.DowngradeOpen();
                }

                tr.Commit();
            }
        }

        public static void Label_Rotate_Match()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;

            var perLines = EditorUtils.GetEntity(new[] { typeof(Line), typeof(Polyline), typeof(Polyline2d), typeof(Polyline3d) }, "\n3DS> Select line or polyline.");

            if (perLines.Status != PromptStatus.OK)
                return;

            string lineType = perLines.ObjectId.ObjectClass.DxfName;
            using (Transaction tr = AcadApp.StartTransaction())
            {
                double angle = 0;

                switch (lineType)
                {
                    case DxfNames.LWPOLYLINE:
                    case DxfNames.POLYLINE:
                        var poly = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline;
                        if (poly != null)
                        {
                            angle = PolylineUtils.GetPolylineSegmentAngle(poly, perLines.PickedPoint);
                            break;
                        }

                        var poly3d = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline3d;
                        if (poly3d != null)
                        {
                            angle = PolylineUtils.GetPolyline3dSegmentAngle(poly3d, perLines.PickedPoint);
                        }

                        break;
                    case DxfNames.LINE:
                        var line = (Line) perLines.ObjectId.GetObject(OpenMode.ForRead);

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

                AcadApp.Editor.WriteMessage("\n3DS> Polyline segment angle (radians): " + angle);

                foreach (ObjectId id in pso.Value.GetObjectIds())
                {
                    var pt = (CogoPoint) id.GetObject(OpenMode.ForRead);
                    var style = pt.LabelStyleId.GetObject(OpenMode.ForRead) as LabelStyle;
                    double textAngle = LabelUtils.GetLabelStyleComponentAngle(style);

                    AcadApp.Editor.WriteMessage($"\n3DS> Point label style current rotation (radians): {textAngle}");
                    AcadApp.Editor.WriteMessage($"\n3DS> Rotating label to {angle} to match polyline segment");

                    pt.UpgradeOpen();
                    pt.LabelRotation = 0;
                    pt.ResetLabelLocation();
                    pt.LabelRotation -= textAngle;
                    pt.LabelRotation += angle;
                    pt.DowngradeOpen();
                }
                tr.Commit();
            }
        }

        public static void Label_Rotate_Match_All()
        {
            // select a bunch of cogopoints and rotate them based on their position to the selected lines/polylines vertexs

        }

        public static void Marker_Rotate_Match()
        {

        }

        public static void Label_Reset_All()
        {
            if (!EditorUtils.GetSelectionOfType<CogoPoint>(out var pointIds, "\n3DS> Select CogoPoints labels to reset: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in pointIds)
                {
                    var cogoPoint = tr.GetObject(objectId, OpenMode.ForWrite) as CogoPoint;

                    if (cogoPoint == null)
                        continue;

                    //cogoPoint.ResetLabel();
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
        public static void Label_Mask_Toggle(bool value)
        {
            if (!EditorUtils.GetSelectionOfType<CogoPoint>(out var objectIds, "\n3DS> Select CogoPoints to turn label mask(s) off: "))
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
            if (!EditorUtils.GetSelectionOfType<CogoPoint>(out var objectIds, "\n3DS> Select CogoPoint: ", "\n3DS> Remove CogoPoints: "))
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
                    int maxWidth = LabelUtils.GetLabelWidth(tr, labelStyle);
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

        public static void Stack_CogoPoint_Labels()
        {
            //TODO: add dialog for settings?

            //if (!EditorUtils.GetSelectionOfType<CogoPoint>(out var objectIds, "\n3DS> Select CogoPoints: "))
            //    return;

            // Put in loop?
            if (!EditorUtils.GetSelectionOfType<CogoPoint>(out var objectIds, "\n3DS> Select CogoPoints: ", "\n3DS> Remove CogoPoints: ", out string keyword, new []{"Settings"}))
                return;

            if (!string.IsNullOrEmpty(keyword))
            {
                // show settings
                return;
            }

            using (var tr = AcadApp.StartTransaction())
            {
                Point3d startLocation = default;
                double labelOffset = 0;

                //for (int i = list.Length; i-- > 0;)

                //for (var i = 0; i < objectIds.Count; i++)
                //Do loop in reverse cause we mainly do a crossing selection.
                //Maybe add option to flip order.

                // Point3d newLocation = label.AnchorInfo.Location + offset;
                // label.LabelLocation = newLocation;
                for (var i = objectIds.Count; i-- > 0;)
                {
                    ObjectId objectId = objectIds[i];
                    var cogoPoint = tr.GetObject(objectId, OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint == null) // if cogoPoint is null continue. It should never be null though.
                        continue;

                    var labelStyle = tr.GetObject(cogoPoint.LabelStyleId, OpenMode.ForRead) as LabelStyle;
                    var labelHeight = LabelUtils.CalculateLabelHeight(labelStyle);
                    double textAngle = LabelUtils.GetLabelStyleComponentAngle(labelStyle);
                    AcadApp.WriteMessage($"Label height {labelHeight}");

                    //TODO: This needs to be worked on further, not 100% working yet.
                    //The whole method I mean. 28/8
                    //Possilbe issue with using current location
                    //Need to reset label first?
                    cogoPoint.ResetLabelLocation();

                    if (i == objectIds.Count - 1)
                    {
                        startLocation = cogoPoint.LabelLocation;
                        continue;
                    }

                    labelOffset += labelHeight;
                    var angle = AngleHelpers.RadiansToAngle(textAngle).ToClockwise();
                    var newPoint = PointHelpers.AngleAndDistanceToPoint(angle + 90, labelOffset, startLocation.ToPoint());

                    //cogoPoint.LabelLocation = new Point3d(startLocation.X, startLocation.Y - labelOffset, 0);
                    cogoPoint.LabelLocation = newPoint.ToPoint3d();
                }

                AcadApp.Editor.Regen();
                tr.Commit();
            }
        }

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
                ObjectId = cogoPoint.ObjectId.Handle.ToString(),
                PointName = cogoPoint.PointName
            };
        }

        public static IEnumerable<CivilPoint> ToListOfCivilPoints(this IEnumerable<CogoPoint> cogoPoints)
        {
            return cogoPoints.Select(cogoPoint => cogoPoint.ToCivilPoint()).ToList();
        }

        public static void Move_CogoPoint_Label(double deltaX, double deltaY)
        {
            if (!EditorUtils.GetSelectionOfType<CogoPoint>(out var objectIds, "\n3DS> Select CogoPoints to move: "))
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
        /// Inverses two <see cref="CogoPoint"/> entities by their point number.
        /// </summary>
        public static void Inverse_ByPointNumber()
        {
            if (!EditorUtils.GetInt(out int firstPointNumber, "\n3DS> Enter first point number: "))
                return;

            if (!EditorUtils.GetInt(out int secondPointNumber, "\n3DS> Enter second point number: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                var cogoPoint1 = GetCogoPointByPointNumber(tr, firstPointNumber);
                var cogoPoint2 = GetCogoPointByPointNumber(tr, secondPointNumber);

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
                AcadApp.Editor.WriteMessage($"\n3DS> {pointNumbers}");
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
        public static CogoPoint GetCogoPointByPointNumber(Transaction tr, int pointNumber)
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
        public static void Set_Next_PointNumber()
        {
            using (var tr = AcadApp.StartTransaction())
            {
                if (EditorUtils.GetInt(out int integer, "\n3DS> Set Number: ", true, Get_Next_PointNumber(tr)))
                    C3DApp.ActiveDocument.Settings.GetSettings<SettingsCmdCreatePoints>().PointIdentity.NextPointNumber.Value = (uint)integer;

                tr.Commit();
            }
        }

        private static int Get_Next_PointNumber(Transaction tr)
        {
            var pointGroup = PointGroupUtils.GetPointGroupByName(tr, "_All Points");

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

                if (!EditorUtils.GetInt(out int textInput, "\n3DS> Enter point number: "))
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
                    AcadApp.Editor.WriteMessage("\n3DS> Invalid point number. ");
                }
                else
                {
                    EditorUtils.ZoomToEntity(zoomPt);
                }

                tr.Commit();
            }
        }
    }
}