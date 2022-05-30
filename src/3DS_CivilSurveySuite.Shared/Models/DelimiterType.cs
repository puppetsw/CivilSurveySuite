using System.ComponentModel;

namespace _3DS_CivilSurveySuite.Shared.Models
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