// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using _3DS_CivilSurveySuite.UI.Models;

namespace _3DS_CivilSurveySuite.UI.Services.Interfaces
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
