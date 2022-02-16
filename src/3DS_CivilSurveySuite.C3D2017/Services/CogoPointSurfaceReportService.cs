// ----------------------------------------------------------------------
//  <copyright file="CogoPointSurfaceReportService.cs" company="Scott Whitney">
//      Author: Scott Whitney
//      Copyright (c) Scott Whitney. All rights reserved.
//  </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services;
using Autodesk.AutoCAD.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class CogoPointSurfaceReportService : ICogoPointSurfaceReportService
    {
        public IEnumerable<CivilSite> GetSites() => SiteUtils.GetCivilSites();

        public IEnumerable<CivilAlignment> GetSiteAlignments(CivilSite site) =>
            AlignmentUtils.GetCivilAlignmentsInCivilSite(site);

        public IEnumerable<CivilAlignment> GetAlignments() => AlignmentUtils.GetCivilAlignments();

        public IEnumerable<CivilPointGroup> GetPointGroups() => PointGroupUtils.GetCivilPointGroups();

        public IEnumerable<CivilSurface> GetSurfaces() => SurfaceUtils.GetCivilSurfaces();

        public CivilAlignment SelectAlignment() => AlignmentUtils.SelectCivilAlignment();

        public CivilPointGroup SelectPointGroup() => PointGroupUtils.SelectCivilPointGroup();

        public CivilSurface SelectSurface() => SurfaceUtils.SelectCivilSurface();

        public Task<ObservableCollection<ReportObject>> GetReportData(CivilPointGroup pointGroup,
            CivilAlignment alignment, CivilSurface surface, bool calculatePointNearSurfaceEdge)
        {
            if (pointGroup == null)
                throw new ArgumentNullException(nameof(pointGroup));

            var reportObjects = new ObservableCollection<ReportObject>();
            using (var tr = AcadApp.StartTransaction())
            {
                var points = new List<CivilPoint>(CogoPointUtils.GetCivilPointsFromPointGroup(tr, pointGroup.Name));

                foreach (var point in points)
                {
                    var reportObject = CreateReportObject(tr, point, alignment, surface, calculatePointNearSurfaceEdge);

                    reportObjects.Add(reportObject);
                }

                tr.Commit();
            }
            return Task.FromResult(reportObjects);
        }

        private static ReportObject CreateReportObject(Transaction tr, CivilPoint point, CivilAlignment alignment,
            CivilSurface surface, bool calculatePointNearSurfaceEdge)
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

            UpdateSurface(tr, reportObject, point, surface, calculatePointNearSurfaceEdge);

            return reportObject;
        }

        private static void UpdateSurface(Transaction tr, ReportObject reportObject, CivilPoint point,
            CivilSurface surface, bool calculatePointNearSurfaceEdge)
        {
            if (surface == null)
                return;

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
                    reportObject.SurfaceElevation =
                        surface.ToTinSurface(tr).FindElevationAtXY(point.Easting, point.Northing);
                }
                catch
                {
                    reportObject.SurfaceElevation = -9999.999;
                }
            }

            reportObject.CalculatedDeltaX = reportObjectCalculatedDeltaX;
            reportObject.CalculatedDeltaY = reportObjectCalculatedDeltaY;
        }
    }
}