// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite_ACADBase21;
using _3DS_CivilSurveySuite_C3DBase21;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.Commands
{
    public static class CogoPointCreateTrunkAtTree
    {
        public static void RunCommand()
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
    }
}