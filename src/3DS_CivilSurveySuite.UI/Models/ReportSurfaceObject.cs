namespace _3DS_CivilSurveySuite.UI.Models
{
    public class ReportSurfaceObject : ObservableObject
    {
        public CivilSurface Surface { get; }

        public CivilPoint Point { get; }

        public CivilPoint ComparisonPoint { get; }

        public bool InvertCutFill { get; set; }

        public ReportSurfaceObject(CivilSurface surface, CivilPoint point, CivilPoint comparisonPoint = null)
        {
            Surface = surface;
            Point = point;
            ComparisonPoint = comparisonPoint;
        }
    }
}
