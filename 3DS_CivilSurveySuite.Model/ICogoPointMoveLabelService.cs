using System;

namespace _3DS_CivilSurveySuite.Model
{
    public interface ICogoPointMoveLabelService
    {
        Vector MoveDifference { get; set; }

        Vector GetMoveDifference();
    }
}
