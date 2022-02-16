// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class SelectSurfaceService : ISelectSurfaceService
    {
        public IEnumerable<CivilSurface> GetSurfaces()
        {
            return SurfaceUtils.GetCivilSurfaces();
        }

        public CivilSurface SelectSurface()
        {
            return SurfaceUtils.SelectCivilSurface();
        }
    }
}