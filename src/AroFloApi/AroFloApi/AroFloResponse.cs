using System.Xml.Serialization;

namespace AroFloApi
{
    [XmlRoot("imsapi")]
    public class AroFloResponse<TZoneResult, TAroFloObject>
        where TZoneResult : ZoneResult<TAroFloObject>
        where TAroFloObject : AroFloObject
    {
        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("statusmessage")]
        public string StatusMessage { get; set; }

        [XmlElement("zoneresponse")]
        public TZoneResult ZoneResponse { get; set; }
    }
}
