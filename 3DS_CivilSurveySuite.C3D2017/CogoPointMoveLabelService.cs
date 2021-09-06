using System;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Views;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public class CogoPointMoveLabelService : ICogoPointMoveLabelService
    {
        public Vector MoveDifference { get; set; }

        public Vector GetMoveDifference()
        {
            bool? dialogResult = C3DService.ShowDialog<CogoPointMoveLabelView>();

            if (dialogResult != null && dialogResult.Value)
            {
                return MoveDifference;
            }

            return null;
        }
    }
}
