namespace _3DS_CivilSurveySuite.C3D2017.C3DUtils
{
    class Surfaces
    {
        /// <summary>
        /// Adds the selected breakline(s) to a surface
        /// </summary>
        void AddBreaklineToSurface()
        { }

        /// <summary>
        /// The RemoveBreaklineFromSurface command allows the user to remove selected objects from the Surface definition to which it is connected. This command is
        /// only available in Sincpac C3D version 3 and newer.
        /// After starting the RemoveBreaklineFromSurface command, you will be prompted to select breaklines.You may select objects that are not breaklines, they will
        /// be ignored during processing. Should a breakline be encountered which is used in more than one surface, you will be prompted to choose which surface(s)
        /// you want it to be removed from. The surface will be rebuilt upon completion of the command.
        /// </summary>
        void RemoveBreaklineFromSurface()
        { }

        /// <summary>
        /// The SPPointElevationsFromSurfaces command allows the user to show point tables with the elevations from 2 surfaces, in addition to the point elevation.
        /// After starting the SPPointElevationsFromSurfaces command, you will be presented with a form from which you select the points, or PointGroups, to compare, 
        /// select the 2 surfaces to use, and the 2 UserDefinedProperties(these must be pre-defined as elevation types).
        /// Once the selection is complete the selected points will have the respective UDP's assigned the surface elevations. You can now assign a label style to 
        /// the points which displays those UDP's, use the DisplayPoints Sincpac tool to create a report, or export the points out to a text file. If you need to 
        /// also include station/offset information, use the DL_Points tool to link the points to alignment(s).
        /// </summary>
        void PointElevationsFromSurface()
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
        void SelectPointsAboveOrBelowSurface()
        { }
    }
}
