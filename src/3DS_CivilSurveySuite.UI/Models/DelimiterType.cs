using System.ComponentModel;

namespace _3DS_CivilSurveySuite.UI.Models
{
    public enum DelimiterType
    {
        [Description(",")]
        Comma,
        [Description(" ")]
        Space,
        [Description("\t")]
        Tab
    }
}