using System.Xml.Serialization;

namespace AroFloApi
{
    public class Project : AroFloObject
    {
        [XmlElement("status")]
        public string Status { get; set; }

        [XmlElement("client")]
        public Client Client { get; set; }

        [XmlElement("location")]
        public Location Location { get; set; }

        [XmlElement("projectid")]
        public string Id { get; set; }

        [XmlElement("projectnumber")]
        public string ProjectNumber { get; set; }

        [XmlElement("projectname")]
        public string ProjectName { get; set; }

        [XmlElement("projecttype")]
        public ProjectType ProjectType { get; set; }

    }
}
