using Autodesk.Civil.DatabaseServices;
using System.Collections.Generic;

namespace _3DS_CivilSurveySuite.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DescriptionKeyMatch
    {
        public string LineNumber { get; set; }
        public string Code { get; set; }
        public List<CogoPoint> CogoPoints { get; set; }
        public DescriptionKey DescriptionKey { get; set; }


        public Dictionary<string, List<CogoPoint>> MatchCollection { get; set; }

        public DescriptionKeyMatch()
        {
            MatchCollection = new Dictionary<string, List<CogoPoint>>();
            CogoPoints = new List<CogoPoint>();
            LineNumber = string.Empty;
        }
    }
}
