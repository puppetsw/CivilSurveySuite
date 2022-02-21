// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.UI.Models;

namespace _3DS_CivilSurveySuite.UI.Services
{
    public interface ICogoPointSurfaceReportService
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="ICogoPointSurfaceReportService"/> should use
        /// surface interpolation for points that fall outside the defined surface area.
        /// </summary>
        /// <value>If <c>true</c> interpolation is used on <see cref="CivilPoint"/>s.</value>
        /// <remarks>
        /// Use <see cref="MaximumInterpolationDistance"/> to set the maximum distance allowed for interpolation.
        /// </remarks>
        bool Interpolate { get; set; }

        /// <summary>
        /// Gets or sets the maximum interpolation distance.
        /// </summary>
        /// <value>The maximum interpolation distance.</value>
        double MaximumInterpolationDistance { get; set; }

        Task<ObservableCollection<ReportObject>> GetReportData(CivilAlignment alignment,
            IEnumerable<CivilPointGroup> pointGroups, IEnumerable<CivilSurface> surfaces);
    }
}