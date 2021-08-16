// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.ACAD2017;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.C3D2017.Commands))]
namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class Commands
    {
        [CommandMethod("3DS", "_3DSCptBrgDist", CommandFlags.Modal)]
        public static void PtBrgDist()
        {
            PointUtils.Create_At_Angle_And_Distance(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptProdDist", CommandFlags.Modal)]
        public static void PtProdDist()
        {
            PointUtils.Create_At_Production_Of_Line_And_Distance(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptOffsetLn", CommandFlags.Modal)]
        public static void PtOffsetLn()
        {
            PointUtils.Create_At_Offset_Two_Lines(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptIntBrg", CommandFlags.Modal)]
        public static void PtIntBrg()
        {
            PointUtils.Create_At_Intersection_Two_Bearings(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptIntDist", CommandFlags.Modal)]
        public static void PtIntDist()
        {
            PointUtils.Create_At_Intersection_Two_Distances(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptIntBrd", CommandFlags.Modal)]
        public static void PtIntBearingDist()
        {
            PointUtils.Create_At_Intersection_Of_Angle_And_Distance(CogoPointUtils.CreatePoint);
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


        [CommandMethod("3DS", "_3DSZoomToPt", CommandFlags.Modal)]
        public static void ZoomToPt()
        {
            CogoPointUtils.ZoomPoint();
        }



        [CommandMethod("3DS", "_3DSCptUsedPts", CommandFlags.Modal)]
        public static void UsedPts()
        {
            CogoPointUtils.UsedPoints();
        }


        // Palettes
        [CommandMethod("3DS", "3DSShowConnectLinePalette", CommandFlags.Modal)]
        public static void ShowConnectLinePalette()
        {
            C3DPalettes.ShowConnectLinePalette();
        }
    }
}