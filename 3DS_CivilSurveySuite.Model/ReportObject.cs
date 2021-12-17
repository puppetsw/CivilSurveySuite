namespace _3DS_CivilSurveySuite.Model
{
    /// <summary>
    ///
    /// </summary>
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

        public double CalculatedDeltaX { get; set; }

        public double CalculatedDeltaY { get; set; }

        public ReportObject(uint pointNumber)
        {
            PointNumber = pointNumber;
        }
    }
}