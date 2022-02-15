// ------------------------------------------------------------------------------
//  <copyright file="ReportObject.cs" company="Scott Whitney">
//      Copyright (c) Scott Whitney.  All rights reserved.
//  </copyright>
// ------------------------------------------------------------------------------

using System.Collections.Generic;

namespace _3DS_CivilSurveySuite.Model
{
    public class ReportObject
    {
        public uint PointNumber { get; }
        public double Easting { get; set; }
        public double Northing { get; set; }
        public double PointElevation { get; set; }
        public string RawDescription { get; set; }
        public string FullDescription { get; set; }
        public double SurfaceElevation { get; set; }
        public StationOffset StationOffset { get; set; }

        public double CutFill => PointElevation - SurfaceElevation;

        public double CutFillInvert => SurfaceElevation - PointElevation;


        public List<SurfaceReportObject> SurfaceComparisons { get; set; }


        public double CalculatedDeltaX { get; set; }

        public double CalculatedDeltaY { get; set; }

        public ReportObject(uint pointNumber)
        {
            PointNumber = pointNumber;
        }
    }

    public class SurfaceReportObject
    {
        private readonly double _surfaceDifference;

        public bool InvertDifference { get; set; }

        public double SurfaceElevation1 { get; }

        public double SurfaceElevation2 { get; }

        public double SurfaceDifference
        {
            get
            {
                if (InvertDifference)
                {
                    return -_surfaceDifference;
                }
                return _surfaceDifference;
            }
        }

        public SurfaceReportObject(double surface1, double surface2)
        {
            SurfaceElevation1 = surface1;
            SurfaceElevation2 = surface2;
            _surfaceDifference = surface1 - surface2;
        }
    }
}