using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Configration
{
    public class PortalSettings
    {
        [XmlElement]
        public string Domain { get; set; }

        [XmlElement]
        public string DomainUrl { get; set; }

        [XmlElement]
        public int PortalId { get; set; }

        [XmlElement]
        public string AirlineLogoLocation { get; set; }

        [XmlElement]
        public bool IsCheckAvailability { get; set; }
        [XmlElement]
        public bool IsPersistentCookies { get; set; }

        [XmlElement]
        public string SelfBookingMail { get; set; }
        
        [XmlElement]
        public EmailServerInfo EmailServerInfo { get; set; }
        [XmlArray(ElementName = "EmailServerTemplate")]
        [XmlArrayItem("Emails")]
        public List<Emails> Emails { get; set; }

        [XmlElement]
        public PortalDetails PortalDetails { get; set; }
        [XmlElement]
        public Operations Operations{ get; set; }
        [XmlArray(ElementName = "WebSiteModes")]
        [XmlArrayItem("WebSiteMode")]
        public List<WebSiteMode> WebSiteMode { get; set; }

        [XmlElement]
        public HomePageDeals HomePageDeals { get; set; }

        [XmlArray(ElementName = "ResponseTagAffiliate")]
        [XmlArrayItem("Affiliate")]
        public List<int> ResponseTagAffiliates { get; set; }

        public string HideTFNFromBanners { get; set; }
        public string SearchEngineAffiliate { get; set; }
        [XmlElement]
        public TFNShowHideSetting TFNShowHideSetting { get; set; }

        [XmlElement("ICICIEasyPay")]
        public ICICIEasyPay EasyPay { get; set; }

        [XmlElement("AffiliateDeeplinkLandingPageProcess")]
        public AffiliateDeeplinkLandingPageProcess DeeplinkLandingPage { get; set; }
        [XmlElement]
        public GeoLocationAPI GeoLocationAPI { get; set; }

        [XmlElement]
        public MetaSearchShow MetaSearchShow { get; set; }
    }
    public class Emails
    {
        [XmlElement]
        public int Portal { get; set; }
        [XmlElement]
        public EmailTemplate BookingReceipt { get; set; }
        public EmailTemplate PaymentReceipt { get; set; }
        public EmailTemplate PaymentConfirmation { get; set; }
        public EmailTemplate DocuSign { get; set; }        
    }
    public class EmailServerInfo
    {
        [XmlElement]
        public string Server { get; set; }
        [XmlElement]
        public string User { get; set; }
        [XmlElement]
        public string Password { get; set; }
        [XmlElement]
        public int Port { get; set; }
        [XmlElement]
        public bool IsEnableSsl { get; set; }
    }
    public class EmailTemplate
    {
        [XmlElement]
        public string EmailUserId { get; set; }
        [XmlElement]
        public string EmailPass { get; set; }
        [XmlElement]
        public string MailRecipient { get; set; }
        [XmlElement]
        public string Subject { get; set; }
        [XmlElement]
        public string Attachment { get; set; }

        [XmlElement]
        public string Body { get; set; }

        [XmlElement]
        public string AdminMailBcC { get; set; }
        [XmlElement]
        public bool IsHtml { get; set; }

    }
    public class Operations
    {
        [XmlElement]
        public string DocuSignLocation { get; set; }
    }


    public class WebSiteMode
    {
        [XmlAttribute]
        public string Guid { get; set; }
        [XmlAttribute]
        public string Modetype { get; set; }
        [XmlAttribute]
        public bool Enabled { get; set; }
        [XmlAttribute]
        public string IP { get; set; }
        public List<string> GetIPs()
        {
            return this.IP.Split(',').ToList<string>();
        }
    }

    public class HomePageDeals
    {
        [XmlElement]
        public bool IsEnable { get; set; }
        [XmlElement]
        public HomeDeal Domestic { get; set; }
        [XmlElement]
        public HomeDeal International { get; set; }
        [XmlElement]
        public HomeDeal Destination { get; set; }
    }
    public class HomeDeal
    {
        [XmlElement]
        public string Guid { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string Airline { get; set; }
        [XmlElement]
        public string From { get; set; }
        [XmlElement]
        public string To { get; set; }
        [XmlElement]
        public int TripType { get; set; }
    }
    public class TFNShowHideSetting
    {
        [XmlElement]
        public bool IsTFNShow { get; set; }
        [XmlElement]
        public string ShowTFNFromCenter { get; set; }
        [XmlElement]
        public string ShowTFNFromBottom { get; set; }
        [XmlElement]
        public bool IsShowShakeStyle { get; set; }
    }

    public class ICICIEasyPay
    {
        [XmlElement]
        public string ASEKEY { get; set; }
        [XmlElement]
        public string MerchantId { get; set; }
        [XmlElement]
        public string SubMerchantId { get; set; }
        [XmlElement]
        public string EasyPayUrl { get; set; }
        [XmlElement]
        public string ReturnUrl { get; set; }
        [XmlElement]
        public string PaymentRSFilePath { get; set; }
    }
    public class AffiliateDeeplinkLandingPageProcess
    {
        [XmlArray(ElementName = "CallOnlyOfferSearch")]
        [XmlArrayItem("Affiliate")]
        public List<int> CallOnlyOfferSearch { get; set; }
        [XmlArray(ElementName = "NoRedirectDevice")]
        [XmlArrayItem("Affiliate")]
        public List<int> NoRedirectDevice { get; set; }
        [XmlArray(ElementName = "NoRedirectDesktop")]
        [XmlArrayItem("Affiliate")]
        public List<int> NoRedirectDesktop { get; set; }
    }

    public class GeoLocationAPI
    {
        [XmlElement]
        public string Domain { get; set; }
        [XmlElement]
        public string Action { get; set; }
        [XmlElement]
        public string APIKey { get; set; }
    }
public class MetaSearchShow
    {
        [XmlElement]
        public string MobileShow { get; set; }
        [XmlElement]
        public string DeaktopShow { get; set; }
    }
}
