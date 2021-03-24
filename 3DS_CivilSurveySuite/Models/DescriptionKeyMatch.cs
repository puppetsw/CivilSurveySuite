using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DescriptionKeyMatch
    {
        public int LineNumber { get; set; }
        public CogoPoint CogoPoint { get; set; }
        public DescriptionKey DescriptionKey { get; set; }
    }
}
