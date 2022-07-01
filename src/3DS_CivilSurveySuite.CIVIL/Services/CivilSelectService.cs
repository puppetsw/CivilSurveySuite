using System.Collections.Generic;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;

namespace _3DS_CivilSurveySuite.CIVIL.Services
{
    public class CivilSelectService : ICivilSelectService
    {
        public IEnumerable<CivilAlignment> GetAlignments()
        {
            return AlignmentUtils.GetAlignments().ToListOfCivilAlignments();
        }

        public CivilAlignment SelectAlignment()
        {
            return AlignmentUtils.SelectCivilAlignment();
        }

        public IEnumerable<CivilAlignment> GetSiteAlignments(CivilSite site)
        {
            return AlignmentUtils.GetCivilAlignmentsInCivilSite(site);
        }

        public IEnumerable<CivilSite> GetSites()
        {
            return SiteUtils.GetCivilSites();
        }

        public IEnumerable<CivilPointGroup> GetPointGroups()
        {
            return PointGroupUtils.GetPointGroups().ToListOfCivilPointGroups();
        }

        public IEnumerable<CivilSurface> GetSurfaces()
        {
            return SurfaceUtils.GetCivilSurfaces();
        }

        public CivilSurface SelectSurface()
        {
            return SurfaceUtils.SelectCivilSurface();
        }

        public CivilPointGroup SelectPointGroup()
        {
            return PointGroupUtils.SelectCivilPointGroup();
        }
    }
}
