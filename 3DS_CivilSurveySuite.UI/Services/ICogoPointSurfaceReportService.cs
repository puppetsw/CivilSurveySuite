// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
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

        IEnumerable<CivilPoint> GetPointsInPointGroup(CivilPointGroup pointGroup);

        double GetElevationAtCivilPoint(CivilPoint civilPoint, CivilSurface civilSurface,
            bool calculatePointNearSurfaceEdge, out double dX, out double dY);

        StationOffset GetStationOffsetAtCivilPoint(CivilPoint civilPoint, CivilAlignment civilAlignment);
    }
}