using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Input;
using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.Services.Interfaces;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    public class CogoPointSurfaceReportViewModel : ObservableObject
    {
        private readonly ICogoPointSurfaceReportService _reportService;
        private readonly ISaveFileDialogService _saveFileService;
        private readonly ICivilSelectService _civilSelectService;
        private CivilSurface _selectedCivilSurface;
        private CivilPointGroup _selectCivilPointGroup;
        private CivilAlignment _selectedCivilAlignment;
        private CivilSite _selectedSite;
        private string _stationRange;
        private string _surfaceRange;
        private bool _calculatePointNearSurfaceEdge;
        private bool _showInterpolatedAmount;
        private bool _showCutFillValues;
        private bool _invertCutFillValues;
        private ObservableCollection<ReportObject> _reportData;

        public bool CalculatePointNearSurfaceEdge
        {
            get => _calculatePointNearSurfaceEdge;
            set
            {
                SetProperty(ref _calculatePointNearSurfaceEdge, value);
                if (!value)
                {
                    ShowInterpolatedAmount = false;
                }
            }
        }

        public bool ShowInterpolatedAmount
        {
            get => _showInterpolatedAmount;
            set => SetProperty(ref _showInterpolatedAmount, value);
        }

        public bool ShowCutFillValues
        {
            get => _showCutFillValues;
            set
            {
                SetProperty(ref _showCutFillValues, value);
                InvertCutFillValues = value;
            }
        }

        public bool InvertCutFillValues
        {
            get => _invertCutFillValues;
            set => SetProperty(ref _invertCutFillValues, value);
        }

        public ObservableCollection<ReportObject> ReportData
        {
            get => _reportData;
            set => SetProperty(ref _reportData, value);
        }

        public ObservableCollection<CivilSite> Sites { get; }

        public ObservableCollection<CivilSurface> Surfaces { get; }

        public ObservableCollection<CivilPointGroup> PointGroups { get; }

        public ObservableCollection<CivilAlignment> Alignments { get; private set; }

        public CivilSurface SelectedSurface
        {
            get => _selectedCivilSurface;
            set => SetProperty(ref _selectedCivilSurface, value);
        }

        public CivilPointGroup SelectedPointGroup
        {
            get => _selectCivilPointGroup;
            set => SetProperty(ref _selectCivilPointGroup, value);
        }

        public CivilAlignment SelectedAlignment
        {
            get => _selectedCivilAlignment;
            set => SetProperty(ref _selectedCivilAlignment, value);
        }

        public CivilSite SelectedSite
        {
            get => _selectedSite;
            set
            {
                SetProperty(ref _selectedSite, value);

                // Update alignments
                Alignments = new ObservableCollection<CivilAlignment>(_civilSelectService.GetSiteAlignments(value));
                NotifyPropertyChanged(nameof(Alignments));
            }
        }

        public ObservableCollection<string> SurfaceNames { get; set; } = new ObservableCollection<string>();

        public string StationRange
        {
            get => _stationRange;
            set => SetProperty(ref _stationRange, value);
        }

        public string SurfaceRange
        {
            get => _surfaceRange;
            set => SetProperty(ref _surfaceRange, value);
        }

        public ICommand SelectPointGroupCommand { get; private set; }

        public ICommand SelectSurfaceCommand { get; private set; }

        public ICommand UpdateDataSourceCommand { get; private set; }

        // public ICommand StationOffsetSortCommand => new RelayCommand(StationOffsetSort, () => true);
        //
        // public ICommand WriteToFileCommand => new RelayCommand(WriteToFile, () => true);

        public CogoPointSurfaceReportViewModel(
            ICogoPointSurfaceReportService cogoPointSurfaceReportService,
            ISaveFileDialogService saveFileDialogService,
            ICivilSelectService civilSelectService)
        {
            _reportService = cogoPointSurfaceReportService;
            _saveFileService = saveFileDialogService;
            _civilSelectService = civilSelectService;

            Sites = new ObservableCollection<CivilSite>(_civilSelectService.GetSites());

            if (Sites.Count >= 1)
                SelectedSite = Sites[0];

            Alignments  = new ObservableCollection<CivilAlignment>(_civilSelectService.GetSiteAlignments(SelectedSite));
            PointGroups = new ObservableCollection<CivilPointGroup>(_civilSelectService.GetPointGroups());
            Surfaces    = new ObservableCollection<CivilSurface>(_civilSelectService.GetSurfaces());
            ReportData  = new ObservableCollection<ReportObject>();

            InitCommands();
        }

        private void InitCommands()
        {
            SelectPointGroupCommand = new RelayCommand(SelectPointGroup, () => true);
            SelectSurfaceCommand    = new RelayCommand(SelectSurface, () => true);
            UpdateDataSourceCommand = new AsyncRelayCommand(UpdateReportData);
        }

        private void SetStationRange()
        {
            if (SelectedAlignment == null)
            {
                StationRange = string.Empty;
                return;
            }

            StationRange = $"{Math.Round(SelectedAlignment.StationStart, 2)} - {Math.Round(SelectedAlignment.StationEnd, 2)}";
        }

        private void SetSurfaceRange()
        {
            if (SelectedSurface == null)
            {
                SurfaceRange = string.Empty;
                return;
            }

            SurfaceRange = $"{Math.Round(SelectedSurface.MinimumElevation, 3)} - {Math.Round(SelectedSurface.MaximumElevation, 3)}";
        }

        private DataTable _dataTable;

        public DataView DataView => _dataTable?.DefaultView;

        private void BuildDataTableColumns()
        {
            _dataTable = new DataTable();

            _dataTable.Columns.Add("Point Number", typeof(uint));
            _dataTable.Columns.Add("Easting", typeof(double));
            _dataTable.Columns.Add("Northing", typeof(double));
            _dataTable.Columns.Add("Elevation", typeof(double));
            _dataTable.Columns.Add("Raw Description", typeof(string));
            _dataTable.Columns.Add("Chainage", typeof(double));
            _dataTable.Columns.Add("Offset", typeof(double));

            // Loop and add selected surfaces
            _dataTable.Columns.Add(SelectedSurface.Name, typeof(double));
        }

        private void BuildDataTableRows()
        {
            foreach (ReportObject ro in ReportData)
            {
                var list = new List<object>
                {
                    ro.Point.PointNumber,
                    ro.Easting,
                    ro.Northing,
                    ro.Point.Elevation,
                    ro.Point.RawDescription,
                    ro.Chainage,
                    ro.Offset
                };

                foreach (ReportSurfaceObject surfaceObject in ro.SurfacePoints)
                {
                    list.Add(surfaceObject.Point.Elevation);
                }

                _dataTable.Rows.Add(list.ToArray());
            }
            NotifyPropertyChanged(nameof(DataView));
        }

        private async Task UpdateReportData()
        {
            if (SelectedAlignment == null)
                return;

            if (SelectedPointGroup == null)
                return;

            if (SelectedSurface == null)
                return;

            SurfaceNames.Clear();
            foreach (CivilSurface surface in Surfaces)
            {
                SurfaceNames.Add(surface.Name);
            }

            _reportService.Interpolate = CalculatePointNearSurfaceEdge;

            var data = await _reportService.GetReportData(SelectedAlignment,
                new List<CivilPointGroup> { SelectedPointGroup }, Surfaces);

            await Task.Run(() => ReportData = data);

            SetStationRange();
            SetSurfaceRange();

            BuildDataTableColumns();
            BuildDataTableRows();
        }

        // private void StationOffsetSort()
        // {
        //     if (SelectedAlignment == null)
        //         return;
        //
        //     var sortString = $"{SelectedAlignment.Name} Chainage ASC, {SelectedAlignment.Name} Offset ASC";
        //     ReportDataTable.DefaultView.Sort = sortString;
        // }

        private void SelectPointGroup()
        {
            var pointGroup = _civilSelectService.SelectPointGroup();
            if (pointGroup == null)
                return;

            if (PointGroups.Contains(pointGroup))
            {
                var index = PointGroups.IndexOf(pointGroup);
                SelectedPointGroup = PointGroups[index];
                SelectedPointGroup.IsSelected = true;
            }
        }

        private void SelectSurface()
        {
            var surface = _civilSelectService.SelectSurface();
            if (surface == null)
                return;

            if (!Surfaces.Contains(surface))
                return;

            var index = Surfaces.IndexOf(surface);
            SelectedSurface = Surfaces[index];
            SelectedSurface.IsSelected = true;
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
    }
}
