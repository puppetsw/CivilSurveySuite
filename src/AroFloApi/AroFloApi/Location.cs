using System.Xml.Serialization;

namespace AroFloApi
{
    public class Location : AroFloObject
    {
        [XmlElement("locationid")]
        public string LocationId { get; set; }

        [XmlElement("locationname")]
        public string LocationName { get; set; }

        [XmlElement("gpslat")]
        public double Latitude { get; set; }

        [XmlElement("gpslong")]
        public double Longitude { get; set; }

        [XmlElement("postcode")]
        public string PostCode { get; set; }

        [XmlElement("state")]
        public string State { get; set; }

        [XmlElement("country")]
        public string Country { get; set; }

        [XmlElement("suburb")]
        public string Suburb { get; set; }

        [XmlElement("archived")]
        public string ArchivedString { get; set; }

        [XmlIgnore]
        public bool IsArchived => !ArchivedString.ToLower().Equals("false");

        [XmlElement("address")]
        public string Address { get; set; }
    }
}

// <location>
//   <locationid>JSYqKyBSXFggCg==</locationid>
//   <gpslat>-34.7293199477</gpslat>
//   <postcode>5110</postcode>
//   <sitecontact></sitecontact>
//   <state>SA</state>
//   <suburb>Burton</suburb>
//   <siteemail></siteemail>
//   <customfields></customfields>
//   <linkedto>
//   <linkedtoname>Boral Asphalt SA</linkedtoname>
//   <linkedtoid>JSc6Uy1QTFwgCg==</linkedtoid>
//   <linkedtotype>client</linkedtotype>
//   </linkedto>
//   <locationname>North HUB - Waterloo Corner Road</locationname>
//   <notes></notes>
//   <country>AUSTRALIA</country>
//   <gpslong>138.580861384</gpslong>
//   <address></address>
//   <documentsandphotos></documentsandphotos>
//   <archived>FALSE</archived>
//   <sitephone></sitephone>
// </location>
