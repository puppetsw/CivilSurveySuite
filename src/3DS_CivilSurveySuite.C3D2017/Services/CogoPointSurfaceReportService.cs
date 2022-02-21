using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services;
using Autodesk.Civil;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class CogoPointSurfaceReportService : ICogoPointSurfaceReportService
    {
        public bool Interpolate { get; set; }
        public double MaximumInterpolationDistance { get; set; }

        public CogoPointSurfaceReportService()
        {
            // Set default maximum interpolation distance to half a meter.
            MaximumInterpolationDistance = 0.5;
        }

        // ReSharper disable once CognitiveComplexity
        public Task<ObservableCollection<ReportObject>> GetReportData(CivilAlignment alignment, IEnumerable<CivilPointGroup> pointGroups, IEnumerable<CivilSurface> surfaces)
        {
            // null checks
            if (pointGroups == null)
            {
                throw new ArgumentNullException(nameof(pointGroups));
            }

            var reportObjects = new ObservableCollection<ReportObject>();

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (CivilPointGroup civilPointGroup in pointGroups)
                {
                    var points = new List<CivilPoint>(CogoPointUtils.GetCivilPointsFromPointGroup(tr, civilPointGroup.Name));

                    foreach (CivilPoint civilPoint in points)
                    {
                        var reportObject = new ReportObject(civilPoint);

                        // check if the point already exists in our collection.
                        var compare = reportObjects.FirstOrDefault(p => p.Point.Equals(civilPoint));
                        if (compare != null && compare.Point.Equals(civilPoint))
                        {
                            continue; // continue if it does.
                        }

                        if (alignment != null)
                        {
                            reportObject.StationOffset = AlignmentUtils.GetStationOffset(tr, alignment, civilPoint.Easting, civilPoint.Northing);
                        }

                        if (surfaces != null)
                        {
                            // ReSharper disable once PossibleMultipleEnumeration
                            foreach (CivilSurface civilSurface in surfaces)
                            {
                                // reportObject.SurfacePoints.Add();

                                if (Interpolate)
                                {
                                    // TODO: add out distances for interpolation amount
                                    var interpolatedElevation = SurfaceUtils.FindElevationNearSurface(civilSurface.ToTinSurface(tr), civilPoint.Easting, civilPoint.Northing, out _, out _);
                                    var newPoint = civilPoint.Clone();
                                    newPoint.Elevation = interpolatedElevation;
                                    reportObject.SurfacePoints.Add(new ReportSurfaceObject(civilSurface, newPoint));

                                }
                                else
                                {
                                    try
                                    {
                                        var elevation = civilSurface.ToTinSurface(tr).FindElevationAtXY(civilPoint.Easting, civilPoint.Northing);
                                        var newPoint = civilPoint.Clone();
                                        newPoint.Elevation = elevation;
                                        //reportObject.SurfacePoints.Add(civilSurface, newPoint);
                                        reportObject.SurfacePoints.Add(new ReportSurfaceObject(civilSurface, newPoint));
                                    }
                                    catch (PointNotOnEntityException)
                                    {
                                        // reportObject.SurfaceElevation = -9999.999;
                                    }
                                }

                                //TODO: add calculation distance
                            }
                        }
                        reportObjects.Add(reportObject);
                    }
                }

                tr.Commit();
            }

            return Task.FromResult(reportObjects);
        }
    }
}