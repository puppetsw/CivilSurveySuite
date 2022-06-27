using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Shared.Helpers;
using _3DS_CivilSurveySuite.UI.Views;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.C3D2017.Commands))]
namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class Commands
    {
        [CommandMethod("3DS", "_3DSTestFeatureLineCurve", CommandFlags.Modal)]
        public static void TestCommand()
        {
            CommandHelpers.ExecuteCommand<TestFeatureLineCurveCommand>();
        }

        [CommandMethod("3DS", "_3DSTestFeatureLineSite", CommandFlags.Modal)]
        public static void TestCommand1()
        {
            EditorUtils.TryGetEntityOfType<FeatureLine>("", "", out var objectId);

            using (var tr = AcadApp.StartTransaction())
            {
                var fl = (FeatureLine)tr.GetObject(objectId, OpenMode.ForRead);

                var o = ObjectId.Null;
                var id = fl.SiteId;

                AcadApp.WriteMessage(id.ToString());

                tr.Commit();
            }
        }

        #region CogoPoints
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

        [CommandMethod("3DS", "_3DSCptMidBetweenPoly", CommandFlags.Modal)]
        public static void CptMidBetweenPoly()
        {
            PolylineUtils.MidPointBetweenPolylines(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptSlope", CommandFlags.Modal)]
        public static void PtSlope()
        {
            PointUtils.Create_At_Slope_At_Point(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptLabelIns", CommandFlags.Modal)]
        public static void CptAtLabelIns()
        {
            PointUtils.Create_At_Label_Location(CogoPointUtils.CreatePoint);
        }

        [CommandMethod("3DS", "_3DSCptLabelInsText", CommandFlags.Modal)]
        public static void CptAtLabelInsText()
        {
            PointUtils.Create_At_Label_Location(CogoPointUtils.CreatePoint, true);
        }

        [CommandMethod("3DS", "_3DSCptLabelsReset", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void CptResetLabels()
        {
            CogoPointUtils.LabelResetSelection();
        }

        [CommandMethod("3DS", "_3DSCptLabelsMove", CommandFlags.Modal)]
        public static void CptMoveLabels()
        {
            C3DApp.ShowDialog<CogoPointMoveLabelView>();
        }

        [CommandMethod("3DS", "_3DSRawDesUpper", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void RawDesUpper()
        {
            CogoPointUtils.RawDescriptionToUpper();
        }

        [CommandMethod("3DS", "_3DSFullDesUpper", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void FullDesUpper()
        {
            CogoPointUtils.DescriptionFormatToUpper();
        }

        [CommandMethod("3DS", "_3DSCptMatchLblRot", CommandFlags.Modal)]
        public static void CptMatchLblRot()
        {
            CogoPointUtils.LabelRotateMatch();
        }

        [CommandMethod("3DS", "_3DSCptMatchMrkRot", CommandFlags.Modal)]
        public static void CptMatchMrkRot()
        {
            CogoPointUtils.MarkerRotateMatch();
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

        [CommandMethod("3DS", "_3DSCptUsedPts", CommandFlags.Modal)]
        public static void UsedPts()
        {
            CogoPointUtils.UsedPoints();
        }

        [CommandMethod("3DS", "_3DSCptSetNext", CommandFlags.Modal)]
        public static void CptSetNextPointNumber()
        {
            CogoPointUtils.SetNextPointNumber();
        }

        [CommandMethod("3DS", "_3DSCptScaleElevations", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public static void CptScaleElevations()
        {
            CogoPointUtils.ScaleElevations();
        }

        [CommandMethod("3DS", "_3DSCptFullDescriptionToText", CommandFlags.Modal)]
        public static void CptFullDescriptionToText()
        {
            CogoPointUtils.FullDescriptionToTextEntity();
        }

        [CommandMethod("3DS", "_3DSCptRawDescriptionToText", CommandFlags.Modal)]
        public static void CptRawDescriptionToText()
        {
            CogoPointUtils.RawDescriptionToTextEntity();
        }

        #endregion

        #region Surfaces
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
        #endregion

        #region Palettes

        // Palettes
        [CommandMethod("3DS", "3DSShowConnectLineworkWindow", CommandFlags.Modal)]
        public static void ShowConnectLinework()
        {
            C3DApp.ShowDialog<ConnectLineworkView>();
        }

        [CommandMethod("3DS", "3DSShowCogoPointEditor", CommandFlags.Modal)]
        public static void ShowCogoPointEditor()
        {
            C3DApp.ShowDialog<CogoPointEditorView>();
        }

        [CommandMethod("3DS", "3DSShowCogoPointFindReplace", CommandFlags.Modal)]
        public static void ShowCogoFindReplace()
        {
            C3DApp.ShowDialog<CogoPointReplaceDuplicateView>();
        }

        [CommandMethod("3DS", "3DSShowCogoPointReporter", CommandFlags.Modal)]
        public static void ShowCogoReporter()
        {
            C3DApp.ShowDialog<CogoPointSurfaceReportView>();
        }

        #endregion

        #region Labels

        // Labels
        [CommandMethod("3DS", "3DSLabelMaskOff", CommandFlags.Modal)]
        public static void LabelMaskOff()
        {
            CogoPointUtils.LabelMaskToggle(false);
        }

        [CommandMethod("3DS", "3DSLabelMaskOn", CommandFlags.Modal)]
        public static void LabelMaskOn()
        {
            CogoPointUtils.LabelMaskToggle(true);
        }

        [CommandMethod("3DS", "3DSLabelLineBreak", CommandFlags.Modal)]
        public static void LabelLineBreak()
        {
            CogoPointUtils.AddLineBreakToDescription();
        }

        [CommandMethod("3DS", "_3DSLabelStack", CommandFlags.Modal)]
        public static void StackLabels()
        {
            CogoPointUtils.LabelStack();
        }

        [CommandMethod("3DS", "_3DSLabelOverride", CommandFlags.Modal)]
        public static void OverrideText()
        {
            LabelUtils.OverrideTextLabel();
        }

        #endregion
    }
}