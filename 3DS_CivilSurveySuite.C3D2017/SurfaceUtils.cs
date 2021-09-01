// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Reflection;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;
using Surface = Autodesk.Civil.DatabaseServices.Surface;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class SurfaceUtils
    {
        /// <summary>
        /// Gets a <see cref="Surface"/> by name.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="surfaceName">Name of the surface.</param>
        /// <returns><see cref="TinSurface"/>.</returns>
        /// <exception cref="ArgumentNullException">tr</exception>
        /// <exception cref="ArgumentException">surfaceName</exception>
        public static TinSurface GetSurfaceByName(Transaction tr, string surfaceName)
        {
            if (tr == null) 
                throw new ArgumentNullException(nameof(tr));

            if (string.IsNullOrEmpty(surfaceName))
                throw new ArgumentException(@"surfaceName was null or empty", nameof(surfaceName));

            var surfaceIds = C3DApp.ActiveDocument.GetSurfaceIds();
            TinSurface surface = null;

            foreach (ObjectId surfaceId in surfaceIds)
            {
                var s = tr.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;

                if (s.Name == surfaceName)
                {
                    surface = s;
                    break;
                }
            }

            return surface;
        }

        /// <summary>
        /// Gets a <see cref="Surface"/> by index.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="index">The index of the surface</param>
        /// <returns><see cref="TinSurface"/>.</returns>
        /// <exception cref="ArgumentNullException">tr</exception>
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
        /// <param name="tr">The transaction.</param>
        /// <param name="objectId">The object identifier.</param>
        /// <returns>TinSurface.</returns>
        /// <exception cref="ArgumentNullException">tr</exception>
        /// <exception cref="ArgumentNullException">objectId</exception>
        public static TinSurface GetSurfaceByObjectId(Transaction tr, ObjectId objectId)
        {
            if (tr == null) 
                throw new ArgumentNullException(nameof(tr));

            if (objectId.IsNull)
                throw new ArgumentNullException(nameof(objectId));

            return tr.GetObject(objectId, OpenMode.ForRead) as TinSurface;
        }


        /// <summary>
        /// Gets the names of the surfaces in the active drawing.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        [Obsolete]
        public static IEnumerable<string> GetSurfaceNames(Transaction tr)
        {
            var surfaceIds = C3DApp.ActiveDocument.GetSurfaceIds();
            var surfaceNames = new List<string>();

            foreach (ObjectId surfaceId in surfaceIds)
            {
                var surface = tr.GetObject(surfaceId, OpenMode.ForRead) as Surface;
                surfaceNames.Add(surface.Name);
            }

            return surfaceNames;
        }

        /// <summary>
        /// Gets the surfaces in the drawing as <see cref="CivilSurface"/> objects.
        /// </summary>
        /// <param name="tr">The transaction.</param>
        /// <returns>IEnumerable&lt;CivilSurface&gt;.</returns>
        [Obsolete("This methid is oboslete, use SurfaceExtensions.", true)]
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

        [Obsolete("This methid is oboslete, use SurfaceExtensions.", true)]
        public static CivilSurface GetCivilSurface(Transaction tr, ObjectId objectId)
        {
            var surface = tr.GetObject(objectId, OpenMode.ForRead) as TinSurface;
            return new CivilSurface { ObjectId = surface.ObjectId.Handle.ToString(), Name = surface.Name, Description = surface.Description };
        }



        /// <summary>
        /// Gets the surface elevation at picked point.
        /// </summary>
        public static void GetSurfaceElevationAtPoint()
        {
            if (!EditorUtils.GetPoint(out Point3d pickedPoint, "\n3DS> Pick point: "))
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
            if (!EditorUtils.GetSelectionOfType<Line, Polyline, Polyline3d>(out var objectIds, "\n3DS> Select breaklines to add: "))
                return;

            // Which surface?
            using (var tr = AcadApp.StartTransaction())
            {
                TinSurface surface = null;
                var surfaces = C3DApp.ActiveDocument.GetSurfaceIds();

                // Check if objects are in more than one surface?
                if (surfaces.Count > 1)
                {
                    var surfaceSelectService = C3DServiceFactory.GetSurfaceSelectService();
                    if (surfaceSelectService.ShowDialog())
                    {
                        surface = GetSurfaceByName(tr, surfaceSelectService.Surface.Name);
                    }
                }
                else
                {
                    // Get first surface.
                    surface = GetSurfaceByIndex(tr, 0);
                }

                if (surface == null)
                    return;

                var breaklineDefs = surface.BreaklinesDefinition;

                // Load defaults
                var midOrd = CommandSettings.GetDefaultBreaklineMidOrdinateDistance;
                var maxDist = CommandSettings.GetDefaultBreaklineSupplementingDistance;
                var weedDist = CommandSettings.GetDefaultBreaklineWeedingDistance;
                var weedAngle = CommandSettings.GetDefaultBreaklineWeedingAngle;

                breaklineDefs.AddStandardBreaklines(objectIds, midOrd, maxDist, weedDist, weedAngle);

                surface.Rebuild();
                tr.Commit();
            }

            AcadApp.Editor.Regen();
            AcadApp.Editor.UpdateScreen(); // Probably not needed.
        }
    
        /// <summary>
        /// Removes the selected breakline(s) from the selected surface.
        /// </summary>
        public static void RemoveBreaklinesFromSurface()
        {
            if (!EditorUtils.GetSelectionOfType<Line, Polyline, Polyline3d>(out var objectIds, "\n3DS> Select breaklines to remove: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                TinSurface surface = null;

                if (C3DApp.ActiveDocument.GetSurfaceIds().Count > 1)
                {
                    var surfaceSelectService = C3DServiceFactory.GetSurfaceSelectService();
                    if (surfaceSelectService.ShowDialog())
                    {
                        surface = GetSurfaceByName(tr, surfaceSelectService.Surface.Name);
                    }
                }
                else
                {
                    surface = GetSurfaceByIndex(tr, 0);
                }
                
                if (surface == null)
                    return;

                var breaklineDefs = surface.BreaklinesDefinition;
                var breaklinesToBeRemoved = new List<int>();

                for (int i = 0; i < breaklineDefs.Count; i++)
                {
                    var breaklineSet = breaklineDefs[i];

                    // Store current breakline details so we can re-create them.
                    var midOrd = breaklineSet.MidOrdinateDistance;
                    var maxDist = breaklineSet.MaximumDistance;
                    var weedDist = breaklineSet.WeedingDistance;
                    var weedAngle = breaklineSet.WeedingAngle;
                    var description = breaklineSet.Description;

                    var breaklineIds = surface.GetBreaklineEntityIds(breaklineSet);

                    for (int j = 0; j < breaklineIds.Count; j++)
                    {
                        var curbl = breaklineIds[j];
                        foreach (ObjectId objectId in objectIds)
                        {
                            if (breaklineIds[j].Handle == objectId.Handle)
                                breaklinesToBeRemoved.Add(j); //Add index of handle match to remove list
                        }

                        foreach (int breaklineIndex in breaklinesToBeRemoved)
                        {
                            breaklineIds.RemoveAt(breaklineIndex);
                        }

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

            ObjectIdCollection result = new ObjectIdCollection();

            object tinsurf = surf.AcadObject;
            object breaklines = tinsurf.GetType().InvokeMember("Breaklines", BindingFlags.GetProperty, null, tinsurf, null);
            int breaklineCount = (int)breaklines.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, breaklines, null);

            object[] args = new object[1];

            for (int j = 0; j < breaklineCount; j++)
            {
                args[0] = j;
                object breakline = breaklines.GetType().InvokeMember("Item", BindingFlags.InvokeMethod, null, breaklines, args);
                string desc = (string)breakline.GetType().InvokeMember("Description", BindingFlags.GetProperty, null, breakline, null);
                
                if (desc != name) 
                    continue;

                object[] entities = (object[])breakline.GetType().InvokeMember("BreaklineEntities", BindingFlags.GetProperty, null, breakline, null);
                for (int i = 0; i < entities.GetLength(0); i++)
                {
                    ObjectId id = DBObject.FromAcadObject(entities[i]);
                    result.Add(id);
                }
            }
            return result;
        }






        /// <summary>
        /// The SPPointElevationsFromSurfaces command allows the user to show point tables with the elevations from 2 surfaces, in addition to the point elevation.
        /// After starting the SPPointElevationsFromSurfaces command, you will be presented with a form from which you select the points, or PointGroups, to compare, 
        /// select the 2 surfaces to use, and the 2 UserDefinedProperties(these must be pre-defined as elevation types).
        /// Once the selection is complete the selected points will have the respective UDP's assigned the surface elevations. You can now assign a label style to 
        /// the points which displays those UDP's, use the DisplayPoints Sincpac tool to create a report, or export the points out to a text file. If you need to 
        /// also include station/offset information, use the DL_Points tool to link the points to alignment(s).
        /// </summary>
        static void PointElevationsFromSurface()
        { }

        /// <summary>
        /// The SelectPointsAboveOrBelowSurface command allows the user to create a selection of points lying either Above or Below a chosen Surface.
        /// Usage
        /// Type SelectPointsAboveOrBelowSurface at the command line.You will be presented with a form to choose which Surface to use, which method - Above or Below - 
        /// to use, and to optionally limit the selection to points in one or more Point Groups.Make the desired selections, press OK, and the corresponding Points
        /// will become selected.You can choose to have your selections saved for the next use of this command.
        /// As command line alternatives, there are the 2 commands SelectPointsAboveSurface and SelectPointsBelowSurface which will operate on all points and just
        /// present you with a small form with which to choose the Surface.
        /// Note that you may not see the selection on screen as gripped objects, as the number of gripped objects is limited by the AutoCAD Sysvar GRIPOBJLIMIT.
        /// </summary>
        static void SelectPointsAboveOrBelowSurface()
        { }
    }
}
