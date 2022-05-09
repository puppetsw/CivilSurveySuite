using System.Xml.Serialization;

namespace AroFloApi
{
    public class Client : AroFloObject
    {
        [XmlElement("orgid")]
        public string Id { get; set; }

        [XmlElement("orgname")]
        public string Name { get; set; }
    }
}
