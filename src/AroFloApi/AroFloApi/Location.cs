using System.Xml.Serialization;

namespace AroFloApi
{
    public class Location
    {
        [XmlElement("locationid")]
        public string Id { get; set; }

        [XmlElement("locationname")]
        public string Address { get; set; }
    }
}
