// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using System.Data;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    public class CogoPointSurfaceReportViewModel : ViewModelBase
    {
        private readonly ICogoPointSurfaceReportService _cogoPointSurfaceReportService;
        private DataTable _reportDataTable;
        private DataView _reportDataView;
        private bool _calculatePointNearSurfaceEdge;
        private CivilSurface _selectedCivilSurface;
        private CivilPointGroup _selectCivilPointGroup;
        private CivilAlignment _selectedCivilAlignment;

        public bool CalculatePointNearSurfaceEdge
        {
            get => _calculatePointNearSurfaceEdge;
            set
            {
                _calculatePointNearSurfaceEdge = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<CivilSurface> Surfaces { get; }

        public ObservableCollection<CivilPointGroup> PointGroups { get; }

        public ObservableCollection<CivilAlignment> Alignments { get; }

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

        public RelayCommand SelectAlignmentCommand => new RelayCommand(SelectAlignment, () => true);

        public RelayCommand SelectAllAlignmentCommand => new RelayCommand(() => SelectAll(Alignments), () => true);

        public RelayCommand SelectNoneAlignmentCommand => new RelayCommand(() => SelectNone(Alignments), () => true);

        public RelayCommand SelectPointGroupCommand => new RelayCommand(SelectPointGroup, () => true);
        
        public RelayCommand SelectAllPointGroupCommand => new RelayCommand(() => SelectAll(PointGroups), () => true);

        public RelayCommand SelectNonePointGroupCommand => new RelayCommand(() => SelectNone(PointGroups), () => true);

        public RelayCommand SelectSurfaceCommand => new RelayCommand(SelectSurface, () => true);

        public RelayCommand SelectAllSurfaceCommand => new RelayCommand(() => SelectAll(Surfaces), () => true);

        public RelayCommand SelectNoneSurfaceCommand => new RelayCommand(() => SelectNone(Surfaces), () => true);

        public RelayCommand CreateReportCommand => new RelayCommand(CreateReport, () => true);

        public CogoPointSurfaceReportViewModel(ICogoPointSurfaceReportService cogoPointSurfaceReportService)
        {
            _cogoPointSurfaceReportService = cogoPointSurfaceReportService;
            Alignments = new ObservableCollection<CivilAlignment>(_cogoPointSurfaceReportService.GetAlignments());
            PointGroups = new ObservableCollection<CivilPointGroup>(_cogoPointSurfaceReportService.GetPointGroups());
            Surfaces = new ObservableCollection<CivilSurface>(_cogoPointSurfaceReportService.GetSurfaces());

            ReportDataTable = new DataTable();
        }

        private void CreateReport()
        {
            ReportDataTable.Clear();
            ReportDataTable.Columns.Clear();

            var id = new DataColumn("ID");
            id.DataType = typeof(uint);
            var easting = new DataColumn("Easting");
            easting.DataType = typeof(double);
            var northing = new DataColumn("Northing");
            northing.DataType = typeof(double);
            var elevation = new DataColumn("Point Elevation");
            elevation.DataType = typeof(double);
            var rawDes = new DataColumn("Raw Description");
            rawDes.DataType = typeof(string);

            ReportDataTable.Columns.Add(id);
            ReportDataTable.Columns.Add(easting);
            ReportDataTable.Columns.Add(northing);
            ReportDataTable.Columns.Add(elevation);
            ReportDataTable.Columns.Add(rawDes);

            // Create Columns for Alignments
            foreach (CivilAlignment civilAlignment in Alignments)
            {
                if (!civilAlignment.IsSelected)
                    continue;

                var alignmentColumn = new DataColumn($"{civilAlignment.Name} Chainage");
                var alignmentColumn1 = new DataColumn($"{civilAlignment.Name} Offset");
                ReportDataTable.Columns.Add(alignmentColumn);
                ReportDataTable.Columns.Add(alignmentColumn1);
            }

            // Create Columns for Surfaces
            foreach (CivilSurface civilSurface in Surfaces)
            {
                if (!civilSurface.IsSelected)
                    continue;
                
                var surfaceColumn = new DataColumn(civilSurface.Name);
                ReportDataTable.Columns.Add(surfaceColumn);
            }
            
            // Loop points in selected point groups
            var cogoPointsAdded = new List<CivilPoint>();
            foreach (CivilPointGroup civilPointGroup in PointGroups)
            {
                if (!civilPointGroup.IsSelected)
                    continue;

                // foreach point in the pointgroup
                foreach (CivilPoint civilPoint in _cogoPointSurfaceReportService.GetPointsInPointGroup(civilPointGroup))
                {
                    if (cogoPointsAdded.Contains(civilPoint))
                        continue;

                    cogoPointsAdded.Add(civilPoint); // Add point to list to check if it's already been added.

                    var dataRow = ReportDataTable.NewRow();
                    dataRow[0] = civilPoint.PointNumber;
                    dataRow[1] = civilPoint.Easting;
                    dataRow[2] = civilPoint.Northing;
                    dataRow[3] = civilPoint.Elevation;
                    dataRow[4] = civilPoint.RawDescription;

                    var columnCount = 5;
                    foreach (CivilAlignment civilAlignment in Alignments)
                    {
                        if (!civilAlignment.IsSelected)
                            continue;

                        var stationOffset = _cogoPointSurfaceReportService.GetStationOffsetAtCivilPoint(civilPoint, civilAlignment);

                        dataRow[columnCount] = stationOffset.Station;
                        columnCount++;
                        dataRow[columnCount] = stationOffset.Offset;
                        columnCount++;
                    }

                    foreach (CivilSurface civilSurface in Surfaces)
                    {
                        if (!civilSurface.IsSelected)
                            continue;

                        dataRow[columnCount] = _cogoPointSurfaceReportService.GetElevationAtCivilPoint(civilPoint, civilSurface, CalculatePointNearSurfaceEdge);
                        columnCount++;
                    }
                    ReportDataTable.Rows.Add(dataRow);
                }
            }
            // Set the view property.
            ReportDataView = ReportDataTable.DefaultView;
        }

        private void SelectNone(IEnumerable<CivilObject> civilObjects)
        {
            foreach (CivilObject civilObject in civilObjects)
            {
                civilObject.IsSelected = false;
            }
        }

        private void SelectAll(IEnumerable<CivilObject> civilObjects)
        {
            foreach (CivilObject civilObject in civilObjects)
            {
                civilObject.IsSelected = true;
            }
        }

        private void SelectAlignment()
        {
            var alignment = _cogoPointSurfaceReportService.SelectAlignment();
            if (alignment == null)
                return;

            if (Alignments.Contains(alignment))
            {
                var index = Alignments.IndexOf(alignment);
                SelectedAlignment = Alignments[index];
                SelectedAlignment.IsSelected = true;
            }
        }

        private void SelectPointGroup()
        {
            var pointGroup = _cogoPointSurfaceReportService.SelectPointGroup();
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
            var surface = _cogoPointSurfaceReportService.SelectSurface();
            if (surface == null)
                return;

            if (Surfaces.Contains(surface))
            {
                var index = Surfaces.IndexOf(surface);
                SelectedSurface = Surfaces[index];
                SelectedSurface.IsSelected = true;
            }
        }
    }
}