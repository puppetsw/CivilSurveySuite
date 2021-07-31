// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.ACAD2017.AcadUtils;
using _3DS_CivilSurveySuite.C3D2017.C3DUtils;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017.Commands
{
    public class CogoPointDescription
    {
        [CommandMethod("3DS", "_3DSCPRawDescriptionToUpper", CommandFlags.Modal)]
        public void CogoPoint_RawDescription_ToUpper()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;
            
            using (Transaction tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in pso.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    CogoPoints.FullDescriptionToUpperCase(ref pt);
                    pt.DowngradeOpen();
                }

                tr.Commit();
            }
        }

        [CommandMethod("3DS", "_3DSCPFullDescriptionToUpper", CommandFlags.Modal)]
        public void CogoPoint_FullDescription_ToUpper()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;
            
            using (Transaction tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in pso.Value.GetObjectIds())
                {
                    CogoPoint pt = (CogoPoint)objectId.GetObject(OpenMode.ForWrite);
                    CogoPoints.RawDescriptionToUpperCase(ref pt);
                    pt.DowngradeOpen();
                }

                tr.Commit();
            }
        }
    }
}