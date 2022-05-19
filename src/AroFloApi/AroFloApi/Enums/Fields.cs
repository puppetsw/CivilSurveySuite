using System;
using System.ComponentModel;

namespace AroFloApi.Enums
{
    public enum Field
    {
        // Project Fields
        [Description("projectid")]
        ProjectId,
        [Description("projectnumber")]
        ProjectNumber,
        [Description("projectname")]
        ProjectName,

        [Obsolete("The status field doesn't seem to currently work with the filter system.")]
        [Description("status")]
        Status,

        // Location Fields
        [Description("locationid")]
        LocationId
    }
}
