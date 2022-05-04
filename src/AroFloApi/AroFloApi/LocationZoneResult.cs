using System.Collections.Generic;
using System.Xml.Serialization;

namespace AroFloApi
{
    public sealed class LocationZoneResult : ZoneResult<Location>
    {
        [XmlArrayItem("location")]
        [XmlArray("locations")]
        public List<Location> Results { get; set; }

        public override IEnumerable<Location> GetResults()
        {
            return Results;
        }
    }
}
