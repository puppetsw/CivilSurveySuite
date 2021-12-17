// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    public class CogoPointSurfaceReportViewModel : ViewModelBase
    {
        private readonly ICogoPointSurfaceReportService _reportService;
        private readonly ISaveFileDialogService _saveFileService;
        private DataTable _reportDataTable;
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

        public bool CalculatePointNearSurfaceEdge
        {
            get => _calculatePointNearSurfaceEdge;
            set
            {
                _calculatePointNearSurfaceEdge = value;
                if (!value)
                    ShowInterpolatedAmount = false;
                NotifyPropertyChanged();
            }
        }

        public bool ShowInterpolatedAmount
        {
            get => _showInterpolatedAmount;
            set
            {
                _showInterpolatedAmount = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShowCutFillValues
        {
            get => _showCutFillValues;
            set
            {
                _showCutFillValues = value;
                InvertCutFillValues = value;
                NotifyPropertyChanged();
            }
        }

        public bool InvertCutFillValues
        {
            get => _invertCutFillValues;
            set
            {
                _invertCutFillValues = value;
                NotifyPropertyChanged();
            }
        }

        private List<ReportObject> ReportData { get; set; }

        public ObservableCollection<CivilSite> Sites { get; }

        public ObservableCollection<CivilSurface> Surfaces { get; }

        public ObservableCollection<CivilPointGroup> PointGroups { get; }

        public ObservableCollection<CivilAlignment> Alignments { get; private set; }

        public CivilSurface SelectedSurface
        {
            get => _selectedCivilSurface;
            set
            {
                _selectedCivilSurface = value;
                NotifyPropertyChanged();
            }
        }

        public CivilPointGroup SelectedPointGroup
        {
            get => _selectCivilPointGroup;
            set
            {
                _selectCivilPointGroup = value;
                NotifyPropertyChanged();
            }
        }

        public CivilAlignment SelectedAlignment
        {
            get => _selectedCivilAlignment;
            set
            {
                _selectedCivilAlignment = value;
                NotifyPropertyChanged();
            }
        }

        public CivilSite SelectedSite
        {
            [DebuggerStepThrough]
            get => _selectedSite;
            [DebuggerStepThrough]
            set
            {
                _selectedSite = value;
                // Update alignments
                Alignments = new ObservableCollection<CivilAlignment>(_reportService.GetSiteAlignments(value));

                NotifyPropertyChanged(nameof(Alignments));
                NotifyPropertyChanged();
            }
        }

        public string StationRange
        {
            [DebuggerStepThrough]
            get => _stationRange;
            [DebuggerStepThrough]
            set
            {
                _stationRange = value;
                NotifyPropertyChanged();
            }
        }

        public string SurfaceRange
        {
            [DebuggerStepThrough]
            get => _surfaceRange;
            [DebuggerStepThrough]
            set
            {
                _surfaceRange = value;
                NotifyPropertyChanged();
            }
        }

        public DataTable ReportDataTable
        {
            get => _reportDataTable;
            set
            {
                _reportDataTable = value;
                NotifyPropertyChanged();
            }
        }

        public RelayCommand SelectPointGroupCommand => new RelayCommand(SelectPointGroup, () => true);

        public RelayCommand SelectSurfaceCommand => new RelayCommand(SelectSurface, () => true);

        public AsyncRelayCommand UpdateDataSourceCommand => new AsyncRelayCommand(UpdateReportData, () => true);

        public AsyncRelayCommand UpdateDataTableCommand => new AsyncRelayCommand(UpdateDataTable, () => true);

        public RelayCommand StationOffsetSortCommand => new RelayCommand(StationOffsetSort, () => true);

        public RelayCommand WriteToFileCommand => new RelayCommand(WriteToFile, () => true);

        public CogoPointSurfaceReportViewModel(ICogoPointSurfaceReportService cogoPointSurfaceReportService,
            ISaveFileDialogService saveFileDialogService)
        {
            _reportService = cogoPointSurfaceReportService;
            _saveFileService = saveFileDialogService;

            Sites = new ObservableCollection<CivilSite>(_reportService.GetSites());

            if (Sites.Count >= 1)
                SelectedSite = Sites[0];

            Alignments = new ObservableCollection<CivilAlignment>(_reportService.GetSiteAlignments(SelectedSite));
            PointGroups = new ObservableCollection<CivilPointGroup>(_reportService.GetPointGroups());
            Surfaces = new ObservableCollection<CivilSurface>(_reportService.GetSurfaces());

            ReportDataTable = new DataTable();
        }

        private void SetStationRange()
        {
            if (SelectedAlignment == null)
            {
                StationRange = string.Empty;
                return;
            }

            StationRange = $"{Math.Round(SelectedAlignment.StationStart, 2)} to {Math.Round(SelectedAlignment.StationEnd, 2)}";
        }

        private void SetSurfaceRange()
        {
            if (SelectedSurface == null)
            {
                SurfaceRange = string.Empty;
                return;
            }

            SurfaceRange = $"{Math.Round(SelectedSurface.MinimumElevation, 3)} to {Math.Round(SelectedSurface.MaximumElevation, 3)}";
        }

        private Task<DataTable> CreateDataTable()
        {
            var dt = new DataTable();

            // Generate default headings
            var id = new DataColumn("ID");
            var easting = new DataColumn("Easting");
            var northing = new DataColumn("Northing");
            var elevation = new DataColumn("Point Elevation");
            var rawDes = new DataColumn("Raw Description");

            // Set datatypes
            id.DataType = typeof(uint);
            easting.DataType = typeof(double);
            northing.DataType = typeof(double);
            elevation.DataType = typeof(double);
            rawDes.DataType = typeof(string);

            // Add to datatable
            dt.Columns.Add(id);
            dt.Columns.Add(easting);
            dt.Columns.Add(northing);
            dt.Columns.Add(elevation);
            dt.Columns.Add(rawDes);

            // Add alignment
            if (SelectedAlignment != null)
            {
                var alignmentStation = new DataColumn($"{SelectedAlignment.Name} Chainage");
                var alignmentOffset = new DataColumn($"{SelectedAlignment.Name} Offset");

                alignmentStation.DataType = typeof(double);
                alignmentOffset.DataType = typeof(double);

                dt.Columns.Add(alignmentStation);
                dt.Columns.Add(alignmentOffset);
            }

            // Add surface
            if (SelectedSurface != null)
            {
                var surfaceColumn = new DataColumn(SelectedSurface.Name);
                surfaceColumn.DataType = typeof(double);
                dt.Columns.Add(surfaceColumn);

                // Generate dX, dY headings if needed
                if (ShowInterpolatedAmount)
                {
                    var dXColumn = new DataColumn($"{SelectedSurface.Name} dX");
                    var dYColumn = new DataColumn($"{SelectedSurface.Name} dY");
                    dXColumn.DataType = typeof(double);
                    dYColumn.DataType = typeof(double);
                    dt.Columns.Add(dXColumn);
                    dt.Columns.Add(dYColumn);
                }

                if (ShowCutFillValues)
                {
                    var cutFillColumn = new DataColumn($"{SelectedSurface.Name} Cut Fill");
                    cutFillColumn.DataType = typeof(double);
                    dt.Columns.Add(cutFillColumn);
                }
            }

            return Task.FromResult(dt);
        }

        private async Task<DataTable> PopulateDataTable(IEnumerable<ReportObject> data)
        {
            var dt = await CreateDataTable().ConfigureAwait(false);

            foreach (var reportObject in data)
            {
                var dataRow = dt.NewRow();
                dataRow[0] = reportObject.PointNumber;
                dataRow[1] = Math.Round(reportObject.Easting, 3);
                dataRow[2] = Math.Round(reportObject.Northing, 3);
                dataRow[3] = Math.Round(reportObject.PointElevation, 3);
                dataRow[4] = reportObject.RawDescription;

                if (SelectedAlignment == null)
                    continue;

                var columnCount = 5;

                dataRow[columnCount] = Math.Round(reportObject.StationOffset.Station, 2);
                columnCount++;
                dataRow[columnCount] = Math.Round(reportObject.StationOffset.Offset, 2);
                SetStationRange();

                if (SelectedSurface != null)
                {
                    columnCount++;
                    dataRow[columnCount] = reportObject.SurfaceElevation;
                    SetSurfaceRange();
                }

                if (ShowInterpolatedAmount)
                {
                    columnCount++;
                    // add dX column
                    dataRow[columnCount] = reportObject.CalculatedDeltaX;
                    columnCount++;
                    // add dY column
                    dataRow[columnCount] = reportObject.CalculatedDeltaY;
                }

                if (ShowCutFillValues)
                {
                    columnCount++;
                    // add cut/fill column
                    // check for cut/fill invert
                    double cutFill = InvertCutFillValues
                        ? Math.Round(reportObject.CutFillInvert, 3)
                        : Math.Round(reportObject.CutFill, 3);

                    dataRow[columnCount] = cutFill;
                }

                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        private async Task UpdateDataTable()
        {
            ReportDataTable = await PopulateDataTable(ReportData).ConfigureAwait(false);
        }

        private async Task UpdateReportData()
        {
            if (SelectedAlignment == null)
                return;

            if (SelectedPointGroup == null)
                return;

            var data = await _reportService.GetReportData(SelectedPointGroup, SelectedAlignment, SelectedSurface, CalculatePointNearSurfaceEdge).ConfigureAwait(false);
            ReportData = new List<ReportObject>(data);
        }

        private void StationOffsetSort()
        {
            if (SelectedAlignment == null)
                return;

            var sortString = $"{SelectedAlignment.Name} Chainage ASC, {SelectedAlignment.Name} Offset ASC";
            ReportDataTable.DefaultView.Sort = sortString;
        }

        private void SelectPointGroup()
        {
            var pointGroup = _reportService.SelectPointGroup();
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
            var surface = _reportService.SelectSurface();
            if (surface == null)
                return;

            if (Surfaces.Contains(surface))
            {
                var index = Surfaces.IndexOf(surface);
                SelectedSurface = Surfaces[index];
                SelectedSurface.IsSelected = true;
            }
        }

        private string PrintTable()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < ReportDataTable.Columns.Count; i++)
            {
                sb.Append(ReportDataTable.Columns[i]);
                if (i < ReportDataTable.Columns.Count - 1)
                    sb.Append(',');
            }
            sb.AppendLine();
            foreach (DataRow dr in ReportDataTable.Rows)
            {
                for (int i = 0; i < ReportDataTable.Columns.Count; i++)
                {
                    sb.Append(dr[i]);

                    if (i < ReportDataTable.Columns.Count - 1)
                        sb.Append(',');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private void WriteToFile()
        {
            _saveFileService.DefaultExt = ".csv";
            _saveFileService.Filter = "CSV Files (*.csv)|*.csv";

            if (_saveFileService.ShowDialog() != true)
                return;

            // Do the saving.
            var fileName = _saveFileService.FileName;
            FileHelpers.WriteFile(fileName, true, PrintTable());
            Process.Start(fileName);
        }
    }
}