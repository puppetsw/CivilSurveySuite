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
        private DataView _reportDataView;
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
        private bool _sortByStationOffset;

        public bool CalculatePointNearSurfaceEdge
        {
            get => _calculatePointNearSurfaceEdge;
            set
            {
                _calculatePointNearSurfaceEdge = value;
                ShowInterpolatedAmount = value;
                GenerateReport();
                NotifyPropertyChanged();
            }
        }

        public bool ShowInterpolatedAmount
        {
            get => _showInterpolatedAmount;
            set
            {
                _showInterpolatedAmount = value;
                GenerateReport();
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
                GenerateReport();
                NotifyPropertyChanged();
            }
        }

        public bool InvertCutFillValues
        {
            get => _invertCutFillValues;
            set
            {
                _invertCutFillValues = value;
                GenerateReport();
                NotifyPropertyChanged();
            }
        }

        public bool SortByStationOffset
        {
            get => _sortByStationOffset;
            set
            {
                _sortByStationOffset = value;
                NotifyPropertyChanged();
            }
        }

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
                GenerateReport();
                SetSurfaceRange();
                NotifyPropertyChanged();
            }
        }

        public CivilPointGroup SelectedPointGroup
        {
            get => _selectCivilPointGroup;
            set
            {
                _selectCivilPointGroup = value;
                GenerateReport();
                NotifyPropertyChanged();
            }
        }

        public CivilAlignment SelectedAlignment
        {
            get => _selectedCivilAlignment;
            set
            {
                _selectedCivilAlignment = value;
                GenerateReport();
                SetStationRange();
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
                GenerateReport();
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

        public DataView ReportDataView
        {
            get => _reportDataView;
            set
            {
                _reportDataView = value;
                NotifyPropertyChanged();
            }
        }

        public RelayCommand SelectPointGroupCommand => new RelayCommand(SelectPointGroup, () => true);

        public RelayCommand SelectSurfaceCommand => new RelayCommand(SelectSurface, () => true);

        public RelayCommand CreateReportCommand => new RelayCommand(GenerateReport, () => true);

        public RelayCommand StationOffsetSortCommand => new RelayCommand(StationOffsetSort, () => true);

        public RelayCommand WriteToFileCommand => new RelayCommand(WriteToFile, () => true);

        public CogoPointSurfaceReportViewModel(ICogoPointSurfaceReportService cogoPointSurfaceReportService, ISaveFileDialogService saveFileDialogService)
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

        private void GenerateColumnHeadings()
        {
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
            ReportDataTable.Columns.Add(id);
            ReportDataTable.Columns.Add(easting);
            ReportDataTable.Columns.Add(northing);
            ReportDataTable.Columns.Add(elevation);
            ReportDataTable.Columns.Add(rawDes);

            // Add alignment
            if (SelectedAlignment != null)
            {
                var alignmentStation = new DataColumn($"{SelectedAlignment.Name} Chainage");
                var alignmentOffset = new DataColumn($"{SelectedAlignment.Name} Offset");

                alignmentStation.DataType = typeof(double);
                alignmentOffset.DataType = typeof(double);

                ReportDataTable.Columns.Add(alignmentStation);
                ReportDataTable.Columns.Add(alignmentOffset);
            }

            // Add surface
            if (SelectedSurface != null)
            {
                var surfaceColumn = new DataColumn(SelectedSurface.Name);
                surfaceColumn.DataType = typeof(double);
                ReportDataTable.Columns.Add(surfaceColumn);

                // Generate dX, dY headings if needed
                if (ShowInterpolatedAmount)
                {
                    var dXColumn = new DataColumn($"{SelectedSurface.Name} dX");
                    var dYColumn = new DataColumn($"{SelectedSurface.Name} dY");
                    dXColumn.DataType = typeof(double);
                    dYColumn.DataType = typeof(double);
                    ReportDataTable.Columns.Add(dXColumn);
                    ReportDataTable.Columns.Add(dYColumn);
                }

                if (ShowCutFillValues)
                {
                    var cutFillColumn = new DataColumn($"{SelectedSurface.Name} Cut Fill");
                    cutFillColumn.DataType = typeof(double);
                    ReportDataTable.Columns.Add(cutFillColumn);
                }
            }
        }

        private void GenerateReport()
        {
            // Clear and create new DataTable
            ReportDataTable = null;
            ReportDataTable = new DataTable();

            // Create headings
            GenerateColumnHeadings();

            if (SelectedPointGroup != null)
            {
                // Loop points in selected point groups
                var cogoPointsAdded = new List<CivilPoint>();

                // foreach point in the PointGroup
                foreach (CivilPoint civilPoint in _reportService.GetPointsInPointGroup(SelectedPointGroup))
                {
                    // don't add duplicates.
                    if (cogoPointsAdded.Contains(civilPoint))
                        continue;

                    // Add point to list to check if it's already been added.
                    cogoPointsAdded.Add(civilPoint);

                    var dataRow = ReportDataTable.NewRow();
                    dataRow[0] = civilPoint.PointNumber;
                    dataRow[1] = Math.Round(civilPoint.Easting, 3);
                    dataRow[2] = Math.Round(civilPoint.Northing, 3);
                    dataRow[3] = Math.Round(civilPoint.Elevation, 3);
                    dataRow[4] = civilPoint.RawDescription;

                    var columnCount = 5;

                    GenerateAlignmentData(dataRow, civilPoint, ref columnCount);
                    GenerateSurfaceData(dataRow, civilPoint, ref columnCount);

                    ReportDataTable.Rows.Add(dataRow);
                }
            }
            // Set the view property.
            ReportDataView = ReportDataTable.DefaultView;
            // Sort by station/offset
            StationOffsetSort();
        }

        private void GenerateAlignmentData(DataRow dataRow, CivilPoint civilPoint, ref int columnCount)
        {
            // Add alignment station and offset
            if (SelectedAlignment == null)
                return;

            var stationOffset = _reportService.GetStationOffsetAtCivilPoint(civilPoint, SelectedAlignment);

            dataRow[columnCount] = Math.Round(stationOffset.Station, 2);
            columnCount++;
            dataRow[columnCount] = Math.Round(stationOffset.Offset, 2);
        }

        private void GenerateSurfaceData(DataRow dataRow, CivilPoint civilPoint, ref int columnCount)
        {
            // Add surface elevation
            if (SelectedSurface == null)
                return;

            var elevationAtSurface = Math.Round(_reportService.GetElevationAtCivilPoint(
                civilPoint, SelectedSurface, CalculatePointNearSurfaceEdge, out double dX, out double dY), 3);

            columnCount++;
            dataRow[columnCount] = elevationAtSurface;

            if (ShowInterpolatedAmount)
            {
                columnCount++;
                // add dX column
                dataRow[columnCount] = dX;
                columnCount++;
                // add dY column
                dataRow[columnCount] = dY;
            }

            if (ShowCutFillValues)
            {
                columnCount++;
                // add cut/fill column
                // check for cut/fill invert
                double cutFill = InvertCutFillValues
                    ? Math.Round(elevationAtSurface - civilPoint.Elevation, 3)
                    : Math.Round(civilPoint.Elevation - elevationAtSurface, 3);

                dataRow[columnCount] = cutFill;
            }
        }

        private void StationOffsetSort()
        {
            if (SelectedAlignment == null)
                return;

            var sortString = $"{SelectedAlignment.Name} Chainage ASC, {SelectedAlignment.Name} Offset ASC";
            ReportDataView.Sort = sortString;
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
            ReportDataView.Table.AcceptChanges();

            for (int i = 0; i < ReportDataView.Table.Columns.Count; i++)
            {
                sb.Append(ReportDataView.Table.Columns[i]);
                if (i < ReportDataView.Table.Columns.Count - 1)
                    sb.Append(',');
            }
            sb.AppendLine();
            foreach (DataRow dr in ReportDataView.Table.Rows)
            {
                for (int i = 0; i < ReportDataView.Table.Columns.Count; i++)
                {
                    sb.Append(dr[i]);

                    if (i < ReportDataView.Table.Columns.Count - 1)
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