namespace _3DS_CivilSurveySuite.UI.Models
{
    public class ReportSurfaceObject : ObservableObject
    {
        public CivilSurface Surface { get; }

        public CivilPoint Point { get; }

        public ReportSurfaceObject(CivilSurface surface, CivilPoint point)
        {
            Surface = surface;
            Point = point;
        }
    }
}
