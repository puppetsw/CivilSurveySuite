using System.ComponentModel;

namespace AroFloApi
{
    /// <summary>
    /// AroFlo Zones
    /// </summary>
    internal enum Zones
    {
        None = 0,
        [Description("projects")]
        Projects,
        [Description("locations")]
        Locations
    }
}
