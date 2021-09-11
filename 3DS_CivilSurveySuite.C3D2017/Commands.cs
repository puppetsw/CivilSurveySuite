// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.UI.Views;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.C3D2017.Commands))]
namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class Commands
    {
        [CommandMethod("3DS", "_3DSCptBrgDist", CommandFlags.Modal)]
        public static void CptBrgDist()
        {
            PointUtils.Create_At_Angle_And_Distance(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptProdDist", CommandFlags.Modal)]
        public static void CptProdDist()
        {
            PointUtils.Create_At_Production_Of_Line_And_Distance(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptOffsetLn", CommandFlags.Modal)]
        public static void CptOffsetLn()
        {
            PointUtils.Create_At_Offset_Two_Lines(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptIntBrg", CommandFlags.Modal)]
        public static void CptIntBrg()
        {
            PointUtils.Create_At_Intersection_Two_Bearings(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptIntDist", CommandFlags.Modal)]
        public static void CptIntDist()
        {
            PointUtils.Create_At_Intersection_Two_Distances(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptIntBrd", CommandFlags.Modal)]
        public static void CptIntBearingDist()
        {
            PointUtils.Create_At_Intersection_Of_Angle_And_Distance(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptIntFour", CommandFlags.Modal)]
        public static void CptIntFourPoint()
        {
            PointUtils.Create_At_Intersection_Of_Four_Points(CogoPointUtils.CreatePoint);
        }


        [CommandMethod("3DS", "_3DSCptLabelIns", CommandFlags.Modal)]
        public static void CptAtLabelIns()
        {
            PointUtils.Create_At_Label_Location(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptLabelsReset", CommandFlags.Modal)]
        public static void CptResetLabels()
        {
            CogoPointUtils.Label_Reset_All();
        }

        [CommandMethod("3DS", "_3DSCptLabelsMove", CommandFlags.Modal)]
        public static void CptMoveLabels()
        {
            C3DService.ShowDialog<CogoPointMoveLabelView>();
        }



        [CommandMethod("3DS", "_3DSCptTrunkAtTrees", CommandFlags.Modal)]
        public static void CptTrunkAtTrees()
        {
            CogoPointUtils.Create_Trunks_At_Trees();
        }


        [CommandMethod("3DS", "_3DSRawDesUpper", CommandFlags.Modal)]
        public static void RawDesUpper()
        {
            CogoPointUtils.RawDescription_ToUpper();
        }


        [CommandMethod("3DS", "_3DSFullDesUpper", CommandFlags.Modal)]
        public static void FullDesUpper()
        {
            CogoPointUtils.FullDescription_ToUpper();
        }


        [CommandMethod("3DS", "_3DSCptMatchLblRot", CommandFlags.Modal)]
        public static void CptMatchLblRot()
        {
            CogoPointUtils.Label_Rotate_Match();
        }


        [CommandMethod("3DS", "_3DSZoomToCpt", CommandFlags.Modal)]
        public static void ZoomToPt()
        {
            CogoPointUtils.ZoomPoint();
        }


        [CommandMethod("3DS", "_3DSCptInverse", CommandFlags.Modal)]
        public static void InverseCogoPoint()
        {
            CogoPointUtils.Inverse_ByPointNumber();
        }



        [CommandMethod("3DS", "_3DSStackLabels", CommandFlags.Modal)]
        public static void StackLabels()
        {
            CogoPointUtils.Stack_CogoPoint_Labels();
        }



        [CommandMethod("3DS", "_3DSCptUsedPts", CommandFlags.Modal)]
        public static void UsedPts()
        {
            CogoPointUtils.UsedPoints();
        }

        [CommandMethod("3DS", "_3DSCptSetNext", CommandFlags.Modal)]
        public static void CptSetNextPointNumber()
        {
            CogoPointUtils.Set_Next_PointNumber();
        }


        // Surfaces
        [CommandMethod("3DS", "_3DSSurfaceElAtPt", CommandFlags.Modal)]
        public static void SurfaceElevationAtPoint()
        {
            SurfaceUtils.GetSurfaceElevationAtPoint();
        }

        [CommandMethod("3DS", "3DSSurfaceAddBreaklines", CommandFlags.Modal)]
        public static void SurfaceAddBreaklines()
        {
            SurfaceUtils.AddBreaklineToSurface();
        }


        [CommandMethod("3DS", "3DSSurfaceRemoveBreaklines", CommandFlags.Modal)]
        public static void SurfaceRemoveBreaklines()
        {
            SurfaceUtils.RemoveBreaklinesFromSurface();
        }

        [CommandMethod("3DS", "3DSSurfaceSelAboveBelow", CommandFlags.Modal)]
        public static void SurfaceSelectAboveOrBelow()
        {
            SurfaceUtils.SelectPointsAboveOrBelowSurface();
        }



        // Palettes
        [CommandMethod("3DS", "3DSShowConnectLineworkWindow", CommandFlags.Modal)]
        public static void ShowConnectLinework()
        {
            C3DService.ShowWindow<ConnectLineworkView>();
        }

        [CommandMethod("3DS", "3DSShowCogoPointEditor", CommandFlags.Modal)]
        public static void ShowCogoPointEditor()
        {
            C3DService.ShowWindow<CogoPointEditorView>();
        }


        // Labels
        [CommandMethod("3DS", "3DSLabelMaskOff", CommandFlags.Modal)]
        public static void LabelMaskOff()
        {
            CogoPointUtils.Label_Mask_Toggle(false);
        }

        [CommandMethod("3DS", "3DSLabelMaskOn", CommandFlags.Modal)]
        public static void LabelMaskOn()
        {
            CogoPointUtils.Label_Mask_Toggle(true);
        }


        
        [CommandMethod("3DS", "3DSTest", CommandFlags.Modal)]
        public static void Test()
        {
            //AcadApp.Editor.WriteMessage($"Point Group Name: {C3DService.PointGroupSelect().GetPointGroup().Name}");

            //C3DService.SelectSurface();
            var pg = C3DService.SelectPointGroup();


        }



    }
}