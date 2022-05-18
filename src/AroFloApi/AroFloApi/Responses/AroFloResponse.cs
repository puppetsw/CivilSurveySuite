using System.Xml.Serialization;
using AroFloApi.Enums;
using AroFloApi.Models;

namespace AroFloApi.Responses
{
    [XmlRoot("imsapi")]
    public class AroFloResponse<TZoneResult, TAroFloObject>
        where TZoneResult : ZoneResponse<TAroFloObject>
        where TAroFloObject : AroFloObject
    {
        [XmlElement("status")]
        public Status Status { get; set; }

        [XmlElement("statusmessage")]
        public string StatusMessage { get; set; }

        [XmlElement("zoneresponse")]
        public TZoneResult ZoneResponse { get; set; }
    }
}
