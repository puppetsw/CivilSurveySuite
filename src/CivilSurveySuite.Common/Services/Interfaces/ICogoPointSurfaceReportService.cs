// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;
using System.Data;
using CivilSurveySuite.Shared.Models;

namespace CivilSurveySuite.Shared.Services.Interfaces
{
    public interface ICogoPointSurfaceReportService
    {
        DataTable DataTable { get; }

        ColumnProperties ColumnProperties { get; }

        bool AllowDuplicatePoints { get; set; }

        bool OutputHeaders { get; set; }

        DelimiterType Delimiter { get; }

        ObservableCollection<ReportCivilSurfaceOptions> CivilSurfaceOptions { get; }

        ObservableCollection<ReportCivilAlignmentOptions> CivilAlignmentOptions { get; }

        ObservableCollection<ReportCivilPointGroupOptions> CivilPointGroupOptions { get; }

        void GenerateReport();

        void BuildColumnHeaders();

        string WriteDataTable();

        void MoveUp(ColumnHeader columnHeader);

        void MoveDown(ColumnHeader columnHeader);
    }
}
