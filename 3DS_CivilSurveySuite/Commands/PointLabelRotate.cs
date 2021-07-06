using _3DS_CivilSurveySuite_ACADBase21;
using _3DS_CivilSurveySuite_C3DBase21;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Commands.PointLabelRotate))]
namespace _3DS_CivilSurveySuite.Commands
{
    public class PointLabelRotate
    {
        /// <summary>
        /// Rotate CogoPoint label to match line or polyline
        /// </summary>
        [CommandMethod("3DSPointLabelRotate")]
        public void PointLabelRotateCommand()
        {
            //Select Point or points
            PromptSelectionOptions psoPoints = new PromptSelectionOptions
            {
                MessageForAdding = "\n3DS> Select points: ",
                MessageForRemoval = "\n3DS> Remove points: "
            };
            TypedValue[] pointsFilter = { new TypedValue((int) DxfCode.Start, "AECC_COGO_POINT") };
            SelectionFilter ssPoints = new SelectionFilter(pointsFilter);
            PromptSelectionResult psrPoints = AutoCADApplicationManager.Editor.GetSelection(psoPoints, ssPoints);

            //SELECT line, polyline or 3D Polyline
            var peoLines = new PromptEntityOptions("\n3DS> Select line, polyline or 3Dpolyline");
            peoLines.SetRejectMessage("\n3DS> Select line, polyline or 3Dpolyline only");
            peoLines.AddAllowedClass(typeof(Polyline3d), true);
            peoLines.AddAllowedClass(typeof(Polyline), true);
            peoLines.AddAllowedClass(typeof(Polyline2d), true);
            peoLines.AddAllowedClass(typeof(Line), true);
            PromptEntityResult perLines = AutoCADApplicationManager.Editor.GetEntity(peoLines);

            if (psrPoints.Value == null) return;

            using (Transaction tr = AutoCADApplicationManager.StartTransaction())
            {
                double angle = 0;
                double textAngle = 0;

                switch (perLines.ObjectId.ObjectClass.DxfName)
                {
                    case "POLYLINE":
                    case "LWPOLYLINE":
                        Polyline poly = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline;
                        if (poly != null)
                        {
                            angle = Polylines.GetPolylineSegmentAngle(poly, perLines.PickedPoint);
                        }
                        else
                        {
                            Polyline3d poly3d = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline3d;
                            if (poly3d != null)
                            {
                                angle = Polylines.GetPolyline3dSegmentAngle(poly3d, perLines.PickedPoint);
                            }
                        }

                        break;
                    case "LINE":
                        var line = (Line) perLines.ObjectId.GetObject(OpenMode.ForRead);
                        angle = line.Angle;
                        break;
                }

                AutoCADApplicationManager.Editor.WriteMessage("Polyline segment angle (radians): " + angle);

                foreach (ObjectId id in psrPoints.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint) id.GetObject(OpenMode.ForRead);
                    LabelStyle style = pt.LabelStyleId.GetObject(OpenMode.ForRead) as LabelStyle;
                    textAngle = Labels.GetLabelStyleComponentAngle(style); //gets the current cogopoints label style rotation from first text component

                    AutoCADApplicationManager.Editor.WriteMessage($"Point label style current rotation (radians): {textAngle}");
                    AutoCADApplicationManager.Editor.WriteMessage($"Rotating label to {angle} to match polyline segment");

                    pt.UpgradeOpen();
                    pt.LabelRotation = 0;
                    pt.ResetLabelLocation();
                    pt.LabelRotation -= textAngle;
                    pt.LabelRotation += angle;
                    pt.DowngradeOpen();
                }

                tr.Commit();
            }
        }
    }
}