// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using _3DS_CivilSurveySuite.ACAD2017;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Surface = Autodesk.Civil.DatabaseServices.Surface;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class SurfaceUtils
    {
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

        public static Surface GetSurfaceByName(string surfaceName)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Adds the selected breakline(s) to a surface
        /// </summary>
        static void AddBreaklineToSurface()
        { }

        /// <summary>
        /// The RemoveBreaklineFromSurface command allows the user to remove selected objects from the Surface definition to which it is connected. This command is
        /// only available in Sincpac C3D version 3 and newer.
        /// After starting the RemoveBreaklineFromSurface command, you will be prompted to select breaklines.You may select objects that are not breaklines, they will
        /// be ignored during processing. Should a breakline be encountered which is used in more than one surface, you will be prompted to choose which surface(s)
        /// you want it to be removed from. The surface will be rebuilt upon completion of the command.
        /// </summary>
        static void RemoveBreaklineFromSurface()
        { }

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
