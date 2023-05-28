using CivilSurveySuite.ACAD;
using Autodesk.AutoCAD.Runtime;
using CivilSurveySuite.Common.Helpers;
using CivilSurveySuite.UI.Views;

[assembly: CommandClass(typeof(Commands))]
namespace CivilSurveySuite.ACAD
{
    public class Commands
    {
        #region Debug/Testing

        [CommandMethod("WMS", "CSSTestingCommand", CommandFlags.Modal)]
        public static void Test()
        {
            CommandHelpers.ExecuteCommand<TestingCommand>();
        }

        [CommandMethod("WMS", "CSSTestTransientGraphics", CommandFlags.Modal)]
        public static void TestTransientGraphics()
        {
            CommandHelpers.ExecuteCommand<TransientGraphicsTestCommand>();
        }

        [CommandMethod("WMS", "CSSTestTransientGraphicsUndo", CommandFlags.Modal)]
        public static void TestTransientGraphicsUndo()
        {
            CommandHelpers.ExecuteCommand<TransientGraphicsUndoTestCommand>();
        }

        #endregion

        #region Menus
        [CommandMethod("WMS", "CSSLoadAcadMenu", CommandFlags.Modal)]
        public static void LoadMenu()
        {
            AcadApp.LoadCuiFile(AcadApp.ACAD_TOOLBAR_FILE);
        }
        #endregion

        #region Help Commands

        // DEBUG
        [CommandMethod("WMS", "CSSShowDebug", CommandFlags.Modal)]
        public static void ShowDebug()
        {
            CommandHelpers.ExecuteCommand<ShowDebugCommand>(AcadApp.Logger);
        }

        // HELP
        [CommandMethod("WMS", "CSSShowHelp", CommandFlags.Modal)]
        public static void ShowHelp()
        {
            CommandHelpers.ExecuteCommand<ShowHelpCommand>(AcadApp.Logger);
        }

        #endregion

        #region Point Commands
        // Points
        [CommandMethod("WMS", "CSSPtProdDist", CommandFlags.Modal)]
        public static void PtProdDist()
        {
            PointUtils.Create_At_Production_Of_Line_And_Distance(PointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSPtOffsetLn", CommandFlags.Modal)]
        public static void PtOffsetLn()
        {
            PointUtils.Create_At_Offset_Two_Lines(PointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSPtBrgDist", CommandFlags.Modal)]
        public static void PtBrgDist()
        {
            PointUtils.Create_At_Angle_And_Distance(PointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSPtIntBrg", CommandFlags.Modal)]
        public static void PtIntBrg()
        {
            PointUtils.Create_At_Intersection_Two_Bearings(PointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSPtIntDist", CommandFlags.Modal)]
        public static void PtIntDist()
        {
            PointUtils.Create_At_Intersection_Two_Distances(PointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSPtIntBrd", CommandFlags.Modal)]
        public static void PtIntBearingDist()
        {
            PointUtils.Create_At_Intersection_Of_Angle_And_Distance(PointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSPtBetweenPts", CommandFlags.Modal)]
        public static void PtBetweenPts()
        {
            PointUtils.Create_Between_Points(PointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSPtOffsetBetweenPts", CommandFlags.Modal)]
        public static void PtOffsetBetweenPts()
        {
            PointUtils.Create_At_Offset_Between_Points(PointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSPtDistSlope", CommandFlags.Modal)]
        public static void PtDistSlope()
        {
            PointUtils.Create_At_Angle_Distance_And_Slope(PointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSPtIntFour", CommandFlags.Modal)]
        public static void PtIntFour()
        {
            PointUtils.Create_At_Intersection_Of_Four_Points(PointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSPtSlope", CommandFlags.Modal)]
        public static void PtSlope()
        {
            PointUtils.Create_At_Slope_At_Point(PointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSPtLine", CommandFlags.Modal)]
        public static void PtLine()
        {
            PointUtils.Create_At_Distance_Between_Points(PointUtils.CreatePoint);
        }
        #endregion

        #region Line Commands

        // Lines
        [CommandMethod("WMS", "CSSLnDrawLeg", CommandFlags.Modal)]
        public static void LnDrawLeg()
        {
            LineUtils.Draw_Leg_Line();
        }

        [CommandMethod("WMS", "CSSMidPointsBetweenPolys", CommandFlags.Modal)]
        public static void MidPointsBetweenPolys()
        {
            PolylineUtils.MidPointBetweenPolylines(PointUtils.CreatePoint);
        }
        #endregion

        #region Utilities
        // Utils

        [CommandMethod("WMS", "CSSInverse", CommandFlags.Modal)]
        public static void Inverse()
        {
            PointUtils.Inverse_Pick();
        }

        [CommandMethod("WMS", "CSSInverseOS", CommandFlags.Modal)]
        public static void InverseDisplay()
        {
            PointUtils.Inverse_Pick_Display();
        }

        [CommandMethod("WMS", "CSSInverseChOff", CommandFlags.Modal)]
        public static void InverseChainageOffset()
        {
            PointUtils.Inverse_Pick_Perpendicular();
        }

        [CommandMethod("WMS", "CSSPtLabelIns", CommandFlags.Modal)]
        public static void PtAtLabelIns()
        {
            PointUtils.Create_At_Label_Location(PointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSPtLabelInsText", CommandFlags.Modal)]
        public static void PtAtLabelInsText()
        {
            PointUtils.Create_At_Label_Location(PointUtils.CreatePoint, true);
        }

        [CommandMethod("WMS", "CSSInsertRaster", CommandFlags.Modal)]
        public static void InsertRasterImg()
        {
            AcadApp.ShowDialog<ImageManagerView>();
        }


        [CommandMethod("WMS", "CSSTraverse", CommandFlags.Modal)]
        public static void Traverse()
        {
            CommandHelpers.ExecuteCommand<TraverseCommand>();
        }


        #endregion

        #region Text Commands

        // Text commands
        [CommandMethod("WMS", "CSSTxtUpper", CommandFlags.Modal)]
        public static void TextToUpper()
        {
            TextUtils.TextToUpper();
        }

        [CommandMethod("WMS", "CSSTxtLower", CommandFlags.Modal)]
        public static void TextToLower()
        {
            TextUtils.TextToLower();
        }

        [CommandMethod("WMS", "CSSTxtSentence", CommandFlags.Modal)]
        public static void TextToSentence()
        {
            TextUtils.TextToSentence();
        }

        [CommandMethod("WMS", "CSSTxtPrefix", CommandFlags.Modal)]
        public static void TextPrefix()
        {
            TextUtils.AddPrefixToText();
        }

        [CommandMethod("WMS", "CSSTxtSuffix", CommandFlags.Modal)]
        public static void TextSuffix()
        {
            TextUtils.AddSuffixToText();
        }

        [CommandMethod("WMS", "CSSTxtRmvAlpha", CommandFlags.Modal)]
        public static void TextRemoveAlpha()
        {
            TextUtils.RemoveAlphaCharactersFromText();
        }

        [CommandMethod("WMS", "CSSTxtMathAdd", CommandFlags.Modal)]
        public static void TextMathAdd()
        {
            TextUtils.AddNumberToText();
        }

        [CommandMethod("WMS", "CSSTxtMathSub", CommandFlags.Modal)]
        public static void TextMathSub()
        {
            TextUtils.SubtractNumberFromText();
        }

        [CommandMethod("WMS", "CSSTxtMathMult", CommandFlags.Modal)]
        public static void TextMathMultiply()
        {
            TextUtils.MultiplyTextByNumber();
        }

        [CommandMethod("WMS", "CSSTxtMathDiv", CommandFlags.Modal)]
        public static void TextMathDivide()
        {
            TextUtils.DivideTextByNumber();
        }

        [CommandMethod("WMS", "CSSTxtRound", CommandFlags.Modal)]
        public static void TextRound()
        {
            TextUtils.RoundTextDecimalPlaces();
        }

        #endregion

        #region Palettes/Windows

        [CommandMethod("WMS", "CSSShowAngleCalculator", CommandFlags.Modal)]
        public void ShowAngleCalculator()
        {
            AcadApp.ShowModelessDialog<AngleCalculatorView>();
        }

        [CommandMethod("WMS", "CSSShowTraverseWindow", CommandFlags.Modal)]
        public void ShowTraversePalette()
        {
            AcadApp.ShowDialog<TraverseView>();
        }

        [CommandMethod("WMS", "CSSShowTraverseAngleWindow", CommandFlags.Modal)]
        public void ShowTraverseAngle()
        {
            AcadApp.ShowDialog<TraverseAngleView>();
        }

        #endregion
    }
}