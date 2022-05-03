using System.Collections.Generic;
using System.Xml.Serialization;

namespace AroFloApi
{
    public class ProjectZoneResult : ZoneResult<Project>
    {
        [XmlArrayItem("project")]
        [XmlArray("projects")]
        public List<Project> Results { get; set; }

        public override IEnumerable<Project> GetResults()
        {
            return Results;
        }
    }
}