using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Configration
{
    public class PortalDetails
    {
        
        [XmlElement]
        public string PageTitleCompanyExtend { get; set; }
        [XmlElement]
        public string DefaultTollFreeNumber { get; set; }
        [XmlElement]
        public string CustomerSupportMail { get; set; }
        [XmlElement]
        public string Email { get; set; }
        [XmlElement]
        public string Logo { get; set; }
        [XmlElement]
        public string Company { get; set; }
        [XmlElement]
        public string Address { get; set; }
        [XmlElement]
        public string City { get; set; }
        [XmlElement]
        public string State { get; set; }
        [XmlElement]
        public string PostCode { get; set; }
        [XmlElement]
        public string Country { get; set; }
        [XmlElement]
        public string CountryCode { get; set; }

        [XmlElement]
        public string PhoneNumber { get; set; }
        [XmlElement]
        public string BrandName { get; set; }
        [XmlElement]
        public string DomainIP { get; set; }
        

    }
}
