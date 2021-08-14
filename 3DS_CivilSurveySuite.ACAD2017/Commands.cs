// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.ACAD2017.Commands))]
namespace _3DS_CivilSurveySuite.ACAD2017
{
    public class Commands
    {

        [CommandMethod("3DS", "_3DSPtProdDist", CommandFlags.Modal)]
        public static void PtProdDist()
        {
            PointUtils.Create_At_Production_Of_Line_And_Distance();
        }

        [CommandMethod("3DS", "_3DSPtOffsetLn", CommandFlags.Modal)]
        public static void PtOffsetLn()
        {
            PointUtils.Create_At_Offset_Two_Lines();
        }

        [CommandMethod("3DS", "_3DSPtBrgDist", CommandFlags.Modal)]
        public static void PtBrgDist()
        {
            PointUtils.Create_At_Angle_And_Distance();
        }

        [CommandMethod("3DS", "_3DSPtInverse", CommandFlags.Modal)]
        public static void PtInverse()
        {
            PointUtils.Inverse();
        }

        [CommandMethod("3DS", "_3DSPtInverseDisp", CommandFlags.Modal)]
        public static void PtInverseDisp()
        {
            PointUtils.Inverse_ScreenDisplay();
        }

        [CommandMethod("3DS", "_3DSPtIntBrg", CommandFlags.Modal)]
        public static void PtIntBrg()
        {
            PointUtils.Create_At_Intersection_Two_Bearings();
        }

        [CommandMethod("3DS", "_3DSPtIntDist", CommandFlags.Modal)]
        public static void PtIntDist()
        {
            PointUtils.Create_At_Intersection_Two_Distances();
        }
 





    }
}