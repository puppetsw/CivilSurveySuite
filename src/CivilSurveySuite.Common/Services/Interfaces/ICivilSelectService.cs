using System.Collections.Generic;
using CivilSurveySuite.Shared.Models;

namespace CivilSurveySuite.Shared.Services.Interfaces
{
    public interface ICivilSelectService
    {
        IEnumerable<CivilSite> GetSites();

        IEnumerable<CivilAlignment> GetAlignments();

        CivilAlignment SelectAlignment();

        IEnumerable<CivilAlignment> GetSiteAlignments(CivilSite site);

        IEnumerable<CivilSurface> GetSurfaces();

        CivilSurface SelectSurface();

        IEnumerable<CivilPointGroup> GetPointGroups();

        CivilPointGroup SelectPointGroup();
    }
}
