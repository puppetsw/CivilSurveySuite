using Autodesk.AutoCAD.Runtime;
using CivilSurveySuite.ACAD;
using CivilSurveySuite.Shared.Helpers;
using CivilSurveySuite.UI.Views;

[assembly: CommandClass(typeof(CivilSurveySuite.CIVIL.Commands))]
namespace CivilSurveySuite.CIVIL
{
    public static class Commands
    {
        #region Menus

        [CommandMethod("WMS", "CSSLoadCivilMenu", CommandFlags.Modal)]
        public static void LoadMenu()
        {
            C3DApp.LoadMenu();
        }

        #endregion

        #region CogoPoints
        [CommandMethod("WMS", "CSSCptBrgDist", CommandFlags.Modal)]
        public static void CptBrgDist()
        {
            PointUtils.Create_At_Angle_And_Distance(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSCptProdDist", CommandFlags.Modal)]
        public static void CptProdDist()
        {
            PointUtils.Create_At_Production_Of_Line_And_Distance(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSCptOffsetLn", CommandFlags.Modal)]
        public static void CptOffsetLn()
        {
            PointUtils.Create_At_Offset_Two_Lines(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSCptIntBrg", CommandFlags.Modal)]
        public static void CptIntBrg()
        {
            PointUtils.Create_At_Intersection_Two_Bearings(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSCptIntDist", CommandFlags.Modal)]
        public static void CptIntDist()
        {
            PointUtils.Create_At_Intersection_Two_Distances(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSCptIntBrd", CommandFlags.Modal)]
        public static void CptIntBearingDist()
        {
            PointUtils.Create_At_Intersection_Of_Angle_And_Distance(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSCptIntFour", CommandFlags.Modal)]
        public static void CptIntFourPoint()
        {
            PointUtils.Create_At_Intersection_Of_Four_Points(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSCptMidBetweenPoly", CommandFlags.Modal)]
        public static void CptMidBetweenPoly()
        {
            PolylineUtils.MidPointBetweenPolylines(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSCptSlope", CommandFlags.Modal)]
        public static void PtSlope()
        {
            PointUtils.Create_At_Slope_At_Point(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSCptLabelIns", CommandFlags.Modal)]
        public static void CptAtLabelIns()
        {
            PointUtils.Create_At_Label_Location(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("WMS", "CSSCptLabelInsText", CommandFlags.Modal)]
        public static void CptAtLabelInsText()
        {
            PointUtils.Create_At_Label_Location(CogoPointUtils.CreatePoint, true);
        }

        [CommandMethod("WMS", "CSSCptLabelsReset", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void CptResetLabels()
        {
            CogoPointUtils.LabelResetSelection();
        }

        [CommandMethod("WMS", "CSSCptLabelsMove", CommandFlags.Modal)]
        public static void CptMoveLabels()
        {
            C3DApp.ShowDialog<CogoPointMoveLabelView>();
        }

        [CommandMethod("WMS", "CSSRawDesUpper", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void RawDesUpper()
        {
            CogoPointUtils.RawDescriptionToUpper();
        }

        [CommandMethod("WMS", "CSSFullDesUpper", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void FullDesUpper()
        {
            CogoPointUtils.DescriptionFormatToUpper();
        }

        [CommandMethod("WMS", "CSSCptMatchLblRot", CommandFlags.Modal)]
        public static void CptMatchLblRot()
        {
            CogoPointUtils.LabelRotateMatch();
        }

        [CommandMethod("WMS", "CSSCptMatchMrkRot", CommandFlags.Modal)]
        public static void CptMatchMrkRot()
        {
            CogoPointUtils.MarkerRotateMatch();
        }

        [CommandMethod("WMS", "CSSZoomToCpt", CommandFlags.Modal)]
        public static void ZoomToPt()
        {
            CogoPointUtils.ZoomPoint();
        }

        [CommandMethod("WMS", "CSSCptInverse", CommandFlags.Modal)]
        public static void InverseCogoPoint()
        {
            CogoPointUtils.Inverse_ByPointNumber();
        }

        [CommandMethod("WMS", "CSSCptUsedPts", CommandFlags.Modal)]
        public static void UsedPts()
        {
            CogoPointUtils.UsedPoints();
        }

        [CommandMethod("WMS", "CSSCptSetNext", CommandFlags.Modal)]
        public static void CptSetNextPointNumber()
        {
            CogoPointUtils.SetNextPointNumber();
        }

        [CommandMethod("WMS", "CSSCptScaleElevations", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void CptScaleElevations()
        {
            CogoPointUtils.ScaleElevations();
        }

        [CommandMethod("WMS", "CSSCptFullDescriptionToText", CommandFlags.Modal)]
        public static void CptFullDescriptionToText()
        {
            CogoPointUtils.FullDescriptionToTextEntity();
        }

        [CommandMethod("WMS", "CSSCptRawDescriptionToText", CommandFlags.Modal)]
        public static void CptRawDescriptionToText()
        {
            CogoPointUtils.RawDescriptionToTextEntity();
        }

        #endregion

        #region Surfaces
        // Surfaces
        [CommandMethod("WMS", "CSSSurfaceElAtPt", CommandFlags.Modal)]
        public static void SurfaceElevationAtPoint()
        {
            SurfaceUtils.GetSurfaceElevationAtPoint();
        }

        [CommandMethod("WMS", "CSSSurfaceAddBreaklines", CommandFlags.Modal)]
        public static void SurfaceAddBreaklines()
        {
            SurfaceUtils.AddBreaklineToSurface();
        }


        [CommandMethod("WMS", "CSSSurfaceRemoveBreaklines", CommandFlags.Modal)]
        public static void SurfaceRemoveBreaklines()
        {
            SurfaceUtils.RemoveBreaklinesFromSurface();
        }

        [CommandMethod("WMS", "CSSSurfaceSelAboveBelow", CommandFlags.Modal)]
        public static void SurfaceSelectAboveOrBelow()
        {
            SurfaceUtils.SelectPointsAboveOrBelowSurface();
        }
        #endregion

        #region Palettes

        // Palettes
        [CommandMethod("WMS", "CSSShowConnectLineworkWindow", CommandFlags.Modal)]
        public static void ShowConnectLinework()
        {
            C3DApp.ShowDialog<ConnectLineworkView>();
        }

        [CommandMethod("WMS", "CSSShowCogoPointEditor", CommandFlags.Modal)]
        public static void ShowCogoPointEditor()
        {
            C3DApp.ShowDialog<CogoPointEditorView>();
        }

        [CommandMethod("WMS", "CSSShowCogoPointFindReplace", CommandFlags.Modal)]
        public static void ShowCogoFindReplace()
        {
            C3DApp.ShowDialog<CogoPointReplaceDuplicateView>();
        }

        [CommandMethod("WMS", "CSSShowCogoPointReporter", CommandFlags.Modal)]
        public static void ShowCogoReporter()
        {
            C3DApp.ShowDialog<CogoPointSurfaceReportView>();
        }

        #endregion

        #region Labels

        // Labels
        [CommandMethod("WMS", "CSSLabelMaskOff", CommandFlags.Modal)]
        public static void LabelMaskOff()
        {
            CogoPointUtils.LabelMaskToggle(false);
        }

        [CommandMethod("WMS", "CSSLabelMaskOn", CommandFlags.Modal)]
        public static void LabelMaskOn()
        {
            CogoPointUtils.LabelMaskToggle(true);
        }

        [CommandMethod("WMS", "CSSLabelLineBreak", CommandFlags.Modal)]
        public static void LabelLineBreak()
        {
            CogoPointUtils.AddLineBreakToDescription();
        }

        [CommandMethod("WMS", "CSSLabelStack", CommandFlags.Modal)]
        public static void StackLabels()
        {
            CogoPointUtils.LabelStack();
        }

        [CommandMethod("WMS", "CSSLabelOverride", CommandFlags.Modal)]
        public static void OverrideText()
        {
            LabelUtils.OverrideTextLabel();
        }

        #endregion

        #region Feature Lines

        [CommandMethod("WMS", "CSSFlattenFeatureLine", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void FlattenFeatureLine()
        {
            CommandHelpers.ExecuteCommand<FlattenFeatureLineCommand>();
        }

        [CommandMethod("WMS", "CSSFeatureLineSite", CommandFlags.Modal)]
        public static void FeatureLineSite()
        {
            CommandHelpers.ExecuteCommand<FeatureLineSiteCommand>();
        }

        #endregion
    }
}
