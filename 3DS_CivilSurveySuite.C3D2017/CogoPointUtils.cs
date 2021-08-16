using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class CogoPointUtils
    {
        public static void CreatePoint(Transaction tr, Point3d position)
        {
            CogoPointCollection cogoPoints = C3DApp.ActiveCivilDocument.CogoPoints;
            var cogoPointId = cogoPoints.Add(position, true);

            if (!EditorUtils.GetString(out string rawDescription, "\n3DS> Enter raw description: "))
                return;

            var cogoPoint = tr.GetObject(cogoPointId, OpenMode.ForWrite) as CogoPoint;
            cogoPoint.RawDescription = rawDescription;
            cogoPoint.DowngradeOpen();
        }

        /// <summary>
        /// Changes a <see cref="CogoPoint"/>'s Draw Description to upper case.
        /// </summary>
        public static void RawDescription_ToUpper()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;
            
            using (Transaction tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in pso.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    pt.RawDescription = pt.RawDescription.ToUpper();
                    pt.DowngradeOpen();
                }

                tr.Commit();
            }
        }

        /// <summary>
        /// Changes a <see cref="CogoPoint"/>'s Full Description to upper case.
        /// </summary>
        public static void FullDescription_ToUpper()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;
            
            using (Transaction tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in pso.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    pt.DescriptionFormat = pt.DescriptionFormat.ToUpper();
                    pt.DowngradeOpen();
                }

                tr.Commit();
            }
        }


        public static void Label_Rotate_Match()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;

            var perLines = EditorUtils.GetEntity(new[] { typeof(Line), typeof(Polyline), typeof(Polyline2d), typeof(Polyline3d) }, "\n3DS> Select line or polyline.");

            if (perLines.Status != PromptStatus.OK)
                return;
            
            string lineType = perLines.ObjectId.ObjectClass.DxfName;
            using (Transaction tr = AcadApp.StartTransaction())
            {
                double angle = 0;

                switch (lineType)
                {
                    case DxfNames.LWPOLYLINE:
                    case DxfNames.POLYLINE:
                        var poly = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline;
                        if (poly != null)
                        {
                            angle = PolylineUtils.GetPolylineSegmentAngle(poly, perLines.PickedPoint);
                            break;
                        }

                        var poly3d = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline3d;
                        if (poly3d != null)
                        {
                            angle = PolylineUtils.GetPolyline3dSegmentAngle(poly3d, perLines.PickedPoint);
                        }

                        break;
                    case DxfNames.LINE:
                        var line = (Line) perLines.ObjectId.GetObject(OpenMode.ForRead);
                        angle = line.Angle;
                        break;
                }

                AcadApp.Editor.WriteMessage("Polyline segment angle (radians): " + angle);

                foreach (ObjectId id in pso.Value.GetObjectIds())
                {
                    var pt = (CogoPoint) id.GetObject(OpenMode.ForRead);
                    var style = pt.LabelStyleId.GetObject(OpenMode.ForRead) as LabelStyle;
                    double textAngle = LabelUtils.GetLabelStyleComponentAngle(style);

                    AcadApp.Editor.WriteMessage($"Point label style current rotation (radians): {textAngle}");
                    AcadApp.Editor.WriteMessage($"Rotating label to {angle} to match polyline segment");

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

        public static void Label_Rotate_Match_All()
        {
            // select a bunch of cogopoints and rotate them based on their position to the selected lines/polylines vertexs

        }


        public static void Create_Trunks_At_Trees()
        {
            //TODO: Use settings to determine codes for TRNK and TRE
            //TODO: Add option to set style for tree and trunk?
            var counter = 0;

            using (Transaction tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId pointId in C3DApp.ActiveCivilDocument.CogoPoints)
                {
                    var cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint is null) 
                        continue;

                    if (!cogoPoint.RawDescription.Contains("TRE ")) 
                        continue;
                    
                    ObjectId trunkPointId = C3DApp.ActiveCivilDocument.CogoPoints.Add(cogoPoint.Location, true);
                    CogoPoint trunkPoint = trunkPointId.GetObject(OpenMode.ForWrite) as CogoPoint;

                    if (trunkPoint != null)
                    {
                        trunkPoint.RawDescription = cogoPoint.RawDescription.Replace("TRE ", "TRNK ");
                        trunkPoint.ApplyDescriptionKeys();

                        cogoPoint.UpgradeOpen();
                        cogoPoint.RawDescription = cogoPoint.RawDescription.Replace("TRE ", "TREE ");
                        cogoPoint.ApplyDescriptionKeys();
                    }
                    counter++;
                }
                tr.Commit();
            }

            string completeMessage = "Changed " + counter + " TRE points, and created " + counter + " TRNK points";
            AcadApp.Editor.WriteMessage(completeMessage);
        }

        /// <summary>
        /// Add multiple points (with interpolated elevation) between two points.
        /// </summary>
        //private void CreatePointBetweenPoints()
        //{
        //}

        /// <summary>
        /// Add multiple points that are offsets of a line defined by two points.
        /// </summary>
        //private void CreatePointBetweenPointsAtOffset()
        //{
        //}

        /// <summary>
        /// Add a point at a picked location with elevation calculated at designated slope.
        /// </summary>
        //private void CreatePointAtLocationWithSlope()
        //{
        //}

        /// <summary>
        /// Create point at extension distance on grade between 2 points.
        /// </summary>
        //private void CreatePointAtExtensionBetweenPoints()
        //{
        //}

      
        /// <summary>
        /// The CreatePointsFromLabels command can be used to create Civil-3D Points at the 
        /// exact location and elevation of Surface Elevation Labels found in a drawing.
        /// </summary>
        //private void CreatePointAtLabel()
        //{
        //}

        /// <summary>
        /// The UsedPt command displays a list of used point numbers in the command window.
        /// Usage
        /// Type UsedPt at the command line.The available point numbers in the drawing are displayed in the 
        /// command window, as in the following example:
        /// </summary>
        public static void UsedPoints()
        {
            using (var tr = AcadApp.StartTransaction())
            {
                var pointNumbers = PointGroupUtils.GroupRange(tr, "_All Points");
                AcadApp.Editor.WriteMessage($"\n3DS> {pointNumbers}");
                tr.Commit();
            }
        }




        /// <summary>
        /// The ZoomPt command zooms the display to the specified point number.
        /// Usage
        /// Type ZoomPt at the command line.You will be prompted to enter either the Number or Name of the CogoPoint to zoom to.
        /// You may also hit ENTER without typing anything to enter a new height factor.The zoom height is determined by taking the current Annotation Scale
        /// and multiplying it by this number.Enter a lower number for the zoom height factor to zoom in closer to the point, or a higher number to zoom out
        /// further. The default zoom height factor is 4.
        /// </summary>
        public static void ZoomPoint()
        {
            using (var tr = AcadApp.StartTransaction())
            {
                var cogoPoints = C3DApp.ActiveCivilDocument.CogoPoints;

                if (!EditorUtils.GetString(out string textInput, "\n3DS> Enter point number: "))
                    return;

                if (!StringHelpers.IsNumeric(textInput))
                    return;

                var zoomPtNum = Convert.ToInt32(textInput);
                CogoPoint zoomPt = null;

                foreach (ObjectId objectId in cogoPoints)
                {
                    var cogoPoint = tr.GetObject(objectId, OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint == null)
                        continue;

                    if (cogoPoint.PointNumber == zoomPtNum)
                    {
                        zoomPt = cogoPoint;
                        break;
                    }
                }

                EditorUtils.ZoomToEntity(zoomPt);

                //SupExtEditor.ActEdt().ZoomCenter(surveyPoint1.Location, Conversions.ToDouble(Application.GetSystemVariable("VIEWSIZE")
                tr.Commit();
            }
        }
    }
}