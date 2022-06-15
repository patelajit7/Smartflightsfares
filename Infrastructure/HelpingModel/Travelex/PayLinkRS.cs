using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Infrastructure.HelpingModel.Travelex
{
    
        [XmlRoot(ElementName = "PaymentConfigurationResponse", Namespace = "http://www.travelexinsurance.com/")]
        public class PayLinkRS
        {
            [XmlElement(ElementName = "TsepUrl", Namespace = "http://www.travelexinsurance.com/")]
            public string TsepUrl { get; set; }
            [XmlElement(ElementName = "Status", Namespace = "http://www.travelexinsurance.com/")]
            public string Status { get; set; }
            [XmlElement(ElementName = "ErrorMessage", Namespace = "http://www.travelexinsurance.com/")]
            public string ErrorMessage { get; set; }
            [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xsi { get; set; }
            [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
            public string Xsd { get; set; }
            [XmlAttribute(AttributeName = "xmlns")]
            public string Xmlns { get; set; }
        }
    
}
