using System.Xml.Serialization;

namespace AroFloApi.Models
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
        public string ProjectId { get; set; }

        [XmlElement("projectnumber")]
        public int ProjectNumber { get; set; }

        [XmlElement("projectname")]
        public string ProjectName { get; set; }

        [XmlElement("projecttype")]
        public ProjectType ProjectType { get; set; }

        [XmlElement("custon")]
        public string CustomerOrderNumber { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

    }
}

/*<project>
    <location>
    <locationid></locationid>
    <locationname></locationname>
    </location>
    <manageruser>
    <userid>JScqKyBSXFggCg==</userid>
    <username>Kappa Business Solutions</username>
    </manageruser>
    <startdate>2019/06/29</startdate>
    <org>
    <orgid>JSc6WyNRPEggCg==</orgid>
    <orgname>3D Surveys</orgname>
    </org>
    <projecttype>
    <projecttypeid>IyYqSyEK</projecttypeid>
    <projecttype>Commercial</projecttype>
    </projecttype>
    <client>
    <orgid>JSc6UyNSXEAgCg==</orgid>
    <orgname>Training Client</orgname>
    </client>
    <enddate>2019/07/08</enddate>
    <closeddate></closeddate>
    <status>open</status>
    <insertedbyuser>
    <userid>JScqKyBSXFggCg==</userid>
    <username>Kappa Business Solutions</username>
    </insertedbyuser>
    <description>
    <![CDATA[]]>
    </description>
    <stages></stages>
    <projectid>IydaTyYK</projectid>
    <refno>traini1</refno>
    <contactuser>
    <userid>JScqSyFQXEAgCg==</userid>
    <username>Accounts Payable</username>
    </contactuser>
    <custon></custon>
    <projectnumber>3</projectnumber>
    <substatus>
    <substatusid></substatusid>
    <substatus></substatus>
    </substatus>
    <projectname>Kappa Test</projectname>
</project>*/