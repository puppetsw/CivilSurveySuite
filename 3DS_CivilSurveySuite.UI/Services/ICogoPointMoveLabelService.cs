using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.UI.Services
{
    public interface ICogoPointMoveLabelService
    {
        Vector MoveDifference { get; set; }

        Vector GetMoveDifference();
    }
}
