using System.Xml.Serialization;

namespace AroFloApi
{
    /// <summary>AroFloResponse class</summary>
    /// <typeparam name="TZoneResult">The type of the t zone result.</typeparam>
    /// <typeparam name="TAroFloObject">The type of the t aro flo object.</typeparam>
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
