// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Shared.Helpers;
using _3DS_CivilSurveySuite.Shared.Models;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Point = _3DS_CivilSurveySuite.Shared.Models.Point;
using Surface = Autodesk.Civil.DatabaseServices.Surface;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class SurfaceUtils
    {
        private const double TOLERANCE = 0.000000001;

        /// <summary>
        /// Gets a <see cref="TinSurface"/> by name.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="surfaceName">Name of the surface.</param>
        /// <returns>A <see cref="TinSurface"/> representing the surface data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <c>tr</c> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <c>surfaceName</c> is null or empty.</exception>
        public static TinSurface GetSurfaceByName(Transaction tr, string surfaceName)
        {
            if (tr == null)
                throw new ArgumentNullException(nameof(tr));

            if (string.IsNullOrEmpty(surfaceName))
                throw new ArgumentException(@"surfaceName was null or empty", nameof(surfaceName));

            var surfaceIds = C3DApp.ActiveDocument.GetSurfaceIds();

            foreach (ObjectId surfaceId in surfaceIds)
            {
                var surface = tr.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;

                if (surface == null)
                    continue;

                if (surface.Name == surfaceName)
                {
                    return surface;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a <see cref="TinSurface"/> by index.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="index">The index of the surface</param>
        /// <returns>A <see cref="TinSurface"/> representing the surface data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <c>tr</c> is null.</exception>
        public static TinSurface GetSurfaceByIndex(Transaction tr, int index)
        {
            if (tr == null)
                throw new ArgumentNullException(nameof(tr));

            var surfaceIds = C3DApp.ActiveDocument.GetSurfaceIds();
            var surfaceId = surfaceIds[index];

            return tr.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;
        }

        /// <summary>
        /// Gets a <see cref="TinSurface"/> by it's <see cref="ObjectId"/>.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="objectId">The <see cref="ObjectId"/> of the <see cref="TinSurface"/>.</param>
        /// <returns>A <see cref="TinSurface"/> representing the surface data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <c>tr</c> or <c>objectId</c> is null.</exception>
        public static TinSurface GetSurfaceByObjectId(Transaction tr, ObjectId objectId)
        {
            if (tr == null)
                throw new ArgumentNullException(nameof(tr));

            if (objectId.IsNull)
                throw new ArgumentNullException(nameof(objectId));

            return tr.GetObject(objectId, OpenMode.ForRead) as TinSurface;
        }

        [Obsolete("This method is obsolete. Use CivilSurface objects.")]
        public static IEnumerable<string> GetSurfaceNames(Transaction tr)
        {
            var surfaceIds = C3DApp.ActiveDocument.GetSurfaceIds();
            var surfaceNames = new List<string>();

            foreach (ObjectId surfaceId in surfaceIds)
            {
                var surface = tr.GetObject(surfaceId, OpenMode.ForRead) as Surface;

                if (surface == null)
                    continue;

                surfaceNames.Add(surface.Name);
            }

            return surfaceNames;
        }

        /// <summary>
        /// Gets an IEnumerable of <see cref="CivilSurface"/> from the active document.
        /// </summary>
        /// <returns>A IEnumerable of <see cref="CivilSurface"/>.</returns>
        public static IEnumerable<CivilSurface> GetCivilSurfaces()
        {
            var list = new List<CivilSurface>();
            using (var tr = AcadApp.StartLockedTransaction())
            {
                var surfaceIds = C3DApp.ActiveDocument.GetSurfaceIds();

                foreach (ObjectId surfaceId in surfaceIds)
                {
                    var surface = tr.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;
                    list.Add(surface.ToCivilSurface());
                }
                tr.Commit();
            }
            return list;
        }

        [Obsolete("This method is obsolete, use SurfaceExtensions.", true)]
        public static IEnumerable<CivilSurface> GetCivilSurfaces(Transaction tr)
        {
            var surfaceIds = C3DApp.ActiveDocument.GetSurfaceIds();
            var surfaces = new List<CivilSurface>();

            foreach (ObjectId surfaceId in surfaceIds)
            {
                var surface = tr.GetObject(surfaceId, OpenMode.ForRead) as Surface;

                if (surface == null)
                    continue;

                surfaces.Add(new CivilSurface
                {
                    ObjectId = surface.ObjectId.Handle.ToString(),
                    Name = surface.Name,
                    Description = surface.Description
                });
            }

            return surfaces;
        }

        [Obsolete("This method is obsolete, use SurfaceExtensions.", true)]
        public static CivilSurface GetCivilSurface(Transaction tr, ObjectId objectId)
        {
            var surface = tr.GetObject(objectId, OpenMode.ForRead) as TinSurface;

            if (surface == null)
                return null;

            return new CivilSurface { ObjectId = surface.ObjectId.Handle.ToString(), Name = surface.Name, Description = surface.Description };
        }

        /// <summary>
        /// Gets the surface elevation at picked point.
        /// </summary>
        public static void GetSurfaceElevationAtPoint()
        {
            if (!EditorUtils.TryGetPoint("\n3DS> Pick point: ", out Point3d pickedPoint))
                return;

            var surfaceIds = C3DApp.ActiveDocument.GetSurfaceIds();

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId surfaceId in surfaceIds)
                {
                    var surface = tr.GetObject(surfaceId, OpenMode.ForRead) as Surface;

                    if (surface == null)
                        continue;

                    var surfaceElev = surface.FindElevationAtXY(pickedPoint.X, pickedPoint.Y);
                    AcadApp.Editor.WriteMessage($"\n3DS> Surface Name: {surface.Name} Elevation: {Math.Round(surfaceElev, SystemVariables.LUPREC)}");
                }

                tr.Commit();
            }
        }

        /// <summary>
        /// Adds the selected breakline(s) to a surface
        /// </summary>
        public static void AddBreaklineToSurface()
        {
            if (!EditorUtils.TryGetSelectionOfType<Line, Polyline, Polyline3d>("\n3DS> Select breaklines: ",
                    "\n3DS> Remove breaklines: ", out var objectIds))
                return;

            // Which surface?
            using (var tr = AcadApp.StartTransaction())
            {
                var surfaces = C3DApp.ActiveDocument.GetSurfaceIds();

                // Check if objects are in more than one surface?
                var surface = surfaces.Count > 1
                    ? C3DApp.SelectSurface()
                    : GetSurfaceByIndex(tr, 0);

                if (surface == null)
                    return;

                var breaklineDefs = surface.BreaklinesDefinition;

                // Load defaults
                double midOrd = CommandSettings.GetDefaultBreaklineMidOrdinateDistance;
                double maxDist = CommandSettings.GetDefaultBreaklineSupplementingDistance;
                double weedDist = CommandSettings.GetDefaultBreaklineWeedingDistance;
                double weedAngle = CommandSettings.GetDefaultBreaklineWeedingAngle;

                breaklineDefs.AddStandardBreaklines(objectIds, midOrd, maxDist, weedDist, weedAngle);

                surface.Rebuild();
                tr.Commit();
            }

            AcadApp.Editor.Regen();
        }

        /// <summary>
        /// Removes the selected breakline(s) from the selected surface.
        /// </summary>
        public static void RemoveBreaklinesFromSurface()
        {
            if (!EditorUtils.TryGetSelectionOfType<Line, Polyline, Polyline3d>("\n3DS> Select breaklines: ",
                    "\n3DS> Remove breaklines: ", out var objectIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                var surface = C3DApp.ActiveDocument.GetSurfaceIds().Count > 1
                    ? C3DApp.SelectSurface()
                    : GetSurfaceByIndex(tr, 0);

                if (surface == null)
                    return;

                var breaklineDefs = surface.BreaklinesDefinition;
                var breaklinesToBeRemoved = new List<int>();

                for (var i = 0; i < breaklineDefs.Count; i++)
                {
                    var breaklineSet = breaklineDefs[i];

                    // Store current breakline details so we can re-create them.
                    double midOrd = breaklineSet.MidOrdinateDistance;
                    double maxDist = breaklineSet.MaximumDistance;
                    double weedDist = breaklineSet.WeedingDistance;
                    double weedAngle = breaklineSet.WeedingAngle;
                    string description = breaklineSet.Description;

                    var breaklineIds = surface.GetBreaklineEntityIds(breaklineSet);

                    for (var j = 0; j < breaklineIds.Count; j++)
                    {
                        foreach (ObjectId objectId in objectIds)
                        {
                            if (breaklineIds[j].Handle == objectId.Handle)
                                breaklinesToBeRemoved.Add(j); //Add index of handle match to remove list
                        }

                        breaklineIds.RemoveAtIndexes(breaklinesToBeRemoved);

                        // Remove current definition from surface
                        surface.BreaklinesDefinition.RemoveAt(i);

                        // Recreate the breakline set in the surface
                        var newBreaklineSet = breaklineDefs.AddStandardBreaklines(breaklineIds, midOrd, maxDist, weedDist, weedAngle);
                        newBreaklineSet.Description = description; //set description to old one.
                    }
                }
                surface.Rebuild();
                tr.Commit();
            }
            AcadApp.Editor.Regen();
            AcadApp.Editor.UpdateScreen(); // Probably not needed.
        }

        /// <summary>
        /// Gets the breakline entity ids.
        /// </summary>
        /// <param name="surf">The surf.</param>
        /// <param name="breaklineSet">The breakline op.</param>
        /// <returns>ObjectIdCollection.</returns>
        /// <remarks>
        /// Jeff M @  https://forums.autodesk.com/t5/civil-3d-customization/get-surfacebreaklines/td-p/4673903
        /// </remarks>
        private static ObjectIdCollection GetBreaklineEntityIds(this DBObject surf, SurfaceOperationAddBreakline breaklineSet)
        {
            string name = breaklineSet.Description;

            var result = new ObjectIdCollection();

            object tinSurface = surf.AcadObject;
            object breaklines = tinSurface.GetType().InvokeMember("Breaklines", BindingFlags.GetProperty, null, tinSurface, null);
            var breaklineCount = (int)breaklines.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, breaklines, null);

            var args = new object[1];

            for (var j = 0; j < breaklineCount; j++)
            {
                args[0] = j;
                object breakline = breaklines.GetType().InvokeMember("Item", BindingFlags.InvokeMethod, null, breaklines, args);
                var desc = (string)breakline.GetType().InvokeMember("Description", BindingFlags.GetProperty, null, breakline, null);

                if (desc != name)
                    continue;

                var entities = (object[])breakline.GetType().InvokeMember("BreaklineEntities", BindingFlags.GetProperty, null, breakline, null);
                for (var i = 0; i < entities.GetLength(0); i++)
                {
                    var id = DBObject.FromAcadObject(entities[i]);
                    result.Add(id);
                }
            }
            return result;
        }

        /// <summary>
        /// If there is more than one <see cref="TinSurface"/> in the active document, prompts the user to select one.
        /// </summary>
        /// <param name="tr">The active <see cref="Transaction"/>.</param>
        /// <returns>A <see cref="TinSurface"/>.</returns>
        /// <remarks>Uses the C3D SelectSurface service to prompt the user if required.</remarks>
        private static TinSurface SelectSurface(Transaction tr)
        {
            var surface = C3DApp.ActiveDocument.GetSurfaceIds().Count > 1
                ? C3DApp.SelectSurface()
                : GetSurfaceByIndex(tr, 0);

            return surface;
        }

        /// <summary>
        /// Calculates a point near the surface edge and finds it's elevation.
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="point">The picked point.</param>
        /// from the given position (in meters).</param>
        /// <param name="calculatedPoint">The calculated point.</param>
        /// <param name="edge"></param>
        private static void FindPointNearSurface(TinSurface surface, Point3d point, out Point3d calculatedPoint, out LineSegment2d edge)
        {
            try // Check if point is in surface.
            {
                edge = null;
                double elevation = surface.FindElevationAtXY(point.X, point.Y);
                calculatedPoint = new Point3d(point.X, point.Y, elevation);
                return;
            }
            catch (PointNotOnEntityException)
            {
                // Suppress error
                // Point is not in surface bounds
            }

            var closestDistance = double.MaxValue;
            LineSegment2d closestEdge = null;

            foreach (var triangle in surface.Triangles)
            {
                var line1 = new LineSegment2d(triangle.Edge1.Vertex1.Location.ToPoint2d(), triangle.Edge1.Vertex2.Location.ToPoint2d());
                var line2 = new LineSegment2d(triangle.Edge2.Vertex1.Location.ToPoint2d(), triangle.Edge2.Vertex2.Location.ToPoint2d());
                var line3 = new LineSegment2d(triangle.Edge3.Vertex1.Location.ToPoint2d(), triangle.Edge3.Vertex2.Location.ToPoint2d());

                double d1 = line1.GetDistanceTo(point.ToPoint2d());
                double d2 = line2.GetDistanceTo(point.ToPoint2d());
                double d3 = line3.GetDistanceTo(point.ToPoint2d());

                double distance = d1;
                if (d2 < distance)
                    distance = d2;

                if (d3 < distance)
                    distance = d3;

                if (distance >= closestDistance)
                    continue;

                if (Math.Abs(distance - d1) < TOLERANCE)
                    closestEdge = line1;

                if (Math.Abs(distance - d2) < TOLERANCE)
                    closestEdge = line2;

                if (Math.Abs(distance - d3) < TOLERANCE)
                    closestEdge = line3;

                closestDistance = distance;
            }

            if (closestEdge == null)
            {
                throw new InvalidOperationException("closestEdge was null.");
            }

            var p = closestEdge.GetClosestPointTo(point.ToPoint2d()).Point.ToPoint3d();
            double el = surface.FindElevationAtXY(p.X, p.Y);
            calculatedPoint = new Point3d(p.X, p.Y, el);
            edge = closestEdge;
        }

        /// <summary>
        /// Finds the elevation of a point near or on a <see cref="TinSurface"/>.
        /// </summary>
        /// <param name="surface">The surface to find the elevation on.</param>
        /// <param name="x">X value of point.</param>
        /// <param name="y">Y value of point.</param>
        /// <param name="maxInterpolateDistance"></param>
        /// <returns>System.Double.</returns>
        public static double FindElevationNearSurface(TinSurface surface, double x, double y, double maxInterpolateDistance = 0.0d)
        {
            FindPointNearSurface(surface, new Point3d(x, y, 0), out Point3d calculatedPoint, out _);

            if (PointHelpers.GetDistanceBetweenPoints(new Point(x, y), calculatedPoint.ToPoint()) > maxInterpolateDistance)
            {
                return 0;
            }

            return calculatedPoint.Z;
        }

        /// <summary>
        /// Finds the elevation of a point near or on a <see cref="TinSurface"/>.
        /// </summary>
        /// <param name="surface">The surface to find the elevation on.</param>
        /// <param name="x">X value of point.</param>
        /// <param name="y">Y value of point.</param>
        /// <param name="dX">The delta X between the point and calculated point.</param>
        /// <param name="dY">The delta Y between the point and calculated point.</param>
        /// <returns>A double representing the elevation nearest the surface.</returns>
        public static double FindElevationNearSurface(TinSurface surface, double x, double y, out double dX, out double dY)
        {
            FindPointNearSurface(surface, new Point3d(x, y, 0), out Point3d calculatedPoint, out _);

            dX = x - calculatedPoint.X;
            dY = y - calculatedPoint.Y;

            return calculatedPoint.Z;
        }

        /// <summary>
        /// Finds the elevation on surface at a given position.
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <returns>System.Double.</returns>
        public static double FindElevationOnSurface(TinSurface surface, double x, double y)
        {
            try
            {
                return surface.FindElevationAtXY(x, y);
            }
            catch (PointNotOnEntityException)
            {
                return 0;
            }
        }

        /// <summary>
        /// The SPPointElevationsFromSurfaces command allows the user to show point tables with the elevations from 2 surfaces, in addition to the point elevation.
        /// After starting the SPPointElevationsFromSurfaces command, you will be presented with a form from which you select the points, or PointGroups, to compare,
        /// select the 2 surfaces to use, and the 2 UserDefinedProperties(these must be pre-defined as elevation types).
        /// Once the selection is complete the selected points will have the respective UDP's assigned the surface elevations. You can now assign a label style to
        /// the points which displays those UDP's, use the DisplayPoints Sincpac tool to create a report, or export the points out to a text file. If you need to
        /// also include station/offset information, use the DL_Points tool to link the points to alignment(s).
        /// </summary>
        public static void PointElevationsFromSurface()
        {
            // This will have to be a service.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a selection set points above or below the selected surface.
        /// </summary>
        /// <remarks>
        /// Note that you may not see the selection on screen as gripped objects, as the number of
        /// gripped objects is limited by the AutoCAD system variable GRIPOBJLIMIT.
        /// </remarks>
        public static void SelectPointsAboveOrBelowSurface()
        {
            using (var tr = AcadApp.StartTransaction())
            {
                var surface = SelectSurface(tr);

                if (surface == null)
                {
                    tr.Commit();
                    return;
                }

                var selectionObjectIds = new ObjectIdCollection();

                // Prompt for above or below surface.
                var pko = new PromptKeywordOptions("\n3DS> Select points above or below surface: ");
                pko.Keywords.Add("Above");
                pko.Keywords.Add("Below");
                pko.AllowNone = true;

                var pkr = AcadApp.Editor.GetKeywords(pko);

                EditorUtils.TryGetDouble("\n3DS> Tolerance: ", out double tolerance, true, 0.001);

                if (pkr.Status != PromptStatus.OK && string.IsNullOrEmpty(pkr.StringResult))
                {
                    tr.Commit();
                    return;
                }

                foreach (var cogoPointId in C3DApp.ActiveDocument.CogoPoints)
                {
                    var cogoPoint = tr.GetObject(cogoPointId, OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint == null)
                        continue;

                    try
                    {
                        double elevationAtXy = surface.FindElevationAtXY(cogoPoint.Easting, cogoPoint.Northing);

                        if (pkr.StringResult == "Above" && cogoPoint.Elevation > elevationAtXy
                                                        && cogoPoint.Elevation - elevationAtXy > tolerance)
                        {
                            selectionObjectIds.Add(cogoPointId);
                        }

                        if (pkr.StringResult == "Below" && cogoPoint.Elevation < elevationAtXy
                                                        && elevationAtXy - cogoPoint.Elevation > tolerance)
                        {
                            selectionObjectIds.Add(cogoPointId);
                        }
                    }
                    catch (PointNotOnEntityException e)
                    {
                        AcadApp.Editor.WriteMessage($"\n3DS> {e.Message} X:{cogoPoint.Easting},Y:{cogoPoint.Northing}");
                    }
                }
                AcadApp.Editor.SetImpliedSelection(selectionObjectIds.ToArray());
                tr.Commit();
            }
        }

        /// <summary>
        /// Selects a <see cref="CivilSurface"/> from the active document.
        /// </summary>
        /// <returns>A <see cref="CivilSurface"/>.</returns>
        public static CivilSurface SelectCivilSurface()
        {
            if (!EditorUtils.TryGetEntityOfType<TinSurface>("\n3DS> Select Surface: ",
                    "\n3DS> Select Surface: ", out var objectId))
                return null;

            CivilSurface surface;

            using (var tr = AcadApp.StartLockedTransaction())
            {
                surface = GetSurfaceByObjectId(tr, objectId).ToCivilSurface();
                tr.Commit();
            }

            return surface;
        }

        /// <summary>
        /// Extension method to convert a <see cref="TinSurface"/> to a <see cref="CivilSurface"/>.
        /// </summary>
        /// <param name="surface">The <see cref="TinSurface"/> to convert.</param>
        /// <returns>A <see cref="CivilSurface"/>.</returns>
        public static CivilSurface ToCivilSurface(this TinSurface surface)
        {
            return new CivilSurface
            {
                ObjectId = surface.ObjectId.Handle.ToString(),
                Name = surface.Name,
                Description = surface.Description,
                MinimumElevation = surface.GetGeneralProperties().MinimumElevation,
                MaximumElevation = surface.GetGeneralProperties().MaximumElevation
            };
        }

        /// <summary>
        /// Converts a <see cref="CivilSurface"/> to a <see cref="TinSurface"/>.
        /// </summary>
        /// <param name="surface">The <see cref="CivilSurface"/> to convert.</param>
        /// <param name="tr">The active <see cref="Transaction"/>.</param>
        /// <returns>A <see cref="TinSurface"/>.</returns>
        public static TinSurface ToTinSurface(this CivilSurface surface, Transaction tr)
        {
            Handle h = new Handle(long.Parse(surface.ObjectId, NumberStyles.AllowHexSpecifier));
            AcadApp.ActiveDatabase.TryGetObjectId(h, out var id);//TryGetObjectId method

            return tr.GetObject(id, OpenMode.ForRead) as TinSurface;
        }
    }
}