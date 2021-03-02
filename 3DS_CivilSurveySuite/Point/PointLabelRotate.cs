using System.Collections.Generic;
// AutoCAD References
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
// Civil 3D References
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Point.PointLabelRotate))]
namespace _3DS_CivilSurveySuite.Point
{
    public class PointLabelRotate : CivilBase
    {
        private struct PolylineVertex
        {
            public double Distance;
            public Point2d Point;
            public int Index;
        }

        /// <summary>
        /// Gets the angle of the segment closest to the picked point from a polyline
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="pickedPoint"></param>
        /// <returns></returns>
        private double GetPolylineSegmentAngle(Polyline polyline, Point3d pickedPoint)
        {
            int segmentStart = 0;

            var closestPoint = polyline.GetClosestPointTo(pickedPoint, false);
            var len = polyline.GetDistAtPoint(closestPoint);

            for (int i = 1; i < polyline.NumberOfVertices - 1; i++)
            {
                var pt1 = polyline.GetPoint3dAt(i);
                var l1 = polyline.GetDistAtPoint(pt1);

                var pt2 = polyline.GetPoint3dAt(i + 1);
                var l2 = polyline.GetDistAtPoint(pt2);

                if (len > l1 && len < l2)
                {
                    segmentStart = i;
                    break;
                }
            }

            var segment = polyline.GetLineSegment2dAt(segmentStart);
            return segment.Direction.Angle;
        }

        /// <summary>
        /// Gets the angle of the segment closest to the picked point from a 3d polyline
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="pickedPoint"></param>
        /// <returns>angle of line segment as double</returns>
        private double GetPolyline3dSegmentAngle(Polyline3d polyline, Point3d pickedPoint)
        {
            var vertexList = new List<PolylineVertex>();
            for (int i = 0; i < polyline.EndParam; i++)
            {
                var pt3d = polyline.GetPointAtParameter(i);
                var pt2d = new Point2d(pt3d.X, pt3d.Y);
                var pickedPt = new Point2d(pickedPoint.X, pickedPoint.Y);
                var distance = pickedPt.GetDistanceTo(pt2d);
                vertexList.Add(new PolylineVertex() { Point = pt2d, Distance = distance, Index = i });
            }

            vertexList.Sort((x, y) => x.Distance.CompareTo(y.Distance)); //sort by distance to point

            //get the line segment
            //HACK: maybe check if it's the last vertex?
            var vert1 = vertexList[0];
            var vert23d = polyline.GetPointAtParameter(vert1.Index + 1); //get second point
            var vert2 = new Point2d(vert23d.X, vert23d.Y);

            var lineSegment = new LineSegment2d(vert1.Point, vert2);
            return lineSegment.Direction.Angle;

        }

        /// <summary>
        /// Gets the angle of the first text component in the style
        /// </summary>
        /// <returns>angle of line segment as double</returns>
        private double GetLabelStyleComponentAngle(LabelStyle style)
        {
            foreach (ObjectId componentId in style.GetComponents(LabelStyleComponentType.Text))
            {
                LabelStyleComponent component = componentId.GetObject(OpenMode.ForRead) as LabelStyleComponent;

                if (component.GetType() == typeof(LabelStyleTextComponent))
                {
                    var textComponent = component as LabelStyleTextComponent;
                    return textComponent.Text.Angle.Value;
                }
            }
            return 0;
        }

        //1. Select Point or Points
        //2. Select Line or Polyline segment, 3d poly converted to 2d
        //3. Done
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
            TypedValue[] pointsFilter = { new TypedValue((int)DxfCode.Start, "AECC_COGO_POINT") };
            SelectionFilter ssPoints = new SelectionFilter(pointsFilter);
            PromptSelectionResult psrPoints = Editor.GetSelection(psoPoints, ssPoints);

            //SELECT line, polyline or 3D Polyline
            PromptEntityOptions peoLines = new PromptEntityOptions("\n3DS> Select line, polyline or 3Dpolyline");
            peoLines.SetRejectMessage("\n3DS> Select line, polyline or 3Dpolyline only");
            peoLines.AddAllowedClass(typeof(Polyline3d), true);
            peoLines.AddAllowedClass(typeof(Polyline), true);
            peoLines.AddAllowedClass(typeof(Polyline2d), true);
            peoLines.AddAllowedClass(typeof(Line), true);
            PromptEntityResult perLines = Editor.GetEntity(peoLines);

            if (psrPoints.Value == null) return;

            using (Transaction tr = startTransaction())
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
                            angle = GetPolylineSegmentAngle(poly, perLines.PickedPoint);
                        }
                        else
                        {
                            Polyline3d poly3d = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline3d;
                            if (poly3d != null)
                            {
                                angle = GetPolyline3dSegmentAngle(poly3d, perLines.PickedPoint);
                            }
                        }
                        break;
                    case "LINE":
                        var line = (Line)perLines.ObjectId.GetObject(OpenMode.ForRead);
                        angle = line.Angle;
                        break;
                }

                WriteMessage("Polyline segment angle (radians): " + angle);

                foreach (ObjectId id in psrPoints.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)id.GetObject(OpenMode.ForRead);
                    LabelStyle style = pt.LabelStyleId.GetObject(OpenMode.ForRead) as LabelStyle;
                    textAngle = GetLabelStyleComponentAngle(style); //gets the current cogopoints label style rotation from first text component

                    WriteMessage("Point label style current rotation (radians): " + textAngle);
                    WriteMessage("Rotating label to " + angle + " to match polyline segment");

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
