using System.ComponentModel;

namespace CivilSurveySuite.Common.Models
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