using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.Services.Interfaces;
using DataTable = System.Data.DataTable;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class CogoPointSurfaceReportService : ICogoPointSurfaceReportService
    {
        private readonly ICivilSelectService _civilSelectService;

        public DataTable DataTable { get; private set; }

        public ObservableCollection<ReportCivilSurfaceOptions> CivilSurfaceOptions { get; }
            = new ObservableCollection<ReportCivilSurfaceOptions>();
        public ObservableCollection<ReportCivilAlignmentOptions> CivilAlignmentOptions { get; }
            = new ObservableCollection<ReportCivilAlignmentOptions>();
        public ObservableCollection<ReportCivilPointGroupOptions> CivilPointGroupOptions { get; }
            = new ObservableCollection<ReportCivilPointGroupOptions>();

        public CogoPointSurfaceReportService(ICivilSelectService civilSelectService)
        {
            _civilSelectService = civilSelectService;

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

            DataTable.Columns.Add("Point Number", typeof(uint));
            DataTable.Columns.Add("Easting", typeof(double));
            DataTable.Columns.Add("Northing", typeof(double));
            DataTable.Columns.Add("Elevation", typeof(double));
            DataTable.Columns.Add("Raw Description", typeof(string));

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
                        continue;

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
                                continue;
                            }

                            // Add point to duplicate list.
                            pointList.Add(civilPoint);

                            // add data to the row object.
                            rowData.Add(civilPoint.PointNumber);
                            rowData.Add(civilPoint.Easting);
                            rowData.Add(civilPoint.Northing);
                            rowData.Add(civilPoint.Elevation);
                            rowData.Add(civilPoint.RawDescription);

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

                                //TODO: work out way to add rounding.
                                rowData.Add(stationOffset.Station);
                                rowData.Add(stationOffset.Offset);
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
                                        civilPoint.Northing) :
                                    SurfaceUtils.FindElevationOnSurface(surfaceOption.CivilSurface.ToTinSurface(tr),
                                        civilPoint.Easting,
                                        civilPoint.Northing);

                                rowData.Add(elevation);

                                // Add cut and fill (invert if we need to)
                                if (surfaceOption.CivilSurfaceProperties.ShowCutFill)
                                {
                                    double cutfill = surfaceOption.CivilSurfaceProperties.InvertCutFill ?
                                        civilPoint.Elevation - elevation :
                                        elevation - civilPoint.Elevation;

                                    rowData.Add(cutfill);
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
