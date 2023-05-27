namespace CivilSurveySuite.Shared.Models
{
    public class ReportCivilSurfaceOptions
    {
        public CivilSurface CivilSurface { get; set; }

        public CivilSurfaceProperties CivilSurfaceProperties { get; set; }
    }

    public class ReportCivilAlignmentOptions
    {
        public CivilAlignment CivilAlignment { get; set; }

        public CivilAlignmentProperties CivilAlignmentProperties { get; set;}
    }

    public class ReportCivilPointGroupOptions
    {
        public CivilPointGroup CivilPointGroup { get; set; }

        public CivilPointGroupProperties CivilPointGroupProperties { get; set; }
    }
}
