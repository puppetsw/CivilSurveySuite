// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class CogoPointSurfaceReportService : ICogoPointSurfaceReportService
    {
        public IEnumerable<CivilSite> GetSites()
        {
            return SiteUtils.GetCivilSites();
        }

        public IEnumerable<CivilAlignment> GetSiteAlignments(CivilSite site)
        {
            return AlignmentUtils.GetCivilAlignmentsInCivilSite(site);
        }

        public IEnumerable<CivilAlignment> GetAlignments() => AlignmentUtils.GetCivilAlignments();

        public IEnumerable<CivilPointGroup> GetPointGroups() => PointGroupUtils.GetCivilPointGroups();

        public IEnumerable<CivilSurface> GetSurfaces() => SurfaceUtils.GetCivilSurfaces();

        public CivilAlignment SelectAlignment() => AlignmentUtils.SelectCivilAlignment();

        public CivilPointGroup SelectPointGroup() => PointGroupUtils.SelectCivilPointGroup();

        public CivilSurface SelectSurface() => SurfaceUtils.SelectCivilSurface();

        public Task<List<ReportObject>> GetReportData(CivilPointGroup pointGroup, CivilAlignment alignment,
            CivilSurface surface, bool calculatePointNearSurfaceEdge)
        {
            if (pointGroup == null)
                throw new ArgumentNullException(nameof(pointGroup));

            var reportObjects = new List<ReportObject>();
            using (var tr = AcadApp.StartTransaction())
            {
                var points = new List<CivilPoint>(CogoPointUtils.GetCivilPointsFromPointGroup(tr, pointGroup.Name));

                foreach (var point in points)
                {
                    var reportObject = new ReportObject(point.PointNumber)
                    {
                        Easting = point.Easting,
                        Northing = point.Northing,
                        PointElevation = point.Elevation,
                        RawDescription = point.RawDescription,
                        FullDescription = point.FullDescription,
                    };

                    if (alignment != null)
                    {
                        reportObject.StationOffset = AlignmentUtils.GetStationOffset(tr, alignment,
                            point.Easting, point.Northing);
                    }

                    if (surface != null)
                    {
                        double reportObjectCalculatedDeltaX = 0.0d;
                        double reportObjectCalculatedDeltaY = 0.0d;

                        if (calculatePointNearSurfaceEdge)
                        {
                            reportObject.SurfaceElevation = SurfaceUtils.FindElevationNearSurface(surface.ToTinSurface(tr),
                                point.Easting, point.Northing, out reportObjectCalculatedDeltaX, out reportObjectCalculatedDeltaY);
                        }
                        else
                        {
                            try
                            {
                                reportObject.SurfaceElevation = surface.ToTinSurface(tr).FindElevationAtXY(point.Easting, point.Northing);
                            }
                            catch
                            {
                                reportObject.SurfaceElevation = -9999.999;
                            }
                        }

                        reportObject.CalculatedDeltaX = reportObjectCalculatedDeltaX;
                        reportObject.CalculatedDeltaY = reportObjectCalculatedDeltaY;
                    }

                    reportObjects.Add(reportObject);
                }
                tr.Commit();
            }
            return Task.FromResult(reportObjects);
        }
    }
}