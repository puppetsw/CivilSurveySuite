// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class StyleUtils
    {
        public static void ChangeStyle(CogoPoint cogoPoint, PointStyle pointStyle, bool applyDescriptionKey = false)
        {
            cogoPoint.StyleId = pointStyle.Id;

            if (applyDescriptionKey)
                cogoPoint.ApplyDescriptionKeys();

            //AcadApp.Editor.UpdateScreen(); //UNDONE: Check if this is needed.
        }
    }
}