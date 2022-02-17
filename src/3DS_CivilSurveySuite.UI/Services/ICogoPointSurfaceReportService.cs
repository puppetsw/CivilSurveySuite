// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.UI.Models;

namespace _3DS_CivilSurveySuite.UI.Services
{
    public interface ICogoPointSurfaceReportService
    {
        Task<ObservableCollection<ReportObject>> GetReportData(CivilPointGroup pointGroup, CivilAlignment alignment,
            CivilSurface surface, bool calculatePointNearSurfaceEdge);
    }
}