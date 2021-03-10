using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using _3DS_CivilSurveySuite.Helpers;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Commands.TrunkSize))]
namespace _3DS_CivilSurveySuite.Commands
{
    public class TrunkSize : CivilBase
    {
        /// <summary>
        /// Create a copy of each TRE point and rename it to a TRNK point.
        /// Renames the TRE code to a TREE code.
        /// </summary>
        [CommandMethod("3DSTrunkSize")]
        public void TrunkSizeCommand()
        {
            //TODO: Use settings to determine codes for TRNK and TRE
            int counter = 0;

            using (Transaction tr = startTransaction())
            {
                foreach (ObjectId pointId in Civildoc.CogoPoints)
                {
                    CogoPoint cogoPoint = pointId.GetObject(OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint.RawDescription.Contains("TRE "))
                    {
                        ObjectId trunkPointId = Civildoc.CogoPoints.Add(cogoPoint.Location, true);
                        CogoPoint trunkPoint = trunkPointId.GetObject(OpenMode.ForWrite) as CogoPoint;

                        trunkPoint.RawDescription = cogoPoint.RawDescription.Replace("TRE ", "TRNK ");
                        trunkPoint.ApplyDescriptionKeys();

                        cogoPoint.UpgradeOpen();
                        cogoPoint.RawDescription = cogoPoint.RawDescription.Replace("TRE ", "TREE ");
                        cogoPoint.ApplyDescriptionKeys();

                        counter++;
                    }
                }

                tr.Commit();
            }

            string completeMessage = "Changed " + counter + " TRE points, and created " + counter + " TRNK points";
            WriteMessage(completeMessage);

        }
    }
}
