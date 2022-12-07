using _3DS_CivilSurveySuite.ACAD;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.CIVIL
{
    public class FlattenFeatureLineCommand : IAcadCommand
    {
        public void Execute()
        {
            if (!EditorUtils.TryGetImpliedSelectionOfType<FeatureLine>(out var featureLines))
            {
                if (!EditorUtils.TryGetSelectionOfType<FeatureLine>("\n3DS> Select Feature Line: ", "\n3DS> Remove Feature Line: ", out featureLines))
                {
                    return;
                }
            }

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId featureLineId in featureLines)
                {
                    var featureLine = (FeatureLine)tr.GetObject(featureLineId, OpenMode.ForWrite);
                    FeatureLineUtils.FlattenFeatureLine(featureLine);
                }

                tr.Commit();
            }
        }
    }
}
