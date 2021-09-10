using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class CogoPointMoveLabelService : ICogoPointMoveLabelService
    {
        public void MoveLabels(double x, double y)
        {
            CogoPointUtils.Move_CogoPoint_Label(x, y);
        }
    }
}
