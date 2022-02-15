// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.UI.Services
{
    public interface ICogoPointSurfaceReportService
    {
        IEnumerable<CivilSite> GetSites();

        IEnumerable<CivilAlignment> GetSiteAlignments(CivilSite site);

        IEnumerable<CivilAlignment> GetAlignments();

        IEnumerable<CivilPointGroup> GetPointGroups();

        IEnumerable<CivilSurface> GetSurfaces();

        CivilAlignment SelectAlignment();

        CivilPointGroup SelectPointGroup();

        CivilSurface SelectSurface();

        Task<ObservableCollection<ReportObject>> GetReportData(CivilPointGroup pointGroup, CivilAlignment alignment,
            CivilSurface surface, bool calculatePointNearSurfaceEdge);
    }
}