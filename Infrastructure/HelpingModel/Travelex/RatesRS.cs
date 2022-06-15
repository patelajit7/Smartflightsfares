using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infrastructure.HelpingModel.Travelex
{
    [XmlRoot(ElementName = "InsuranceBookRS", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
    public class RatesRS
    {
        [XmlElement(ElementName = "PolicyDetail", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public PolicyDetail PolicyDetail { get; set; }
        [XmlElement(ElementName = "PlanCost", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public PlanCost PlanCost { get; set; }
        [XmlElement(ElementName = "Traveler", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public List<Traveler> Traveler { get; set; }
        [XmlElement(ElementName = "Contact", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public Contact Contact { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlElement(ElementName = "Warnings", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public Warnings Warnings { get; set; }
        [XmlElement(ElementName = "Benefit", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public List<Benefit> Benefit { get; set; }
    }

    [XmlRoot(ElementName = "PolicyDetail", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
    public class PolicyDetail
    {
        [XmlElement(ElementName = "RefNumber", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public string RefNumber { get; set; }
        [XmlElement(ElementName = "GroupNumber", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public string GroupNumber { get; set; }
        [XmlAttribute(AttributeName = "OrderDate")]
        public string OrderDate { get; set; }
        [XmlAttribute(AttributeName = "EffectiveDate")]
        public string EffectiveDate { get; set; }
        [XmlAttribute(AttributeName = "ExpireDate")]
        public string ExpireDate { get; set; }
        [XmlElement(ElementName = "PolicyNumber", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public string PolicyNumber { get; set; }
    }

    [XmlRoot(ElementName = "PlanCost", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
    public class PlanCost
    {
        [XmlElement(ElementName = "Amount", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public decimal Amount { get; set; }
        [XmlElement(ElementName = "ProcessingFee", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public decimal ProcessingFee { get; set; }
        [XmlElement(ElementName = "Commission", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public decimal Commission { get; set; }
        [XmlAttribute(AttributeName = "CurrencyCode")]
        public string CurrencyCode { get; set; }
    }

    [XmlRoot(ElementName = "Traveler", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
    public class Traveler
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "TripCost")]
        public string TripCost { get; set; }
        [XmlAttribute(AttributeName = "Rate")]
        public string Rate { get; set; }
        [XmlAttribute(AttributeName = "Commission")]
        public string Commission { get; set; }
    }

    [XmlRoot(ElementName = "Contact", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
    public class Contact
    {
        [XmlElement(ElementName = "Telephone", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public string Telephone { get; set; }
        [XmlAttribute(AttributeName = "ContactType")]
        public string ContactType { get; set; }
    }
    [XmlRoot(ElementName = "Warnings", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
    public class Warnings
    {
        [XmlElement(ElementName = "Warning", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
        public string Warning { get; set; }
    }
    [XmlRoot(ElementName = "Benefit", Namespace = "http://www.travelex-insurance/2003/01/InsuranceBookRS.xsd")]
    public class Benefit
    {
        [XmlAttribute(AttributeName = "Description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "Coverage")]
        public string Coverage { get; set; }
    }
}

