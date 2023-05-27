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
