using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Configration
{
    public class Settings
    {
        [XmlElement]
        public bool EnableBundling { get; set; }
        [XmlElement]
        public int ListingPageSize { get; set; }

        [XmlElement]
        public string EncryptionKey { get; set; }
        [XmlElement]
        public string AlphaNumericCharacters { get; set; }
        [XmlElement]
        public float MarginActualWithAlternateNearBy { get; set; }
        [XmlElement]
        public string IncompletBookingKey { get; set; }
        [XmlElement]
        public string IncompletBookingScheduleTime { get; set; }
        [XmlElement]
        public bool IncompletBookingScheduleEnable { get; set; }

        [XmlElement]
        public bool IsCouponEnable { get; set; }

        [XmlElement]
        public bool IsListingPhoneStripEnable { get; set; }

        [XmlElement]
        public bool IsSentItineraryEnable { get; set; }

        [XmlElement]
        public TravelAPI TravelAPI { get; set; }
        [XmlElement]
        public TravelBagAPI TravelBagAPI { get; set; }

        [XmlArray(ElementName = "Countries")]
        [XmlArrayItem("Country")]
        public List<Country> Country { get; set; }

        [XmlArray(ElementName = "USState")]
        [XmlArrayItem("State")]
        public List<State> USState { get; set; }

        [XmlArray(ElementName = "CanadaState")]
        [XmlArrayItem("State")]
        public List<State> CanadaState { get; set; }


        [XmlArray(ElementName = "Affiliates")]
        [XmlArrayItem("Affiliate")]
        public List<Affiliate> Affiliate { get; set; }

        [XmlArray(ElementName = "Portals")]
        [XmlArrayItem("Portal")]
        public List<Portal> Portal { get; set; }

        [XmlArray(ElementName = "SocialMedias")]
        [XmlArrayItem("SocialMedia")]
        public List<SocialMedia> SocialMedia { get; set; }

        [XmlArray(ElementName = "ListingAnimatedBanner")]
        [XmlArrayItem("LstBannerAnimationDisable")]
        public List<int> LstBannerAnimationDisable { get; set; }

        [XmlElement]
        public string HideAffiliateTFN { get; set; }

        [XmlElement]
        public DealAPI DealAPI { get; set; }

        [XmlArray(ElementName = "BasicEconomyAirlines")]
        [XmlArrayItem("Airline")]
        public List<string> BasicEconomyAirlines { get; set; }

        [XmlElement]
        public TravelexInsuranceAPI TravelexInsuranceAPI { get; set; }
        [XmlArray(ElementName = "ICICIErrors")]
        [XmlArrayItem("ICICIError")]
        public List<ICICIError> ICICIError { get; set; }
        [XmlElement]
        public ZendeskTicket ZendeskTicket { get; set; }
        [XmlArray(ElementName = "RestrictedCountries")]
        [XmlArrayItem("Code")]
        public List<string> RestrictedCountries { get; set; }
        [XmlElement]
        public UserProfileAPI UserProfileAPI { get; set; }
        [XmlElement]
        public CouponAPI CouponAPI { get; set; }
        [XmlElement]
        public TravelExtendedCancellation TravelExtendedCancellation { get; set; }
        [XmlElement]
        public TravelInsurance TravelInsurance { get; set; }
        [XmlElement]
        public CurrencySeting CurrencySeting { get; set; }

    }

    public class ListingAnimatedBanner
    {
        [XmlArrayItem("Affiliate")]
        public List<int> Affiliate { get; set; }
    }
    public class Affiliate
    {
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public string Name { get; set; }


        [XmlAttribute]
        public string Signature { get; set; }
    }

    public class Portal
    {
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
    }

    public class Country
    {
        [XmlAttribute]
        public string Code { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
    }

    public class State
    {
        [XmlAttribute]
        public string Code { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
    }

    public class AuthoriseToken
    {
        [XmlAttribute]
        public string Header { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
    }

    public class TravelAPI
    {
        [XmlElement]
        public string ApiPath { get; set; }
        [XmlElement]
        public string SearchAction { get; set; }
        [XmlElement]
        public string CheckAvailabilityAction { get; set; }
        [XmlElement]
        public string BookingAction { get; set; }
        [XmlElement]
        public string RequestHeaderReferrer { get; set; }
        [XmlElement]
        public AuthoriseToken AuthoriseToken { get; set; }
        [XmlElement]
        public int SearchRestClientTimeOut { get; set; }
        [XmlElement]
        public int CheckAvailRestClientTimeOut { get; set; }
        [XmlElement]
        public int BookingRestClientTimeOut { get; set; }

    }
    public class TravelBagAPI
    {
        [XmlElement]
        public bool IsEnable { get; set; }
        [XmlElement]
        public bool IsLiveEnvironment { get; set; }
        [XmlElement]
        public bool IsBillingLatter { get; set; }
        [XmlElement]
        public Environment TestEnvironent { get; set; }
        [XmlElement]
        public Environment ProductionEnvironent { get; set; }
        [XmlElement]
        public Products Products { get; set; }

    }
    public class Environment
    {

        [XmlElement]
        public bool IsEnable { get; set; }

        [XmlElement]
        public string User { get; set; }
        [XmlElement]
        public string Credential { get; set; }
        [XmlElement]
        public string ApiPath { get; set; }
        [XmlElement]
        public string GetProductList { get; set; }
        [XmlElement]
        public string GetAirlineList { get; set; }
        [XmlElement]
        public string PurchaseService { get; set; }
        [XmlElement]
        public string RequestHeaderReferrer { get; set; }
        [XmlElement]
        public AuthoriseToken AuthoriseToken { get; set; }
        [XmlElement]
        public int RestClientTimeOut { get; set; }
    }


    public class Products
    {
        [XmlElement]
        public List<Product> Product { get; set; }
    }
    public class Product
    {
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public string ProductCode { get; set; }
        [XmlAttribute]
        public string ProductName { get; set; }
        [XmlAttribute]
        public float ProductPrice { get; set; }
        [XmlAttribute]
        public string CurrencyCode { get; set; }
        [XmlAttribute]
        public double ProductCoverageAmount { get; set; }
    }
    public class SocialMedia
    {
        [XmlAttribute]
        public int OrderId { get; set; }
        [XmlAttribute]
        public string SocialClass { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Link { get; set; }
    }

    public class DealAPI
    {
        [XmlElement]
        public string ApiPath { get; set; }
        [XmlElement]
        public string RequestHeaderReferrer { get; set; }
        [XmlElement]
        public string DMSAction { get; set; }
        [XmlElement]
        public AuthoriseToken AuthoriseToken { get; set; }
        [XmlElement]
        public int RestClientTimeOut { get; set; }

    }
    public class TravelexInsuranceAPI
    {
        [XmlElement]
        public bool IsEnable { get; set; }
        [XmlElement]
        public float PPaxPrice { get; set; }
        [XmlElement]
        public bool IsLiveEnvironment { get; set; }
        [XmlElement]
        public bool IsBillingLatter { get; set; }
        [XmlElement]
        public int MaxTripCost { get; set; }
        [XmlElement]
        public int MaxTripLength { get; set; }
        [XmlElement]
        public int MinTripCost { get; set; }
        [XmlElement]
        public string RestrictedDestination { get; set; }
        [XmlElement]
        public TravelexInsEnvironment TestEnvironent { get; set; }
        [XmlElement]
        public TravelexInsEnvironment ProductionEnvironent { get; set; }
        public TravelexProducts TravelexProducts { get; set; }

    }
    public class TravelexProducts
    {
        [XmlElement]
        public List<TravelexProduct> TravelexProduct { get; set; }
    }
    public class TravelexProduct
    {
        [XmlAttribute]
        public int Id { get; set; }
        [XmlAttribute]
        public string ProducType { get; set; }
        [XmlAttribute]
        public string ProductFormNumber { get; set; }
        [XmlAttribute]
        public string CoverageTypeCode { get; set; }
    }
    public class TravelexInsEnvironment
    {

        [XmlElement]
        public string Location { get; set; }
        [XmlElement]
        public string User { get; set; }
        [XmlElement]
        public string Credential { get; set; }
        [XmlElement]
        public string ApiPath { get; set; }
        [XmlElement]
        public string GetRate { get; set; }
        [XmlElement]
        public string GetPaymentConfiguration { get; set; }
        [XmlElement]
        public string CreatePolicy { get; set; }
        [XmlElement]
        public string Host { get; set; }
        [XmlElement]
        public string RequestHeaderReferrer { get; set; }
        [XmlElement]
        public AuthoriseToken AuthoriseToken { get; set; }
        [XmlElement]
        public int RestClientTimeOut { get; set; }
    }
    public class ICICIError
    {
        [XmlAttribute]
        public string Code { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
    }
    public class ZendeskTicket
    {
        [XmlElement]
        public string UserID { get; set; }
        [XmlElement]
        public string Password { get; set; }
        [XmlElement]
        public string ZendeskURL { get; set; }
        [XmlElement]
        public string UserCreatURL { get; set; }
        [XmlElement]
        public string TicketURL { get; set; }
        [XmlElement]
        public string subject { get; set; }
        [XmlElement]
        public string BodyMsg { get; set; }
        [XmlElement]
        public bool IsEnableZendeskTicket { get; set; }
    }
    public class UserProfileAPI
    {
        [XmlElement]
        public bool UserProfileIsEnable { get; set; }
        [XmlElement]
        public string UserProfileURL { get; set; }
        [XmlElement]
        public int UserProfilePortal { get; set; }
        [XmlElement]
        public string UserProfileAuthoriseToken { get; set; }
    }
    public class CouponAPI
    {
        [XmlElement]
        public bool IsEnable { get; set; }
    }
    public class TravelExtendedCancellation
    {
        [XmlElement]
        public bool IsEnable { get; set; }
        [XmlElement]
        public float PricePPax { get; set; }
        [XmlElement]
        public float MinBookingAmount { get; set; }
        [XmlElement]
        public float MinAmount { get; set; }
        [XmlElement]
        public float MaxAmount { get; set; }

    }
    public class TravelInsurance
    {
        [XmlElement]
        public bool IsEnable { get; set; }
        [XmlElement]
        public float PricePPax { get; set; }
    }

    public class CurrencySeting
    {
        [XmlElement]
        public bool IsCurrencyEnable { get; set; }
    }

}
