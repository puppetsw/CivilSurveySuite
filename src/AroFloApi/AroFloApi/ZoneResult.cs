using System.Collections.Generic;
using System.Xml.Serialization;

namespace AroFloApi
{
    public abstract class ZoneResult<T> where T : AroFloObject
    {
        [XmlElement("maxpageresults")]
        public int MaxPageResults { get; set; }

        [XmlElement("pagenumber")]
        public int PageNumber { get; set; }

        [XmlElement("currentpageresults")]
        public int CurrentPageResults { get; set; }

        [XmlElement("generatedisplayresponsetime")]
        public int GeneratedDisplayResponseTime { get; set; }

        [XmlElement("queryresponsetimes")]
        public int QueryResponseTime { get; set; }

        [XmlIgnore]
        public bool IsMorePages => CurrentPageResults == MaxPageResults;

        public abstract IEnumerable<T> GetResults();

    }
}