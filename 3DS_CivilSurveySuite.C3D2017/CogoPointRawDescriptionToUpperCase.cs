// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.ACAD2017;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class CogoPointRawDescriptionToUpperCase
    {
        public static void RunCommand()
        {
            var pso = EditorUtils.GetEntities<CogoPoint>("\n3DS> Select points: ", "\n3DS> Remove points: ");

            if (pso.Status != PromptStatus.OK)
                return;
            
            using (Transaction tr = AcadUtils.StartTransaction())
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
    }
}