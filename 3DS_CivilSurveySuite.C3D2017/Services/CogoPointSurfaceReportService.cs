// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class CogoPointSurfaceReportService : ICogoPointSurfaceReportService
    {
        public IEnumerable<CivilAlignment> GetAlignments() => AlignmentUtils.GetCivilAlignments();

        public IEnumerable<CivilPointGroup> GetPointGroups() => PointGroupUtils.GetCivilPointGroups();

        public IEnumerable<CivilSurface> GetSurfaces() => SurfaceUtils.GetCivilSurfaces();

        public CivilAlignment SelectAlignment() => AlignmentUtils.SelectCivilAlignment();

        public CivilPointGroup SelectPointGroup() => PointGroupUtils.SelectCivilPointGroup();

        public CivilSurface SelectSurface() => SurfaceUtils.SelectCivilSurface();

        public IEnumerable<CivilPoint> GetPointsInPointGroup(CivilPointGroup pointGroup) => CogoPointUtils.GetCivilPointsFromPointGroup(pointGroup.Name);

        public double GetElevationAtCivilPoint(CivilPoint civilPoint, CivilSurface civilSurface, bool calculatePointNearSurfaceEdge)
        {
            double elevation;
            using (var tr = AcadApp.StartTransaction())
            {
                if (calculatePointNearSurfaceEdge)
                {
                    elevation = SurfaceUtils.FindElevationNearSurface(civilSurface.ToSurface(tr), civilPoint.Easting, civilPoint.Northing);
                }
                else
                {
                    try
                    {
                        elevation = civilSurface.ToSurface(tr).FindElevationAtXY(civilPoint.Easting, civilPoint.Northing);
                    }
                    catch
                    {
                        elevation = -9999.9999;
                    }
                }

                tr.Commit();
            }
            return elevation;
        }

        public StationOffset GetStationOffsetAtCivilPoint(CivilPoint civilPoint, CivilAlignment civilAlignment)
        {
            return AlignmentUtils.GetStatoinOffset(civilAlignment, civilPoint.Easting, civilPoint.Northing);
        }

    }
}