using Autodesk.AutoCAD.DatabaseServices;

namespace _3DS_CivilSurveySuite.CIVIL
{
    public sealed class BreaklineData
    {
        public string Description { get; set; }

        public double MidOrdinateDistance { get; set; }

        public double MaximumDistance { get; set; }

        public double WeedingDistance { get; set; }


        public double WeedingAngle { get; set; }

        public ObjectIdCollection ObjectIds { get; set; }


    }
}
