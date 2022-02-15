// ----------------------------------------------------------------------
//  <copyright file="CogoPointSurfaceReportViewModel.cs" company="Scott Whitney">
//      Author: Scott Whitney
//      Copyright (c) Scott Whitney. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    public class CogoPointSurfaceReportViewModel : ViewModelBase
    {
        private readonly ICogoPointSurfaceReportService _reportService;
        private readonly ISaveFileDialogService _saveFileService;
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

        public ObservableCollection<ReportObject> ReportData
        {
            get => _reportData;
            set
            {
                _reportData = value;
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

        public ICommand SelectPointGroupCommand => new RelayCommand(SelectPointGroup, () => true);

        public ICommand SelectSurfaceCommand => new RelayCommand(SelectSurface, () => true);

        public ICommand UpdateDataSourceCommand => new AsyncRelayCommand(UpdateReportData);

        // public ICommand StationOffsetSortCommand => new RelayCommand(StationOffsetSort, () => true);
        //
        // public ICommand WriteToFileCommand => new RelayCommand(WriteToFile, () => true);

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
            ReportData = new ObservableCollection<ReportObject>();
        }

        private void SetStationRange()
        {
            if (SelectedAlignment == null)
            {
                StationRange = string.Empty;
                return;
            }

            StationRange = $"{Math.Round(SelectedAlignment.StationStart, 2)} to " +
                           $"{Math.Round(SelectedAlignment.StationEnd, 2)}";
        }

        private void SetSurfaceRange()
        {
            if (SelectedSurface == null)
            {
                SurfaceRange = string.Empty;
                return;
            }

            SurfaceRange = $"{Math.Round(SelectedSurface.MinimumElevation, 3)} to " +
                           $"{Math.Round(SelectedSurface.MaximumElevation, 3)}";
        }

        private async Task UpdateReportData()
        {
            if (SelectedAlignment == null)
                return;

            if (SelectedPointGroup == null)
                return;

            if (SelectedSurface == null)
                return;

            var data = await _reportService.GetReportData(SelectedPointGroup, SelectedAlignment, SelectedSurface,
                CalculatePointNearSurfaceEdge);

            // This seems to solve the UI locking up issue.
            await Task.Run(() => ReportData = data);

            SetStationRange();
            SetSurfaceRange();

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