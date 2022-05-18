using System.Collections.Generic;
using System.Xml.Serialization;
using AroFloApi.Models;

namespace AroFloApi.Responses
{
    public sealed class ProjectZoneResponse : ZoneResponse<Project>
    {
        [XmlArrayItem("project")]
        [XmlArray("projects")]
        public List<Project> Results { get; set; }

        public override IEnumerable<Project> GetContent()
        {
            return Results;
        }
    }
}
