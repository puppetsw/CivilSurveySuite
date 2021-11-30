// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.UI.Views;
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

        [CommandMethod("3DS", "_3DSPtOffsetBetweenPts", CommandFlags.Modal)]
        public static void PtOffsetBetweenPts()
        {
            PointUtils.Create_At_Offset_Between_Points(PointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSPtDistSlope", CommandFlags.Modal)]
        public static void PtDistSlope()
        {
            PointUtils.Create_At_Angle_Distance_And_Slope(PointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSPtIntFour", CommandFlags.Modal)]
        public static void PtIntFour()
        {
            PointUtils.Create_At_Intersection_Of_Four_Points(PointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSPtSlope", CommandFlags.Modal)]
        public static void PtSlope()
        {
            PointUtils.Create_At_Slope_At_Point(PointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSPtLine", CommandFlags.Modal)]
        public static void PtLine()
        {
            PointUtils.Create_At_Distance_Between_Points(PointUtils.CreatePoint);
        }




        // Lines
        [CommandMethod("3DS", "_3DSLnDrawLeg", CommandFlags.Modal)]
        public static void LnDrawLeg()
        {
            LineUtils.Draw_Leg_Line();
        }

        [CommandMethod("3DS", "_3DSMidPointsBetweenPolys", CommandFlags.Modal)]
        public static void MidPointsBetweenPolys()
        {
            PolylineUtils.MidPointBetweenPolylines(PointUtils.CreatePoint);
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


        [CommandMethod("3DS", "_3DSPtLabelIns", CommandFlags.Modal)]
        public static void PtAtLabelIns()
        {
            PointUtils.Create_At_Label_Location(PointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSPtLabelInsText", CommandFlags.Modal)]
        public static void PtAtLabelInsText()
        {
            PointUtils.Create_At_Label_Location(PointUtils.CreatePoint, true);
        }





        [CommandMethod("3DS", "_3DSTraverse", CommandFlags.Modal)]
        public static void Traverse()
        {
            TraverseUtils.Traverse();
        }


        // Text commands
        [CommandMethod("3DS", "_3DSTxtUpper", CommandFlags.Modal)]
        public static void TextToUpper()
        {
            TextUtils.TextToUpper();
        }

        [CommandMethod("3DS", "_3DSTxtLower", CommandFlags.Modal)]
        public static void TextToLower()
        {
            TextUtils.TextToLower();
        }

        [CommandMethod("3DS", "_3DSTxtSentence", CommandFlags.Modal)]
        public static void TextToSentence()
        {
            TextUtils.TextToSentence();
        }

        [CommandMethod("3DS", "_3DSTxtPrefix", CommandFlags.Modal)]
        public static void TextPrefix()
        {
            TextUtils.AddPrefixToText();
        }

        [CommandMethod("3DS", "_3DSTxtSuffix", CommandFlags.Modal)]
        public static void TextSuffix()
        {
            TextUtils.AddSuffixToText();
        }

        [CommandMethod("3DS", "_3DSTxtRmvAlpha", CommandFlags.Modal)]
        public static void TextRemoveAlpha()
        {
            TextUtils.RemoveAlphaCharactersFromText();
        }

        [CommandMethod("3DS", "_3DSTxtMathAdd", CommandFlags.Modal)]
        public static void TextMathAdd()
        {
            TextUtils.AddNumberToText();
        }

        [CommandMethod("3DS", "_3DSTxtMathSub", CommandFlags.Modal)]
        public static void TextMathSub()
        {
            TextUtils.SubtractNumberFromText();
        }

        [CommandMethod("3DS", "_3DSTxtMathMult", CommandFlags.Modal)]
        public static void TextMathMultiply()
        {
            TextUtils.MultiplyTextByNumber();
        }

        [CommandMethod("3DS", "_3DSTxtMathDiv", CommandFlags.Modal)]
        public static void TextMathDivide()
        {
            TextUtils.DivideTextByNumber();
        }






        // Palettes/Windows
        [CommandMethod("3DS", "_3DSShowAngleCalculator", CommandFlags.Modal)]
        public void ShowAngleCalculator()
        {
            AcadService.ShowModelessDialog<AngleCalculatorView>();
        }

        [CommandMethod("3DS", "_3DSShowTraverseWindow", CommandFlags.Modal)]
        public void ShowTraversePalette()
        {
            AcadService.ShowDialog<TraverseView>();
        }

            [CommandMethod("3DS", "_3DSShowTraverseAngleWindow", CommandFlags.Modal)]
        public void ShowTraverseAngle()
        {
            AcadService.ShowDialog<TraverseAngleView>();
        }


    }
}