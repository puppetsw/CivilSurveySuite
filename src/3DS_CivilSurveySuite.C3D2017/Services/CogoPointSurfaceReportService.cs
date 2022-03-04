using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.UI;
using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services.Interfaces;
using DataTable = System.Data.DataTable;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class CogoPointSurfaceReportService : ObservableObject, ICogoPointSurfaceReportService
    {
        private readonly ICivilSelectService _civilSelectService;
        private ColumnProperties _columnProperties;

        public DataTable DataTable { get; private set; }

        public ColumnProperties ColumnProperties
        {
            get => _columnProperties;
            private set => SetProperty(ref _columnProperties, value);
        }

        public ObservableCollection<ReportCivilSurfaceOptions> CivilSurfaceOptions { get; }
            = new ObservableCollection<ReportCivilSurfaceOptions>();
        public ObservableCollection<ReportCivilAlignmentOptions> CivilAlignmentOptions { get; }
            = new ObservableCollection<ReportCivilAlignmentOptions>();
        public ObservableCollection<ReportCivilPointGroupOptions> CivilPointGroupOptions { get; }
            = new ObservableCollection<ReportCivilPointGroupOptions>();

        public CogoPointSurfaceReportService(ICivilSelectService civilSelectService)
        {
            _civilSelectService = civilSelectService;

            ColumnProperties = new ColumnProperties();

            LoadPointGroups();
            LoadAlignments();
            LoadSurfaces();
        }

        private void LoadSurfaces()
        {
            var surfaces = _civilSelectService.GetSurfaces();

            foreach (CivilSurface civilSurface in surfaces)
            {
                CivilSurfaceOptions.Add(new ReportCivilSurfaceOptions
                {
                    CivilSurface = civilSurface,
                    CivilSurfaceProperties = new CivilSurfaceProperties()
                });
            }
        }

        private void LoadAlignments()
        {
            var alignments = _civilSelectService.GetAlignments();

            foreach (CivilAlignment civilAlignment in alignments)
            {
                CivilAlignmentOptions.Add(new ReportCivilAlignmentOptions
                {
                    CivilAlignment = civilAlignment,
                    CivilAlignmentProperties = new CivilAlignmentProperties()
                });
            }
        }

        private void LoadPointGroups()
        {
            var pointGroups = _civilSelectService.GetPointGroups();

            foreach (CivilPointGroup civilPointGroup in pointGroups)
            {
                CivilPointGroupOptions.Add(new ReportCivilPointGroupOptions
                {
                    CivilPointGroup = civilPointGroup,
                    CivilPointGroupProperties = new CivilPointGroupProperties()
                });
            }
        }

        public void GenerateReport()
        {
            BuildDataTableColumns();
            BuildDataTableRows();
        }

        private void BuildDataTableColumns()
        {
            DataTable = new DataTable();

            // Determine which columns to show.
            if (ColumnProperties.ShowPointNumber)
            {
                DataTable.Columns.Add("Point Number", typeof(uint));
            }

            if (ColumnProperties.ShowEasting)
            {
                DataTable.Columns.Add("Easting", typeof(double));
            }

            if (ColumnProperties.ShowNorthing)
            {
                DataTable.Columns.Add("Northing", typeof(double));
            }

            if (ColumnProperties.ShowElevation)
            {
                DataTable.Columns.Add("Elevation", typeof(double));
            }

            if (ColumnProperties.ShowRawDescription)
            {
                DataTable.Columns.Add("Raw Description", typeof(string));
            }

            if (ColumnProperties.ShowFullDescription)
            {
                DataTable.Columns.Add("Full Description", typeof(string));
            }

            // Add alignment details.
            foreach (ReportCivilAlignmentOptions alignmentOption in CivilAlignmentOptions)
            {
                if (!alignmentOption.CivilAlignment.IsSelected)
                {
                    continue;
                }

                // Add alignment name column.
                DataTable.Columns.Add($"{alignmentOption.CivilAlignment.Name} Chainage", typeof(double));

                // Add offset column
                DataTable.Columns.Add($"{alignmentOption.CivilAlignment.Name} Offset", typeof(double));
            }

            // Loop and add selected surfaces.
            foreach (ReportCivilSurfaceOptions option in CivilSurfaceOptions)
            {
                if (!option.CivilSurface.IsSelected)
                {
                    continue;
                }

                // Add surface elevation column.
                DataTable.Columns.Add($"{option.CivilSurface.Name} Elevation", typeof(double));

                // Add cut and fill column if it's enabled.
                if (option.CivilSurfaceProperties.ShowCutFill)
                {
                    DataTable.Columns.Add($"{option.CivilSurface.Name} Cut Fill", typeof(double));
                }
            }
        }

        private void BuildDataTableRows()
        {
            using (var tr = AcadApp.StartTransaction())
            {
                var pointList = new List<CivilPoint>();

                foreach (ReportCivilPointGroupOptions pointGroupOption in CivilPointGroupOptions)
                {
                    if (!pointGroupOption.CivilPointGroup.IsSelected)
                    {
                        continue;
                    }

                    var rowData = new List<object>();

                    var civilPoints = CogoPointUtils.GetCivilPointsFromPointGroup(
                        tr,
                        pointGroupOption.CivilPointGroup.Name);

                    foreach (CivilPoint civilPoint in civilPoints)
                    {
                        if (!pointGroupOption.CivilPointGroupProperties.AllowDuplicates)
                        {
                            // check for duplicate point.
                            if (pointList.Contains(civilPoint))
                            {
                                // Log duplicate point
                                AcadApp.WriteMessage($"3DS> ReportService: Duplicate Point {civilPoint.PointNumber}");
                                Debug.WriteLine($"3DS> ReportService: Duplicate Point {civilPoint.PointNumber}");
                                continue;
                            }

                            // Add point to duplicate list.
                            pointList.Add(civilPoint);

                            // add data to the row object.

                            if (ColumnProperties.ShowPointNumber)
                                rowData.Add(civilPoint.PointNumber);

                            if (ColumnProperties.ShowEasting)
                                rowData.Add(Math.Round(civilPoint.Easting,
                                    pointGroupOption.CivilPointGroupProperties.DecimalPlaces));

                            if (ColumnProperties.ShowNorthing)
                                rowData.Add(Math.Round(civilPoint.Northing,
                                    pointGroupOption.CivilPointGroupProperties.DecimalPlaces));

                            if (ColumnProperties.ShowElevation)
                                rowData.Add(Math.Round(civilPoint.Elevation,
                                    pointGroupOption.CivilPointGroupProperties.DecimalPlaces));

                            if (ColumnProperties.ShowRawDescription)
                                rowData.Add(civilPoint.RawDescription);

                            if (ColumnProperties.ShowFullDescription)
                                rowData.Add(civilPoint.FullDescription);

                            // add alignment data
                            foreach (ReportCivilAlignmentOptions alignmentOption in CivilAlignmentOptions)
                            {
                                if (!alignmentOption.CivilAlignment.IsSelected)
                                {
                                    continue;
                                }

                                var stationOffset = AlignmentUtils.GetStationOffset(
                                    tr,
                                    alignmentOption.CivilAlignment,
                                    civilPoint.Easting,
                                    civilPoint.Northing);

                                // Round station and offset values.
                                double station = Math.Round(
                                    stationOffset.Station,
                                    alignmentOption.CivilAlignmentProperties.StationDecimalPlaces);

                                double offset = Math.Round(
                                    stationOffset.Offset,
                                    alignmentOption.CivilAlignmentProperties.OffsetDecimalPlaces);

                                rowData.Add(station);
                                rowData.Add(offset);
                            }

                            // add surface data
                            foreach (ReportCivilSurfaceOptions surfaceOption in CivilSurfaceOptions)
                            {
                                if (!surfaceOption.CivilSurface.IsSelected)
                                {
                                    continue;
                                }

                                // Add surface elevation (interpolate if we need to)
                                double elevation = surfaceOption.CivilSurfaceProperties.InterpolateLevels ?
                                    SurfaceUtils.FindElevationNearSurface(
                                        surfaceOption.CivilSurface.ToTinSurface(tr),
                                        civilPoint.Easting,
                                        civilPoint.Northing,
                                        surfaceOption.CivilSurfaceProperties.InterpolateMaximumDistance) :
                                    SurfaceUtils.FindElevationOnSurface(
                                        surfaceOption.CivilSurface.ToTinSurface(tr),
                                        civilPoint.Easting,
                                        civilPoint.Northing);

                                rowData.Add(Math.Round(elevation, surfaceOption.CivilSurfaceProperties.DecimalPlaces));

                                // Add cut and fill (invert if we need to)
                                if (surfaceOption.CivilSurfaceProperties.ShowCutFill)
                                {
                                    double cutAndFill = surfaceOption.CivilSurfaceProperties.InvertCutFill ?
                                        civilPoint.Elevation - elevation :
                                        elevation - civilPoint.Elevation;

                                    rowData.Add(Math.Round(cutAndFill,
                                        surfaceOption.CivilSurfaceProperties.DecimalPlaces));
                                }
                            }
                        }
                        // Add the row to the DataTable.
                        DataTable.Rows.Add(rowData.ToArray());

                        rowData.Clear(); // is clear faster than new?
                    }
                }
                tr.Commit();
            }
        }
    }
}
