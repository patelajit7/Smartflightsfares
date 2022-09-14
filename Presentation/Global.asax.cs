using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Business;
using Infrastructure.HelpingModel;
using Infrastructure;
using Infrastructure.HelpingModel.API;

namespace Presentation
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Utility.LoadApplicationConfigartion(HttpContext.Current);
            BundleTable.EnableOptimizations = Utility.Settings.EnableBundling;
            if (Utility.Settings.IncompletBookingScheduleEnable)
            {
                SchedulerBusiness.Start();
            }

            //BookingDetail detail = Newtonsoft.Json.JsonConvert.DeserializeObject<BookingDetail>("{\"Transaction\":{\"Id\":0,\"Guid\":null,\"PNR\":\"INPROGRESS\",\"ReferenceNumber\":null,\"GDS\":0,\"ProviderId\":0,\"PortalId\":0,\"BookingType\":0,\"BookingStatus\":0,\"BookingSubStatus\":0,\"BookingSourceType\":0,\"AgentId\":0,\"AgentLead\":0,\"UserId\":0,\"BookedOn\":\"2022-09-05T10:50:00.0104824Z\"},\"FlightSearch\":{\"SearchGuidId\":\"01ed276f7a744594804c57e9c2306a9e\",\"PortalId\":8000,\"AffiliateId\":8000,\"TripType\":1,\"Origin\":\"PNS\",\"Destination\":\"PHX\",\"Departure\":\"2022-09-05T00:00:00+05:30\",\"Return\":null,\"Adult\":1,\"Senior\":0,\"Child\":0,\"InfantOnSeat\":0,\"InfantOnLap\":0,\"Cabin\":1,\"PreferredCarrier\":null,\"IsDirectFlight\":false,\"IP\":\"172.56.81.224\",\"IsMobileDevice\":false,\"UserId\":0,\"UserAgent\":null,\"Providers\":null,\"ResultFound\":128,\"OriginSearch\":\"PNS - Pensacola International Arpt, Pensacola, United States\",\"DestinationSearch\":\"PHX - Sky Harbor Intl Arpt, Phoenix, United States\",\"OriginAirportName\":\"Pensacola International Arpt\",\"OriginCountry\":\"United States\",\"DestAirportName\":\"Sky Harbor Intl Arpt\",\"DestCountry\":\"United States\",\"IsMetaSearch\":true,\"Created\":\"2022-09-05T09:23:26.761Z\",\"UtmSource\":null,\"UtmMedium\":null,\"UtmCampaign\":null,\"UtmTerm\":null,\"UtmContent\":null,\"UtmKeyword\":null,\"ClickedId\":null,\"PageType\":0,\"PageId\":0,\"SearchDateTime\":\"0001-01-01T00:00:00\",\"FlexiblityQualifier\":null},\"Contract\":{\"SearchGuid\":\"01ed276f7a744594804c57e9c2306a9e\",\"ContractId\":23,\"Provider\":2,\"GDSType\":0,\"Origin\":\"PNS\",\"OriginCityName\":\"Pensacola\",\"Destination\":\"PHX\",\"DestinationCityName\":\"Phoenix\",\"OriginSearch\":\"PNS\",\"DestinationSearch\":\"PHX\",\"DepartureDate\":\"2022-09-05T18:45:00+05:30\",\"ArrivalDate\":\"2022-09-05T00:00:00+05:30\",\"TripDetails\":{\"OutBoundSegment\":[{\"Id\":0,\"IsReturn\":false,\"IsDepartDateHighlight\":false,\"IsOriginHighlight\":false,\"IsDestinationHighlight\":false,\"Departure\":\"2022-09-05T00:00:00+05:30\",\"Arrival\":\"2022-09-05T00:00:00+05:30\",\"MarketingCarrier\":{\"Code\":\"WN\",\"Name\":\"Southwest Airlines\",\"IsLowcost\":false,\"MinPrice\":0.0,\"IsMultiAirline\":false},\"OperatingCarrier\":{\"Code\":\"WN\",\"Name\":\"Southwest Airlines\",\"IsLowcost\":false,\"MinPrice\":0.0,\"IsMultiAirline\":false},\"DepartureTime\":\"18:45:00\",\"ArrivalTime\":\"20:10:00\",\"StopOverTime\":null,\"OutTerminal\":\"-\",\"InTerminal\":\"-\",\"EquipmentType\":\"Boeing 73W\",\"FlightNumber\":\"2501\",\"CnxType\":null,\"FareBasis\":\"RLA0P2H\",\"Class\":\"R\",\"PrevClass\":null,\"Cabin\":\"E\",\"CabinType\":1,\"Origin\":\"PNS\",\"OriginCity\":\"Pensacola\",\"Destination\":\"BNA\",\"DestinationCity\":\"Nashville\",\"FlightDuration\":\"01:25:00\",\"CompanyFranchiseDetails\":null,\"AvailableSeats\":1,\"NoOfStops\":0,\"IsSoldOut\":false,\"AirlineLocator\":null,\"SegmentStatus\":null,\"SegmentTripProExt\":{\"BaggageAllowance\":\"2P\",\"BaggageInfoUrl\":\"https://www.southwest.com/html/customer-service/baggage/index-pol.html\"},\"SegmentASSExtension\":null},{\"Id\":0,\"IsReturn\":false,\"IsDepartDateHighlight\":false,\"IsOriginHighlight\":false,\"IsDestinationHighlight\":false,\"Departure\":\"2022-09-05T00:00:00+05:30\",\"Arrival\":\"2022-09-05T00:00:00+05:30\",\"MarketingCarrier\":{\"Code\":\"WN\",\"Name\":\"Southwest Airlines\",\"IsLowcost\":false,\"MinPrice\":0.0,\"IsMultiAirline\":false},\"OperatingCarrier\":{\"Code\":\"WN\",\"Name\":\"Southwest Airlines\",\"IsLowcost\":false,\"MinPrice\":0.0,\"IsMultiAirline\":false},\"DepartureTime\":\"21:50:00\",\"ArrivalTime\":\"23:25:00\",\"StopOverTime\":null,\"OutTerminal\":\"-\",\"InTerminal\":\"-\",\"EquipmentType\":\"Boeing 7M8\",\"FlightNumber\":\"2851\",\"CnxType\":null,\"FareBasis\":\"RLA0P2H\",\"Class\":\"R\",\"PrevClass\":null,\"Cabin\":\"E\",\"CabinType\":1,\"Origin\":\"BNA\",\"OriginCity\":\"Nashville\",\"Destination\":\"PHX\",\"DestinationCity\":\"Phoenix\",\"FlightDuration\":\"03:35:00\",\"CompanyFranchiseDetails\":null,\"AvailableSeats\":1,\"NoOfStops\":0,\"IsSoldOut\":false,\"AirlineLocator\":null,\"SegmentStatus\":null,\"SegmentTripProExt\":{\"BaggageAllowance\":\"2P\",\"BaggageInfoUrl\":\"https://www.southwest.com/html/customer-service/baggage/index-pol.html\"},\"SegmentASSExtension\":null}],\"InBoundSegment\":null},\"ValidatingCarrier\":{\"Code\":\"WN\",\"Name\":\"Southwest Airlines Texas\",\"IsLowcost\":false,\"MinPrice\":0.0,\"IsMultiAirline\":false},\"FareType\":\"PUBLISHED\",\"TripType\":1,\"IsRefundable\":false,\"Adult\":1,\"Senior\":0,\"Child\":0,\"InfantOnSeat\":0,\"InfantOnLap\":0,\"FareBasisCode\":\"RLA0P2H\",\"AdultFare\":{\"PaxCount\":1,\"PaxType\":1,\"GDSPaxType\":null,\"ActualBaseFare\":278.84,\"BaseFare\":278.84,\"Tax\":44.14,\"TotalFare\":322.97998,\"Markup\":6.45959949,\"SupplierFee\":0.0,\"Discount\":0.0,\"IsSellInsurance\":false,\"InsuranceAmount\":0.0,\"IsSellBaggageInsurance\":false,\"BaggageInsuranceAmount\":0.0,\"FareBaseCode\":\"RLA0P2H\",\"CurrencyType\":1,\"IsExtendedCancellation\":false,\"ExtendedCancellationAmount\":0.0,\"BookingFee\":0.0,\"TotalFareV2\":329.439575,\"TotalFarePPax\":329.439575,\"BaseFarePPax\":278.84,\"TotalBaseFare\":278.84,\"TaxPPax\":50.5995979,\"TotalTax\":50.5995979},\"ChildFare\":null,\"InfantOnSeatFare\":null,\"InfantOnLapFare\":null,\"SeniorFare\":null,\"TotalMarkup\":6.45959949,\"TotalSupplierFee\":0.0,\"TotalBaseFare\":278.84,\"TotalTax\":44.14,\"TotalGDSFareV2\":329.439575,\"EnginePriority\":1,\"Contractkey\":\"051845WN2501052150WN2851\",\"DatesKey\":null,\"PricingSource\":\"PUB\",\"MaxStopOutbound\":1,\"MaxStopInbound\":0,\"IsMultipleAirlineContract\":false,\"MinSeatAvailableForContract\":1,\"IsPhoneOnly\":false,\"ContractType\":1,\"TotalOutBoundFlightDuration\":\"06:40:00\",\"TotalInBoundFlightDuration\":\"00:00:00\",\"AffiliateId\":8000,\"AmadeusSessionToken\":null,\"BookingStatus\":0,\"TotalBookingFee\":0.0,\"FareDifference\":0.0,\"BaggageQuantity\":0,\"IsGhostBooking\":false,\"BaggageDetails\":{\"OutboundBaggage\":{\"PesonalItem\":{\"IsAllowed\":true,\"Description\":\"<li>Purse, small backpack, briefcase</li>\",\"Quantity\":1},\"CarryOn\":null,\"Checkin\":{\"IsAllowed\":true,\"Description\":\"<li>UPTO50LB 23KG AND62LI 158LCM</li> <li>UPTO50LB 23KG AND62LI 158LCM</li>\",\"Quantity\":2}},\"InboundBaggage\":null},\"MetaFixedPrice\":null,\"TripProExt\":{\"ItineraryId\":\"3b5aa3b65e94479ba21c60c4cbd4f901\"},\"MystiflyExt\":null,\"AmaduesSelfServiceExtension\":null},\"BillingDetails\":{\"CCHolderName\":null,\"CardNumber\":null,\"CVVNumber\":null,\"ExpiryYear\":0,\"ExpiryMonth\":0,\"CardType\":0,\"Email\":\"eyesofthemaker@hotmail.com\",\"EmailConfirm\":null,\"Country\":\"US\",\"State\":\"AK\",\"StateName\":null,\"ZipCode\":null,\"AddressLine1\":null,\"AddressLine2\":null,\"AddressLine3\":null,\"City\":null,\"BillingPhone\":\"6302002510\",\"ContactPhone\":null,\"IsPrimaryCard\":false,\"AreaCode\":null,\"CountryCode\":null},\"Travellers\":[{\"PaxOrderId\":1,\"PaxType\":1,\"Title\":0,\"FirstName\":\"Tiffany\",\"MiddleName\":null,\"LastName\":\"Love \",\"Gender\":2,\"DOBDay\":null,\"DOBMonth\":0,\"DOBYear\":null,\"PassportNumber\":null,\"PassportIssuingCountry\":null,\"PassportExpiryDate\":null}],\"PriceIncrease\":0.0,\"BagInsuranc\":null,\"TravelerInsurance\":null,\"CouponDetails\":null,\"ExtendedCancellation\":null,\"Currency\":0,\"CurrencyConversion\":0.0,\"CurrencyCode\":null}");

            //IncompletBookingBusiness.SaveBookingDetails(detail);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string absolutePath = HttpContext.Current.Request.Url.AbsolutePath.Trim();
            string lowercaseURL = (Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + absolutePath);
            if (Regex.IsMatch(lowercaseURL, @"[A-Z]"))
            {
                PermanentRedirect(lowercaseURL.ToLower() + HttpContext.Current.Request.Url.Query);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Exception lastError = Server.GetLastError();
                if(lastError!=null)
                {
                    HttpContext context = HttpContext.Current;
                    string error = lastError.ToString();
                    string referrer = context.Request.UrlReferrer != null ? HttpContext.Current.Request.UrlReferrer.ToString() : "unknown";
                    if (!string.IsNullOrEmpty(referrer) && !referrer.Equals("unknown", StringComparison.OrdinalIgnoreCase))
                    {
                        Utility.Logger.Error(string.Format("APPLICATION ERROR LAST APP ERROR|Message:{0}|Referrer:{1}", error, referrer));
                    }
                    else
                    {
                        Utility.Logger.Error("APPLICATION_ERROR LAST ERROR:" + error);
                    }

                    if (HttpContext.Current.IsCustomErrorEnabled)
                        ShowCustomErrorPage(lastError);
                }
               
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(ex.ToString());
                if (!string.IsNullOrEmpty(ex.Message))
                {
                    Utility.Logger.Error(ex.Message);
                }
                if (ex.InnerException != null)
                {
                    Utility.Logger.Error(ex.InnerException.ToString());
                }
            }


        }
        private void ShowCustomErrorPage(Exception exception)
        {
            try
            {
                bool isLogError = false;
                HttpException httpException = exception as HttpException;
                if (httpException == null)
                {
                    httpException = new HttpException(500, "Internal Server Error", exception);
                }
                if (httpException.Message != null)
                {
                    isLogError = true;
                    Utility.Logger.Error("MESSAGE:" + httpException.Message);
                }
                if (!isLogError && httpException.InnerException != null)
                {
                    Utility.Logger.Error("INNER EXCEPTION:" + httpException.InnerException);
                }
                Response.Clear();
                switch (httpException.GetHttpCode())
                {
                    case 403:
                        Server.ClearError();
                        Response.Redirect("/access-denied");
                        break;
                    case 404:
                        Server.ClearError();
                        Response.Redirect("/404-page-not-found");
                        break;
                    case 500:
                        Server.ClearError();
                        Response.Redirect("/500-internal-server-error");
                        break;
                    default:
                        Server.ClearError();
                        Response.Redirect(string.Format("error/{0}", httpException.GetHttpCode()));
                        break;
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("ShowCustomeErrorPage|" + ex.ToString());
            }
        }
        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("X-Powered-By");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Remove("Server");
        }
        private void PermanentRedirect(string url)
        {
            Response.Clear();
            Response.Status = "301 Moved Permanently";
            Response.AddHeader("Location", url);
            Response.End();
        }
        private void TemporarilyRedirect(string url)
        {
            Response.Clear();
            Response.Status = "302 Moved Temporarily";
            Response.AddHeader("Location", url);
            Response.End();
        }
        void Application_End(object sender, EventArgs e)
        {
            try
            {
                BookingInformation.SaveFlightSearches(true);
                IncompletBookingBusiness.SaveIncompletBookingsInDB();
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Global.Application_End" + ex.ToString());
            }
        }
    }
}
