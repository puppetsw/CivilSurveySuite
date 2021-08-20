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
        // Points
        [CommandMethod("3DS", "_3DSPtProdDist", CommandFlags.Modal)]
        public static void PtProdDist()
        {
            PointUtils.Create_At_Production_Of_Line_And_Distance(PointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSPtOffsetLn", CommandFlags.Modal)]
        public static void PtOffsetLn()
        {
            PointUtils.Create_At_Offset_Two_Lines(PointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSPtBrgDist", CommandFlags.Modal)]
        public static void PtBrgDist()
        {
            PointUtils.Create_At_Angle_And_Distance(PointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSPtIntBrg", CommandFlags.Modal)]
        public static void PtIntBrg()
        {
            PointUtils.Create_At_Intersection_Two_Bearings(PointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSPtIntDist", CommandFlags.Modal)]
        public static void PtIntDist()
        {
            PointUtils.Create_At_Intersection_Two_Distances(PointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSPtIntBrd", CommandFlags.Modal)]
        public static void PtIntBearingDist()
        {
            PointUtils.Create_At_Intersection_Of_Angle_And_Distance(PointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSPtBetweenPts", CommandFlags.Modal)]
        public static void PtBetweenPts()
        {
            PointUtils.Create_Between_Points(PointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSPtDistSlope", CommandFlags.Modal)]
        public static void PtDistSlope()
        {
            PointUtils.Create_At_Angle_Distance_And_Slope(PointUtils.CreatePoint);
        }


        // Lines
        [CommandMethod("3DS", "_3DSLnDrawLeg", CommandFlags.Modal)]
        public static void LnDrawLeg()
        {
            LineUtils.Draw_Leg_Line();
        }


        // Utils
        [CommandMethod("3DS", "_3DSInverse", CommandFlags.Modal)]
        public static void Inverse()
        {
            PointUtils.Inverse_Pick();
        }

        [CommandMethod("3DS", "_3DSInverseOS", CommandFlags.Modal)]
        public static void InverseDisplay()
        {
            PointUtils.Inverse_Pick_Display();
        }
        
        [CommandMethod("3DS", "_3DSInverseChOff", CommandFlags.Modal)]
        public static void InverseChainageOffset()
        {
            PointUtils.Inverse_Pick_Perpendicular();
        }




        [CommandMethod("3DS", "_3DSTraverse", CommandFlags.Modal)]
        public static void Traverse()
        {
            TraverseUtils.Traverse();
        }




        // Palettes
        [CommandMethod("3DS", "_3DSShowAngleCalculator", CommandFlags.Modal)]
        public static void ShowAngleCalculator()
        {
            AcadPalettes.ShowAngleCalculatorPalette();
        }

        [CommandMethod("3DS", "_3DSShowTraversePalette", CommandFlags.Modal)]
        public static void ShowTraversePalette()
        {
            AcadPalettes.ShowTraversePalette();
        }

        [CommandMethod("3DS", "_3DSShowTraverseAnglePalette", CommandFlags.Modal)]
        public static void ShowTraverseAngle()
        {
            AcadPalettes.ShowTraverseAnglePalette();
        }


    }
}