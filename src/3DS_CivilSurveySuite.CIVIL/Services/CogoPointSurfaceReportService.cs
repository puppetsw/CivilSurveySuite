using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using _3DS_CivilSurveySuite.ACAD;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using _3DS_CivilSurveySuite.UI.Helpers;
using DataTable = System.Data.DataTable;

namespace _3DS_CivilSurveySuite.CIVIL.Services
{
    public class CogoPointSurfaceReportService : ObservableObject, ICogoPointSurfaceReportService
    {
        private readonly ICivilSelectService _civilSelectService;
        private ColumnProperties _columnProperties;
        private bool _allowDuplicatePoints;
        private DelimiterType _delimiter;
        private bool _outputHeaders;

        public DataTable DataTable { get; private set; }

        public DelimiterType Delimiter
        {
            get => _delimiter;
            set => SetProperty(ref _delimiter, value);
        }

        public bool AllowDuplicatePoints
        {
            get => _allowDuplicatePoints;
            set => SetProperty(ref _allowDuplicatePoints, value);
        }

        public ColumnProperties ColumnProperties
        {
            get => _columnProperties;
            private set => SetProperty(ref _columnProperties, value);
        }

        public bool OutputHeaders
        {
            get => _outputHeaders;
            set => SetProperty(ref _outputHeaders, value);
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

            Delimiter = DelimiterType.Comma;

            ColumnProperties = new ColumnProperties();

            LoadPointGroups();
            LoadAlignments();
            LoadSurfaces();
        }

        public void MoveUp(ColumnHeader header)
        {
            if (!ColumnProperties.Headers.Contains(header))
            {
                return;
            }

            var currentIndex = ColumnProperties.Headers.IndexOf(header);
            if (currentIndex >= 1)
            {
                ColumnProperties.Headers.Move(currentIndex, currentIndex - 1);
            }
        }

        public void MoveDown(ColumnHeader header)
        {
            if (!ColumnProperties.Headers.Contains(header))
            {
                return;
            }

            var currentIndex = ColumnProperties.Headers.IndexOf(header);
            if (currentIndex < ColumnProperties.Headers.Count)
            {
                ColumnProperties.Headers.Move(currentIndex, currentIndex + 1);
            }
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
            foreach (ColumnHeader columnHeader in ColumnProperties.Headers)
            {
                if (!columnHeader.IsVisible)
                {
                    continue;
                }

                switch (columnHeader.ColumnType)
                {
                    case ColumnType.PointNumber:
                        DataTable.Columns.Add(columnHeader.HeaderText, typeof(uint));
                        break;
                    case ColumnType.Easting:
                    case ColumnType.Northing:
                    case ColumnType.Elevation:
                    case ColumnType.Station:
                    case ColumnType.Offset:
                    case ColumnType.SurfaceElevation:
                    case ColumnType.SurfaceCutFill:
                        DataTable.Columns.Add(columnHeader.HeaderText, typeof(double));
                        break;
                    case ColumnType.RawDescription:
                    case ColumnType.FullDescription:
                        DataTable.Columns.Add(columnHeader.HeaderText, typeof(string));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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
                        if (!AllowDuplicatePoints)
                        {
                            // check for duplicate point.
                            if (pointList.Contains(civilPoint))
                            {
                                // Log duplicate point
                                AcadApp.WriteMessage($"3DS> {nameof(BuildDataTableRows)} Duplicate:{civilPoint.PointNumber}");
                                Debug.WriteLine($"3DS> {nameof(BuildDataTableRows)} Duplicate:{civilPoint.PointNumber}");
                                continue;
                            }
                        }

                        // Add point to duplicate list.
                        pointList.Add(civilPoint);

                        // add data to the row object.
                        foreach (ColumnHeader columnHeader in ColumnProperties.Headers)
                        {
                            switch (columnHeader.ColumnType)
                            {
                                case ColumnType.PointNumber:
                                    rowData.Add(civilPoint.PointNumber);
                                    break;
                                case ColumnType.Easting:
                                    rowData.Add(Math.Round(civilPoint.Easting,
                                        pointGroupOption.CivilPointGroupProperties.DecimalPlaces));
                                    break;
                                case ColumnType.Northing:
                                    rowData.Add(Math.Round(civilPoint.Northing,
                                        pointGroupOption.CivilPointGroupProperties.DecimalPlaces));
                                    break;
                                case ColumnType.Elevation:
                                    rowData.Add(Math.Round(civilPoint.Elevation,
                                        pointGroupOption.CivilPointGroupProperties.DecimalPlaces));
                                    break;
                                case ColumnType.RawDescription:
                                    rowData.Add(civilPoint.RawDescription);
                                    break;
                                case ColumnType.FullDescription:
                                    rowData.Add(civilPoint.FullDescription);
                                    break;
                                case ColumnType.Station:
                                {
                                    var stationOption = CivilAlignmentOptions.FirstOrDefault(p =>
                                        p.CivilAlignment.Name.Equals(columnHeader.Key) && p.CivilAlignment.IsSelected);

                                    if (stationOption != null)
                                    {
                                        var stationOffset = AlignmentUtils.GetStationOffset(
                                            tr,
                                            stationOption.CivilAlignment,
                                            civilPoint.Easting,
                                            civilPoint.Northing);

                                        double station = Math.Round(
                                            stationOffset.Station,
                                            stationOption.CivilAlignmentProperties.StationDecimalPlaces);

                                        rowData.Add(station);
                                    }
                                    break;
                                }
                                case ColumnType.Offset:
                                {
                                    var offsetOption = CivilAlignmentOptions.FirstOrDefault(p =>
                                        p.CivilAlignment.Name.Equals(columnHeader.Key) && p.CivilAlignment.IsSelected);

                                    if (offsetOption != null)
                                    {
                                        var stationOffset = AlignmentUtils.GetStationOffset(
                                            tr,
                                            offsetOption.CivilAlignment,
                                            civilPoint.Easting,
                                            civilPoint.Northing);

                                        double offset = Math.Round(
                                            stationOffset.Offset,
                                            offsetOption.CivilAlignmentProperties.OffsetDecimalPlaces);

                                        rowData.Add(offset);
                                    }

                                    break;
                                }
                                case ColumnType.SurfaceElevation:
                                {
                                    var surfaceElOption = CivilSurfaceOptions.FirstOrDefault(
                                        p => p.CivilSurface.Name.Equals(columnHeader.Key) && p.CivilSurface.IsSelected);

                                    // Add surface elevation (interpolate if we need to)
                                    double elevation = surfaceElOption.CivilSurfaceProperties.InterpolateLevels ?
                                        SurfaceUtils.FindElevationNearSurface(
                                            surfaceElOption.CivilSurface.ToTinSurface(tr),
                                            civilPoint.Easting,
                                            civilPoint.Northing,
                                            surfaceElOption.CivilSurfaceProperties.InterpolateMaximumDistance) :
                                        SurfaceUtils.FindElevationOnSurface(
                                            surfaceElOption.CivilSurface.ToTinSurface(tr),
                                            civilPoint.Easting,
                                            civilPoint.Northing);

                                    rowData.Add(Math.Round(elevation,
                                        surfaceElOption.CivilSurfaceProperties.DecimalPlaces));

                                    break;
                                }
                                case ColumnType.SurfaceCutFill:
                                {
                                    var surfaceElOption = CivilSurfaceOptions.FirstOrDefault(
                                        p => p.CivilSurface.Name.Equals(columnHeader.Key) && p.CivilSurface.IsSelected);

                                    // Add surface elevation (interpolate if we need to)
                                    double elevation = surfaceElOption.CivilSurfaceProperties.InterpolateLevels ?
                                        SurfaceUtils.FindElevationNearSurface(
                                            surfaceElOption.CivilSurface.ToTinSurface(tr),
                                            civilPoint.Easting,
                                            civilPoint.Northing,
                                            surfaceElOption.CivilSurfaceProperties.InterpolateMaximumDistance) :
                                        SurfaceUtils.FindElevationOnSurface(
                                            surfaceElOption.CivilSurface.ToTinSurface(tr),
                                            civilPoint.Easting,
                                            civilPoint.Northing);

                                    double cutAndFill = surfaceElOption.CivilSurfaceProperties.InvertCutFill ?
                                        civilPoint.Elevation - elevation :
                                        elevation - civilPoint.Elevation;

                                    rowData.Add(Math.Round(cutAndFill,
                                        surfaceElOption.CivilSurfaceProperties.DecimalPlaces));

                                    break;
                                }
                                default:
                                    throw new ArgumentOutOfRangeException();
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

        public void BuildColumnHeaders()
        {
            // Clear existing columns
            ColumnProperties.Headers.Clear();
            // Add default columns in.
            ColumnProperties.LoadDefaultHeaders();

            // Add alignment details.
            foreach (ReportCivilAlignmentOptions alignmentOption in CivilAlignmentOptions)
            {
                if (!alignmentOption.CivilAlignment.IsSelected)
                {
                    continue;
                }
                // Add alignment name column.
                ColumnProperties.Headers.Add(new ColumnHeader
                {
                    HeaderText = $"{alignmentOption.CivilAlignment.Name} {ResourceHelpers.GetLocalisedString("Station")}",
                    IsVisible = true,
                    ColumnType = ColumnType.Station, Key = alignmentOption.CivilAlignment.Name
                });
                // Add offset column
                ColumnProperties.Headers.Add(new ColumnHeader
                {
                    HeaderText = $"{alignmentOption.CivilAlignment.Name} {ResourceHelpers.GetLocalisedString("Offset")}",
                    IsVisible = true,
                    ColumnType = ColumnType.Offset, Key = alignmentOption.CivilAlignment.Name
                });
            }

            // Loop and add selected surfaces.
            foreach (ReportCivilSurfaceOptions surfaceOption in CivilSurfaceOptions)
            {
                if (!surfaceOption.CivilSurface.IsSelected)
                {
                    continue;
                }

                // Add surface elevation column.
                ColumnProperties.Headers.Add(new ColumnHeader
                {
                    HeaderText = $"{surfaceOption.CivilSurface.Name} {ResourceHelpers.GetLocalisedString("Elevation")}",
                    IsVisible = true,
                    ColumnType = ColumnType.SurfaceElevation, Key = surfaceOption.CivilSurface.Name
                });

                // Add cut and fill column if it's enabled.
                if (surfaceOption.CivilSurfaceProperties.ShowCutFill)
                {
                    ColumnProperties.Headers.Add(new ColumnHeader
                    {
                        HeaderText = $"{surfaceOption.CivilSurface.Name} {ResourceHelpers.GetLocalisedString("CutFill")}",
                        IsVisible = true,
                        ColumnType = ColumnType.SurfaceCutFill, Key = surfaceOption.CivilSurface.Name
                    });
                }
            }
        }

        public string WriteDataTable()
        {
            StringBuilder sb = new StringBuilder();

            // Write column headings.
            for (int i = 0; i < DataTable.Columns.Count; i++)
            {
                sb.Append(DataTable.Columns[i]);
                if (i < DataTable.Columns.Count - 1)
                {
                    sb.Append(SelectDelimiter());
                }
            }
            sb.AppendLine();

            // Write data
            for (int i = 0; i < DataTable.Rows.Count; i++)
            {
                for (int j = 0; j < DataTable.Columns.Count; j++)
                {
                    sb.Append(DataTable.Rows[i][j]);

                    if (j < DataTable.Columns.Count - 1)
                    {
                        sb.Append(SelectDelimiter());
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private char SelectDelimiter()
        {
            switch (Delimiter)
            {
                case DelimiterType.Comma:
                    return ',';
                case DelimiterType.Space:
                    return ' ';
                case DelimiterType.Tab:
                    return '\t';
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
