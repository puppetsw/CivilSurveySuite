using System.Data;
using System.Windows.Input;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.Services.Interfaces;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    public class CogoPointSurfaceReportViewModel : ObservableObject
    {
        public ICogoPointSurfaceReportService ReportService { get; }
        private readonly ISaveFileDialogService _saveFileService;
        private readonly ICivilSelectService _civilSelectService;

        public ICommand SelectPointGroupCommand { get; private set; }

        public ICommand SelectSurfaceCommand { get; private set; }

        public ICommand StationOffsetSortCommand { get; private set; }

        public ICommand WriteToFileCommand { get; private set; }

        public ICommand GenerateReportCommand { get; private set; }

        public CogoPointSurfaceReportViewModel(
            ICogoPointSurfaceReportService cogoPointSurfaceReportService,
            ISaveFileDialogService saveFileDialogService,
            ICivilSelectService civilSelectService)
        {
            ReportService = cogoPointSurfaceReportService;
            _saveFileService = saveFileDialogService;
            _civilSelectService = civilSelectService;

            InitCommands();
        }

        private void InitCommands()
        {
            GenerateReportCommand    = new RelayCommand(UpdateReportData, () => true);
        }

        public DataView DataView => ReportService.DataTable?.DefaultView;

        private void UpdateReportData()
        {
            ReportService.GenerateReport();
            NotifyPropertyChanged(nameof(DataView));
        }

        // private string PrintTable()
        // {
        //     StringBuilder sb = new StringBuilder();
        //
        //     for (int i = 0; i < ReportDataTable.Columns.Count; i++)
        //     {
        //         sb.Append(ReportDataTable.Columns[i]);
        //         if (i < ReportDataTable.Columns.Count - 1)
        //             sb.Append(',');
        //     }
        //     sb.AppendLine();
        //     foreach (DataRow dr in ReportDataTable.Rows)
        //     {
        //         for (int i = 0; i < ReportDataTable.Columns.Count; i++)
        //         {

        //             sb.Append(dr[i]);
        //
        //             if (i < ReportDataTable.Columns.Count - 1)
        //                 sb.Append(',');
        //         }
        //         sb.AppendLine();
        //     }
        //     return sb.ToString();
        // }
        //
        // private void WriteToFile()
        // {
        //     _saveFileService.DefaultExt = ".csv";
        //     _saveFileService.Filter = "CSV Files (*.csv)|*.csv";
        //
        //     if (_saveFileService.ShowDialog() != true)
        //         return;
        //
        //     // Do the saving.
        //     var fileName = _saveFileService.FileName;
        //     FileHelpers.WriteFile(fileName, true, PrintTable());
        //     Process.Start(fileName);
        // }

        // {
        //     foreach (CivilAlignment alignment in Alignments)
        //     {
        //         var alignmentSetting = new AlignmentProperties
        //         {
        //             Alignment = alignment
        //         };
        //
        //         AlignmentSettings.Add(alignmentSetting);
        //     }
        //
        //     foreach (CivilSurface surface in Surfaces)
        //     {
        //         var surfaceSetting = new SurfaceProperties
        //         {
        //             Surface = surface
        //         };
        //
        //         SurfaceSettings.Add(surfaceSetting);
        //     }
        //
        //     foreach (CivilPointGroup pointGroup in PointGroups)
        //     {
        //         var pointGroupSetting = new PointGroupSettings
        //         {
        //             PointGroup = pointGroup
        //         };
        //
        //         PointGroupSettings.Add(pointGroupSetting);
        //     }
        // }
    }
}
