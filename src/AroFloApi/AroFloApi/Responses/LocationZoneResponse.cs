using System.Collections.Generic;
using System.Xml.Serialization;
using AroFloApi.Models;

namespace AroFloApi.Responses
{
    public sealed class LocationZoneResponse : ZoneResponse<Location>
    {
        [XmlArrayItem("location")]
        [XmlArray("locations")]
        public List<Location> Results { get; set; }

        public override IEnumerable<Location> GetContent()
        {
            return Results;
        }
    }
}
