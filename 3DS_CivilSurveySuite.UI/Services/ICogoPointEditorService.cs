// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.UI.Services
{
    public interface ICogoPointEditorService
    {
        void ZoomTo(CivilPoint civilPoint);

        void Select(CivilPoint civilPoint);

        void Update(CivilPoint civilPoint);

        void UpdateSelected(IEnumerable<CivilPoint> civilPoints, string propertyName, string value);

        IEnumerable<CivilPoint> GetPoints();

    }
}