using System.ComponentModel;

namespace CivilSurveySuite.Shared.Models
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