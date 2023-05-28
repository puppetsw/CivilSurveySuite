using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CivilSurveySuite.ACAD;
using CivilSurveySuite.Shared.Services.Interfaces;

namespace CivilSurveySuite.CIVIL
{
    public class FlattenFeatureLineCommand : IAcadCommand
    {
        public void Execute()
        {
            if (!EditorUtils.TryGetImpliedSelectionOfType<FeatureLine>(out var featureLines))
            {
                if (!EditorUtils.TryGetSelectionOfType<FeatureLine>("\nSelect Feature Line: ", "\nRemove Feature Line: ", out featureLines))
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
