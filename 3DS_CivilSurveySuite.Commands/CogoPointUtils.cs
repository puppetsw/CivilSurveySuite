// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite_ACADBase21;
using _3DS_CivilSurveySuite_C3DBase21;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Commands.CogoPointUtils))]
namespace _3DS_CivilSurveySuite.Commands
{
    public class CogoPointUtils
    {
        [CommandMethod("3DS", "_3DSCogoPointLabelRotate", CommandFlags.Modal)]
        public void CogoPointLabelRotate() //BUG: Need to make it so rotation is always correct way up.
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;

            var perLines = EditorUtils.GetEntity(new[] { typeof(Line), typeof(Polyline), typeof(Polyline2d), typeof(Polyline3d) }, "\n3DS> Select line or polyline.");

            if (perLines.Status != PromptStatus.OK)
                return;
            
            string lineType = perLines.ObjectId.ObjectClass.DxfName;
            using (Transaction tr = AutoCADActive.StartTransaction())
            {
                double angle = 0;

                switch (lineType)
                {
                    case DxfNames.LWPOLYLINE:
                    case DxfNames.POLYLINE:
                        var poly = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline;
                        if (poly != null)
                        {
                            angle = Polylines.GetPolylineSegmentAngle(poly, perLines.PickedPoint);
                            break;
                        }

                        var poly3d = perLines.ObjectId.GetObject(OpenMode.ForRead) as Polyline3d;
                        if (poly3d != null)
                        {
                            angle = Polylines.GetPolyline3dSegmentAngle(poly3d, perLines.PickedPoint);
                        }

                        break;
                    case DxfNames.LINE:
                        var line = (Line) perLines.ObjectId.GetObject(OpenMode.ForRead);
                        angle = line.Angle;
                        break;
                }

                AutoCADActive.Editor.WriteMessage("Polyline segment angle (radians): " + angle);

                foreach (ObjectId id in pso.Value.GetObjectIds())
                {
                    var pt = (CogoPoint) id.GetObject(OpenMode.ForRead);
                    var style = pt.LabelStyleId.GetObject(OpenMode.ForRead) as LabelStyle;
                    double textAngle = Labels.GetLabelStyleComponentAngle(style);

                    AutoCADActive.Editor.WriteMessage($"Point label style current rotation (radians): {textAngle}");
                    AutoCADActive.Editor.WriteMessage($"Rotating label to {angle} to match polyline segment");

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

        [CommandMethod("3DS", "_3DSCogoPointRawDescriptionToUpperCase", CommandFlags.Modal)]
        public void CogoPointRawDescriptionToUpper()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;
            
            using (Transaction tr = AutoCADActive.StartTransaction())
            {
                foreach (ObjectId objectId in pso.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    CogoPoints.RawDescriptionToUpperCase(ref pt);
                    pt.DowngradeOpen(); // Don't leave point in write mode?
                }

                tr.Commit();
            }
        }

        [CommandMethod("3DS", "_3DSCogoPointFullDescriptionToUpperCase", CommandFlags.Modal)]
        public void CogoPointFullDescriptionToUpper()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;
            
            using (Transaction tr = AutoCADActive.StartTransaction())
            {
                foreach (ObjectId objectId in pso.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    CogoPoints.FullDescriptionToUpperCase(ref pt);
                    pt.DowngradeOpen(); // Don't leave point in write mode?
                }

                tr.Commit();
            }
        }

        [CommandMethod("3DS", "_3DSCreateTrunkPointAtTrees", CommandFlags.Modal)]
        public void CogoPointCreateTrunksAtTrees()
        {
            //TODO: Use settings to determine codes for TRNK and TRE
            //TODO: Add option to set style for tree and trunk?
            var counter = 0;

            using (Transaction tr = AutoCADActive.StartTransaction())
            {
                foreach (ObjectId pointId in CivilActive.ActiveCivilDocument.CogoPoints)
                {
                    var cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint is null) 
                        continue;

                    if (!cogoPoint.RawDescription.Contains("TRE ")) 
                        continue;
                    
                    ObjectId trunkPointId = CivilActive.ActiveCivilDocument.CogoPoints.Add(cogoPoint.Location, true);
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
            AutoCADActive.Editor.WriteMessage(completeMessage);

        }

        [CommandMethod("3DS", "_3DSCreateCogoPointAtSelectedBearingAndDistance", CommandFlags.Modal)]
        public void CogoPoint_Create_At_Selected_Bearing_And_Distance()
        {

        }
    }
}