using Configration;
using Database;
using Infrastructure;
using Infrastructure.Entities;
using Infrastructure.HelpingModel;
using Infrastructure.HelpingModel.API;
using Infrastructure.Interfaces;
using Logger;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Data;
using System.Data.SqlClient;
using Infrastructure.MongoDB;
using BBRibbon = Infrastructure.HelpingModel.BlueribbonbagsAPI;
using Newtonsoft.Json;
using Infrastructure.HelpingModel.BookingEntities;
using Infrastructure.HelpingModel.Travelex;

namespace Common
{
    public class Utility
    {
        public static Settings Settings { get; set; }
        public static PortalSettings PortalSettings { get; set; }
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        public static ILoggingService Logger = (ILoggingService)LoggingService.GetLoggingService();
        public static IDatabase DatabaseService = (IDatabase)new DatabaseService();
        public static IMongoDBRepository MongoInstance = new MongoDBRepository();
        private static object syncPoolRoot = new Object();
        private static bool IsApplicationLoaded { get; set; }
        public static int PortalId = Convert.ToInt32(ConfigurationManager.AppSettings["PortalId"].ToString());
        public static List<Airports> Airports { get; set; }
        public static List<string> MultiAirportCityCode { get; set; }
        public static List<Airlines> Airlines { get; set; }
        public static PortalStaticData PortalData { get; set; }
        public static MemoryCache GLobalCache = MemoryCache.Default;
        public static string ConnString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
        public static List<FlightSearch> FlightSearches = new List<FlightSearch>();
        public static List<BBRibbon.Airlines> BaggageAirlines { get; set; }
        public static List<TravelexAirlines> TravelexAirlines { get; set; }
        public static List<CouponData> CouponData { get; set; }
        public static GoogleReviews Reviews { get; set; }

        public static HomeDeals HomeDeals { get; set; }
        /// <summary>
        /// Load application configration
        /// </summary>
        /// <param name="httpContext">HttpContext>
        public static void LoadApplicationConfigartion(HttpContext httpContext)
        {
            try
            {
                Logger.Info("APPLICATION START LOADING");
                if (!IsApplicationLoaded)
                {

                    lock (syncPoolRoot)
                    {
                        if (!IsApplicationLoaded)
                        {
                            if (IsLoadConfig())
                            {
                                List<Task> lstTasks = new List<Task>
                        {
                            Task.Factory.StartNew(()=>Utility.Airports=Utility.DatabaseService.List<Airports>()),
                            Task.Factory.StartNew(()=>Utility.Airlines=Utility.DatabaseService.List<Airlines>()),
                            Task.Factory.StartNew(()=>Utility.PortalData=GetPortalData(Utility.PortalId, Utility.ConnString)),
                            Task.Factory.StartNew(()=>Utility.CouponData=BookingProcedures.GetCouponDataDetails(Utility.PortalId,Utility.ConnString)),
                            Task.Factory.StartNew(()=>Utility.Reviews=BookingProcedures.GetReviews(Utility.ConnString))
                        };
                                Task.WaitAll(lstTasks.ToArray());
                                IsApplicationLoaded = true;
                                Task.Factory.StartNew(() => SetMultiAirportCityCode());
                            }

                        }
                    }
                }

                Logger.Info("APPLICATION END LOADING");
            }
            catch (Exception ex)
            {
                Logger.Error("APPLICATION START ISSUE:EXCEPTION|" + ex.ToString());
            }
        }
        public static string GetBRBFlightDetails(TripDetails tripDetails)
        {
            string response = string.Empty;
            try
            {
                StringBuilder str = new StringBuilder();
                foreach (Segments item in tripDetails.OutBoundSegment)
                {
                    if (string.IsNullOrEmpty(str.ToString()))
                    {
                        str.Append(string.Format("{0}{1}", item.MarketingCarrier.Code, item.FlightNumber));
                    }
                    else
                    {
                        str.Append(string.Format("-{0}{1}", item.MarketingCarrier.Code, item.FlightNumber));
                    }
                }
                if (tripDetails.InBoundSegment != null && tripDetails.InBoundSegment.Count > 0)
                {
                    foreach (Segments item in tripDetails.InBoundSegment)
                    {

                        str.Append(string.Format("-{0}{1}", item.MarketingCarrier.Code, item.FlightNumber));
                    }
                }

                response = str.ToString().ToUpper();
            }
            catch (Exception ex)
            {
                Logger.Error("APPLICATION START ISSUE:EXCEPTION|" + ex.ToString());
            }
            return response;
        }

        public static bool IsAirlineExistForBaggage(string _code)
        {
            bool isExist = false;
            try
            {
                BBRibbon.Airlines airlines = Utility.BaggageAirlines.Where<BBRibbon.Airlines>(o => !string.IsNullOrEmpty(o.AirlineIATACode) && o.AirlineIATACode.Equals(_code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<BBRibbon.Airlines>();
                if (airlines != null)
                {
                    isExist = true;
                }

            }
            catch (Exception ex)
            {
                Logger.Error("IsAirlineExistForBaggage:EXCEPTION|" + ex.ToString());
            }
            return isExist;
        }
        private static List<BBRibbon.Airlines> LoadBaggageAirlines()
        {
            List<BBRibbon.Airlines> airlines = null;
            try
            {
                if (Utility.Settings.TravelBagAPI.IsEnable)
                {
                    airlines = ReadDataFile<BBRibbon.Airlines>("bluebagribbon");
                }

            }
            catch (Exception ex)
            {

                Logger.Error("LoadBaggageAirlines:EXCEPTION|" + ex.ToString());
            }
            return airlines;
        }
        private static List<TravelexAirlines> LoadTravelexAirlines()
        {
            List<TravelexAirlines> airlines = null;
            try
            {
                if (Utility.Settings.TravelexInsuranceAPI.IsEnable)
                {
                    airlines = ReadDataFile<TravelexAirlines>("travelexAirlines");
                }

            }
            catch (Exception ex)
            {

                Logger.Error("LoadTravelexAirlines:EXCEPTION|" + ex.ToString());
            }
            return airlines;
        }
        public static void SetOriginDestinationDetails(FlightSearch search)
        {
            try
            {

                string ori = search.Origin;
                string dest = search.Destination;
                string origin = string.Empty, destination = string.Empty;
                string originName = string.Empty, destinationName = string.Empty;
                Airports tempOriAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(ori, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                if (tempOriAirport != null)
                {
                    origin = tempOriAirport.AirportCode;
                    originName = tempOriAirport.AirportName;
                }
                else
                {
                    tempOriAirport = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(ori, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                    if (tempOriAirport != null)
                    {
                        origin = tempOriAirport.CityCode;
                        originName = tempOriAirport.City;
                    }
                }

                Airports tempDestAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(dest, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                if (tempDestAirport != null)
                {
                    destination = tempDestAirport.AirportCode;
                    destinationName = tempDestAirport.AirportName;
                }
                else
                {
                    tempDestAirport = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(dest, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                    if (tempOriAirport != null)
                    {
                        destination = tempDestAirport.CityCode;
                        destinationName = tempDestAirport.City;
                    }
                }
                if (tempOriAirport != null && tempDestAirport != null)
                {

                    search.OriginAirportName = originName;
                    search.OriginSearch = string.Format("{0} - {1}, {2}", origin, tempOriAirport.City, tempOriAirport.CountryName);
                    search.Destination = destination;
                    search.DestAirportName = destinationName;
                    search.DestinationSearch = string.Format("{0} - {1}, {2}", destination, tempDestAirport.City, tempDestAirport.CountryName);
                }
            }
            catch (Exception ex)
            {

                Logger.Error("SetOriginDestinationDetails Track:EXCEPTION|" + ex.ToString() + "| Search|" + JsonConvert.SerializeObject(search));
            }
        }

        private static void SetMultiAirportCityCode()
        {
            try
            {
                if (Utility.Airports != null)
                {
                    var multiAirportCity =
                 from airport in Utility.Airports
                 group airport by airport.CityCode into newGroup
                 where newGroup.Count() > 1
                 select newGroup;
                    if (multiAirportCity != null)
                    {
                        Utility.MultiAirportCityCode = new List<string>();
                        foreach (var item in multiAirportCity)
                        {
                            Utility.MultiAirportCityCode.Add(item.Key.ToString().ToUpper());
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Logger.Error("SetMultiAirportCityCode:EXCEPTION|" + ex.ToString());
            }
        }

        public static int IsValidSignature(string signature)
        {
            int affiliateId = 0;
            try
            {
                Affiliate affiliate = Utility.Settings.Affiliate.Where<Affiliate>(o => o.Signature.Equals(signature, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Affiliate>();
                if (affiliate != null)
                {
                    affiliateId = affiliate.Id;
                }
            }
            catch (Exception ex)
            {

                Logger.Error("IsValidSignature:EXCEPTION|" + ex.ToString());
            }
            return affiliateId;
        }

        public static FlightSearch GetSearch(NameValueCollection queryString)
        {
            FlightSearch search = null;
            try
            {
                if (queryString != null && queryString["origin"] != null && queryString["destination"] != null)
                {
                    Airports tempOriAirport, tempDestAirport = null;
                    string ori = queryString["origin"].ToString().Trim();
                    string dest = queryString["destination"].ToString().Trim();
                    if (ori.Length == 3)
                    {
                        tempOriAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(ori, StringComparison.OrdinalIgnoreCase) || o.CityCode.Equals(ori, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                    }
                    else
                    {
                        tempOriAirport = Utility.Airports.Where<Airports>(o => o.AirportName.Equals(ori, StringComparison.OrdinalIgnoreCase) || o.City.Equals(ori, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                    }
                    if (tempOriAirport == null)
                    {
                        tempOriAirport = Utility.Airports.Where<Airports>(o => o.AirportName.StartsWith(ori, StringComparison.OrdinalIgnoreCase) || o.City.StartsWith(ori, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                    }

                    if (dest.Length == 3)
                    {
                        tempDestAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(dest, StringComparison.OrdinalIgnoreCase) || o.CityCode.Equals(dest, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                    }
                    else
                    {
                        tempDestAirport = Utility.Airports.Where<Airports>(o => o.AirportName.Equals(dest, StringComparison.OrdinalIgnoreCase) || o.City.Equals(dest, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                    }
                    if (tempDestAirport == null)
                    {
                        tempDestAirport = Utility.Airports.Where<Airports>(o => o.AirportName.StartsWith(dest, StringComparison.OrdinalIgnoreCase) || o.City.StartsWith(dest, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                    }


                    if (tempOriAirport != null && tempDestAirport != null)
                    {
                        search = new FlightSearch() { Origin = tempOriAirport.CityCode, OriginSearch = string.Format("{0} - {1}", tempOriAirport.CityCode, tempOriAirport.City), Destination = tempDestAirport.CityCode, DestinationSearch = string.Format("{0} - {1}", tempDestAirport.CityCode, tempDestAirport.City) };
                        int pax = 0;
                        if (queryString["adult"] != null && int.TryParse(queryString["adult"].ToString(), out pax) && pax > 0)
                        {
                            search.Adult = pax;
                        }
                        else
                        {
                            search.Adult = 1;
                        }
                        pax = 0;
                        if (queryString["child"] != null && int.TryParse(queryString["child"].ToString(), out pax) && pax > 0)
                        {
                            search.Child = pax;
                        }
                        pax = 0;
                        if (queryString["infant"] != null && int.TryParse(queryString["infant"].ToString(), out pax) && pax > 0)
                        {
                            search.InfantOnLap = pax;
                        }
                        if (queryString["triptype"] != null)
                        {
                            if (queryString["triptype"].ToString().Trim().Equals("return", StringComparison.OrdinalIgnoreCase))
                            {
                                search.TripType = TripType.ROUNDTRIP;
                            }
                            else
                            {
                                search.TripType = TripType.ONEWAY;
                            }
                        }
                        else
                        {
                            search.TripType = TripType.ONEWAY;
                        }

                        DateTime tempdate;
                        if (queryString["departure"] != null && DateTime.TryParseExact(queryString["departure"].ToString().Trim(), "d-M-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempdate))
                        {
                            search.Departure = tempdate;
                            if (search.TripType == TripType.ROUNDTRIP)
                            {
                                if (DateTime.TryParseExact(queryString["return"].ToString().Trim(), "d-M-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempdate))
                                {
                                    search.Return = tempdate;
                                }
                                else
                                {
                                    search.Return = search.Departure.AddDays(7);
                                }
                            }
                        }
                        else
                        {
                            search.Departure = DateTime.Now.AddDays(2).Date;
                            if (search.TripType == TripType.ROUNDTRIP)
                            {
                                search.Return = search.Departure.AddDays(7);
                            }
                        }

                        if (queryString["cabin"] != null)
                        {
                            string cabin = queryString["cabin"].ToString().ToLower();
                            switch (cabin)
                            {
                                case "economy":
                                    search.Cabin = CabinType.EconomyCoach;
                                    break;
                                case "business":
                                    search.Cabin = CabinType.Business;
                                    break;
                                case "first":
                                    search.Cabin = CabinType.First;
                                    break;
                                default:
                                    search.Cabin = CabinType.EconomyCoach;
                                    break;
                            }
                        }
                        else
                        {
                            search.Cabin = CabinType.EconomyCoach;
                        }

                        if (queryString["airline"] != null)
                        {
                            string air = queryString["airline"].ToString().Trim().ToLower();
                            Airlines airline = null;
                            if (air.Length == 2)
                            {
                                airline = Utility.Airlines.Where<Airlines>(o => o.Code.Equals(air, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airlines>();
                            }
                            else
                            {
                                airline = Utility.Airlines.Where<Airlines>(o => o.Name.Equals(air, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airlines>();
                                if (airline == null)
                                {
                                    airline = Utility.Airlines.Where<Airlines>(o => o.Name.StartsWith(air, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airlines>();
                                }
                            }
                            if (airline != null)
                            {
                                search.PreferredCarrier = airline.Code;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("QUERY STRING:GetSearch" + queryString != null ? queryString.ToString() : "");
                Logger.Error("GetSearch:EXCEPTION|" + ex.ToString());
            }
            return search;
        }
        public static FlightSearch GetAffiliateSearch(NameValueCollection queryString, HttpRequestBase _request)
        {
            FlightSearch search = null;
            try
            {
                int affiliate = 0;
                string utmSource = string.Empty;
                int utmPageType = 0;
                if (queryString["utm_page"] != null)
                {
                    int.TryParse(queryString["utm_page"].ToString(), out utmPageType);
                }
                if (queryString["utm_source"] != null)
                {
                    utmSource = queryString["utm_source"].Trim().ToLower();
                    if (utmSource.Contains(","))
                    {
                        utmSource = utmSource.Split(',')[0];
                    }
                    int.TryParse(utmSource, out affiliate);
                }
                else
                {
                    if (_request != null && _request.UrlReferrer != null)
                        Utility.Logger.Debug("AFF NOT FOUND DEEPLINK:" + _request.UrlReferrer.ToString());
                }

                if (queryString != null && queryString["origin"] != null && queryString["origin"].Length == 3 && queryString["destination"] != null && queryString["destination"].Length == 3)
                {
                    Airports tempOriAirport, tempDestAirport = null;
                    string ori = queryString["origin"].ToString().Trim();
                    string dest = queryString["destination"].ToString().Trim();
                    string origin = string.Empty, destination = string.Empty;
                    string originName = string.Empty, destinationName = string.Empty;
                    tempOriAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(ori, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                    if (tempOriAirport != null)
                    {
                        origin = tempOriAirport.AirportCode;
                        originName = tempOriAirport.AirportName;
                    }
                    else
                    {
                        tempOriAirport = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(ori, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                        if (tempOriAirport != null)
                        {
                            origin = tempOriAirport.CityCode;
                            originName = tempOriAirport.City;
                        }
                        else
                        {
                            Utility.Logger.Info("CODE NOT FOUND:" + ori);
                        }
                    }

                    tempDestAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(dest, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                    if (tempDestAirport != null)
                    {
                        destination = tempDestAirport.AirportCode;
                        destinationName = tempDestAirport.AirportName;
                    }
                    else
                    {
                        tempDestAirport = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(dest, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                        if (tempDestAirport != null)
                        {
                            destination = tempDestAirport.CityCode;
                            destinationName = tempDestAirport.City;
                        }
                        else
                        {
                            Utility.Logger.Info("CODE NOT FOUND:" + dest);
                        }
                    }

                    if (tempOriAirport != null && tempDestAirport != null)
                    {
                        search = new FlightSearch()
                        {
                            Origin = origin,
                            OriginAirportName = originName,
                            OriginSearch = string.Format("{0} - {1}, {2}", origin, tempOriAirport.City, tempOriAirport.CountryName),
                            Destination = destination,
                            DestAirportName = destinationName,
                            DestinationSearch = string.Format("{0} - {1}, {2}", destination, tempDestAirport.City, tempDestAirport.CountryName)
                        };
                        int pax = 0;
                        if (queryString["adults"] != null && int.TryParse(queryString["adults"].ToString(), out pax) && pax > 0)
                        {
                            search.Adult = pax;
                        }

                        pax = 0;
                        if (queryString["children"] != null && int.TryParse(queryString["children"].ToString(), out pax) && pax > 0)
                        {
                            search.Child = pax;
                        }
                        pax = 0;
                        if (queryString["infants"] != null && int.TryParse(queryString["infants"].ToString(), out pax) && pax > 0)
                        {
                            search.InfantOnSeat = pax;
                        }

                        if (queryString["type"] != null)
                        {
                            if (queryString["type"].ToString().Trim().Equals("2") || queryString["type"].ToString().Trim().Equals("return") || queryString["type"].ToString().Trim().Equals("RT", StringComparison.OrdinalIgnoreCase) || queryString["type"].ToString().Trim().Equals("roundtrip", StringComparison.OrdinalIgnoreCase) || queryString["type"].ToString().Trim().Equals("round trip", StringComparison.OrdinalIgnoreCase))
                            {
                                search.TripType = TripType.ROUNDTRIP;
                            }
                            else
                            {
                                search.TripType = TripType.ONEWAY;
                            }
                        }
                        else
                        {
                            search.TripType = TripType.ONEWAY;
                        }

                        DateTime tempdate;
                        if (queryString["departure"] != null)
                        {
                            if (affiliate == 1026 || affiliate == 1027 || (!string.IsNullOrEmpty(utmSource) && utmSource.Equals("skyscanner", StringComparison.OrdinalIgnoreCase)))
                            {
                                if (DateTime.TryParseExact(queryString["departure"].ToString().Trim(), "yyyy-M-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempdate))
                                {
                                    search.Departure = tempdate;
                                    if (search.TripType == TripType.ROUNDTRIP)
                                    {
                                        if (DateTime.TryParseExact(queryString["return"].ToString().Trim(), "yyyy-M-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempdate))
                                        {
                                            search.Return = tempdate;
                                        }
                                        else
                                        {
                                            search.Return = search.Departure.AddDays(7);
                                        }
                                    }
                                }
                                else
                                {
                                    search.Departure = DateTime.Now.AddDays(2).Date;
                                    if (search.TripType == TripType.ROUNDTRIP)
                                    {
                                        search.Return = search.Departure.AddDays(7);
                                    }
                                }
                            }
                            else
                            {
                                if (DateTime.TryParseExact(queryString["departure"].ToString().Trim(), "M-d-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempdate))
                                {
                                    search.Departure = tempdate;
                                    if (search.TripType == TripType.ROUNDTRIP)
                                    {
                                        if (DateTime.TryParseExact(queryString["return"].ToString().Trim(), "M-d-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempdate))
                                        {
                                            search.Return = tempdate;
                                        }
                                        else
                                        {
                                            search.Return = search.Departure.AddDays(7);
                                        }
                                    }
                                }
                                else
                                {
                                    search.Departure = DateTime.Now.AddDays(2).Date;
                                    if (search.TripType == TripType.ROUNDTRIP)
                                    {
                                        search.Return = search.Departure.AddDays(7);
                                    }
                                }
                            }
                        }
                        else
                        {
                            search.Departure = DateTime.Now.AddDays(2).Date;
                            if (search.TripType == TripType.ROUNDTRIP)
                            {
                                search.Return = search.Departure.AddDays(7);
                            }
                        }


                        if (queryString["cabin"] != null)
                        {
                            string cabin = queryString["cabin"].ToString().ToLower();
                            switch (cabin)
                            {
                                case "economy":
                                    search.Cabin = CabinType.EconomyCoach;
                                    break;
                                case "business":
                                    search.Cabin = CabinType.Business;
                                    break;
                                case "first":
                                    search.Cabin = CabinType.First;
                                    break;
                                default:
                                    search.Cabin = CabinType.EconomyCoach;
                                    break;
                            }
                        }
                        else
                        {
                            search.Cabin = CabinType.EconomyCoach;
                        }

                        if (queryString["airline"] != null)
                        {
                            string air = queryString["airline"].ToString().Trim().ToLower();
                            Airlines airline = null;
                            if (air.Length == 2)
                            {
                                airline = Utility.Airlines.Where<Airlines>(o => o.Code.Equals(air, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airlines>();
                                if (airline != null)
                                {
                                    search.PreferredCarrier = airline.Code;
                                }
                            }
                        }

                        if (queryString["utm_source"] != null)
                        {

                            search.AffiliateId = affiliate;
                            search.PortalId = affiliate;
                            //string utmCampaign = queryString["utm_campaign"] != null ? queryString["utm_campaign"].Trim().ToLower() : string.Empty;
                            //CampaignMasters campaign = null;
                            //if (affiliate > 0)
                            //{
                            //    campaign = Utility.PortalData.Campaigns.Where<CampaignMasters>(o => o.AffiliateId == affiliate).FirstOrDefault<CampaignMasters>();
                            //}
                            //else
                            //{
                            //    campaign = Utility.PortalData.Campaigns.Where<CampaignMasters>(o =>
                            //    o.UtmSource.Equals(utmSource, StringComparison.OrdinalIgnoreCase)
                            //    && !string.IsNullOrEmpty(o.UtmCampaign) && !string.IsNullOrEmpty(utmCampaign) && o.UtmCampaign.Equals(utmCampaign, StringComparison.OrdinalIgnoreCase)
                            //    ).FirstOrDefault<CampaignMasters>();
                            //    if (campaign == null)
                            //    {
                            //        campaign = Utility.PortalData.Campaigns.Where<CampaignMasters>(o => o.UtmSource.Equals(utmSource, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<CampaignMasters>();
                            //    }
                            //}
                            //if (campaign != null)
                            //{
                            //    CampaignMasters finalCamp = new CampaignMasters()
                            //    {
                            //        AffiliateId = campaign.AffiliateId,
                            //        CampaignName = campaign.CampaignName,
                            //        UtmSource = campaign.UtmSource,
                            //        PortalID = campaign.PortalID,
                            //        TollFreeNumber = campaign.TollFreeNumber
                            //    };

                            //    if (!string.IsNullOrEmpty(utmCampaign))
                            //    {
                            //        finalCamp.UtmCampaign = utmCampaign;
                            //    }

                            //    if (!string.IsNullOrEmpty(campaign.UtmMedium) && queryString[campaign.UtmMedium] != null)
                            //    {
                            //        finalCamp.UtmMedium = queryString[campaign.UtmMedium].ToString().Trim();
                            //    }
                            //    else
                            //    {
                            //        finalCamp.UtmMedium = null;
                            //    }

                            //    if (!string.IsNullOrEmpty(campaign.UtmKeyword) && queryString[campaign.UtmKeyword] != null)
                            //    {
                            //        finalCamp.UtmKeyword = queryString[campaign.UtmKeyword].ToString().Trim();
                            //    }
                            //    else
                            //    {
                            //        finalCamp.UtmKeyword = null;
                            //    }

                            //    if (!string.IsNullOrEmpty(campaign.UtmContent) && queryString[campaign.UtmContent] != null)
                            //    {
                            //        finalCamp.UtmContent = queryString[campaign.UtmContent].ToString().Trim();
                            //    }
                            //    else
                            //    {
                            //        finalCamp.UtmContent = null;
                            //    }
                            //    if (!string.IsNullOrEmpty(campaign.UtmTerm) && queryString[campaign.UtmTerm] != null)
                            //    {
                            //        finalCamp.UtmTerm = queryString[campaign.UtmTerm].ToString().Trim();
                            //    }
                            //    else
                            //    {
                            //        finalCamp.UtmTerm = null;
                            //    }


                            //    if (!string.IsNullOrEmpty(campaign.ClickedId) && queryString[campaign.ClickedId] != null)
                            //    {
                            //        finalCamp.ClickedId = queryString[campaign.ClickedId].ToString().Trim();
                            //    }
                            //    else
                            //    {
                            //        finalCamp.ClickedId = null;
                            //    }

                            //    if (!string.IsNullOrEmpty(campaign.UtmPublisher) && queryString[campaign.UtmPublisher] != null)
                            //    {
                            //        finalCamp.UtmPublisher = queryString[campaign.UtmPublisher].ToString().Trim();
                            //    }
                            //    else
                            //    {
                            //        finalCamp.UtmPublisher = null;
                            //    }

                            //    if (!string.IsNullOrEmpty(campaign.UtmPublisherId) && queryString[campaign.UtmPublisherId] != null)
                            //    {
                            //        finalCamp.UtmPublisherId = queryString[campaign.UtmPublisherId].ToString().Trim();
                            //    }
                            //    else
                            //    {
                            //        finalCamp.UtmPublisherId = null;
                            //    }
                            //    if (!string.IsNullOrEmpty(campaign.UtmChannelId) && queryString[campaign.UtmChannelId] != null)
                            //    {
                            //        finalCamp.UtmChannelId = queryString[campaign.UtmChannelId].ToString().Trim();
                            //    }
                            //    else
                            //    {
                            //        finalCamp.UtmChannelId = null;
                            //    }

                            //    search.AffiliateId = finalCamp.AffiliateId;
                            //    search.ClickedId = finalCamp.ClickedId;
                            //    finalCamp.Id = utmPageType;

                            //}

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("QUERY STRING:GetAffiliateSearch:{0}", queryString != null ? queryString.ToString() : ""));
                Logger.Error(string.Format("GetAffiliateSearch:InnerException|Message:{0}", ex != null ? ex.InnerException.ToString() + "|" + ex.Message + "|" + ex.StackTrace : ""));
            }
            return search;
        }


        public static FlightSearch GetAffiliateSearchRoutes(string _from, string _to, string _triptype, string _airline, string _cabin, NameValueCollection queryString, HttpRequestBase _request)
        {
            FlightSearch search = null;
            try
            {


                if (!string.IsNullOrEmpty(_from) && !string.IsNullOrEmpty(_to))
                {
                    Airports tempOriAirport, tempDestAirport = null;
                    string ori = _from.Trim();
                    string dest = _to.Trim();
                    string origin = string.Empty, destination = string.Empty;
                    string originName = string.Empty, destinationName = string.Empty;
                    tempOriAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(ori, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                    if (tempOriAirport != null)
                    {
                        origin = tempOriAirport.AirportCode;
                        originName = tempOriAirport.AirportName;
                    }
                    else
                    {
                        tempOriAirport = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(ori, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                        if (tempOriAirport != null)
                        {
                            origin = tempOriAirport.CityCode;
                            originName = tempOriAirport.City;
                        }
                        else
                        {
                            Utility.Logger.Info("CODE NOT FOUND:" + ori);
                        }
                    }

                    tempDestAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(dest, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                    if (tempDestAirport != null)
                    {
                        destination = tempDestAirport.AirportCode;
                        destinationName = tempDestAirport.AirportName;
                    }
                    else
                    {
                        tempDestAirport = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(dest, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                        if (tempDestAirport != null)
                        {
                            destination = tempDestAirport.CityCode;
                            destinationName = tempDestAirport.City;
                        }
                        else
                        {
                            Utility.Logger.Info("CODE NOT FOUND:" + dest);
                        }
                    }

                    if (tempOriAirport != null && tempDestAirport != null)
                    {
                        search = new FlightSearch()
                        {
                            Origin = origin,
                            OriginAirportName = originName,
                            OriginSearch = string.Format("{0} - {1}, {2}", origin, tempOriAirport.City, tempOriAirport.CountryName),
                            Destination = destination,
                            DestAirportName = destinationName,
                            DestinationSearch = string.Format("{0} - {1}, {2}", destination, tempDestAirport.City, tempDestAirport.CountryName)
                        };

                        search.Adult = 2;



                        if (!string.IsNullOrEmpty(_triptype))
                        {
                            if (_triptype.Trim().Equals("2"))
                            {
                                search.TripType = TripType.ROUNDTRIP;
                            }
                            else
                            {
                                search.TripType = TripType.ONEWAY;
                            }
                        }
                        else
                        {
                            search.TripType = TripType.ROUNDTRIP;
                        }

                        //DateTime tempdate;
                        //if (queryString["departure"] != null && DateTime.TryParseExact(queryString["departure"].ToString().Trim(), "M-d-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempdate))
                        //{
                        //    search.Departure = tempdate;
                        //    if (search.TripType == TripType.ROUNDTRIP)
                        //    {
                        //        if (DateTime.TryParseExact(queryString["return"].ToString().Trim(), "M-d-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempdate))
                        //        {
                        //            search.Return = tempdate;
                        //        }
                        //        else
                        //        {
                        //            search.Return = search.Departure.AddDays(7);
                        //        }
                        //    }
                        //}
                        //else
                        //{

                        //}
                        search.Departure = DateTime.Now.AddDays(15).Date;
                        if (search.TripType == TripType.ROUNDTRIP)
                        {
                            search.Return = search.Departure.AddDays(7);
                        }

                        if (!string.IsNullOrEmpty(_cabin))
                        {
                            string cabin = _cabin.ToString().ToLower();
                            switch (cabin)
                            {
                                case "economy":
                                    search.Cabin = CabinType.EconomyCoach;
                                    break;
                                case "business":
                                    search.Cabin = CabinType.Business;
                                    break;
                                case "first":
                                    search.Cabin = CabinType.First;
                                    break;
                                default:
                                    search.Cabin = CabinType.EconomyCoach;
                                    break;
                            }
                        }
                        else
                        {
                            search.Cabin = CabinType.EconomyCoach;
                        }

                        if (!string.IsNullOrEmpty(_airline))
                        {
                            string air = _airline.Trim().ToLower();
                            Airlines airline = null;
                            if (air.Length == 2)
                            {
                                airline = Utility.Airlines.Where<Airlines>(o => o.Code.Equals(air, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airlines>();
                                if (airline != null)
                                {
                                    search.PreferredCarrier = airline.Code;
                                }
                            }
                        }

                        if (queryString["utm_source"] != null)
                        {
                            int affiliate = 0;
                            string utmSource = queryString["utm_source"].Trim().ToLower();
                            string utmCampaign = queryString["utm_campaign"] != null ? queryString["utm_campaign"].Trim().ToLower() : string.Empty;
                            bool isUtmSourceNumber = int.TryParse(utmSource, out affiliate);
                            CampaignMasters campaign = null;
                            if (isUtmSourceNumber && affiliate > 0)
                            {
                                campaign = Utility.PortalData.Campaigns.Where<CampaignMasters>(o => o.AffiliateId == affiliate).FirstOrDefault<CampaignMasters>();
                            }
                            else
                            {
                                campaign = Utility.PortalData.Campaigns.Where<CampaignMasters>(o =>
                               o.UtmSource.Equals(utmSource, StringComparison.OrdinalIgnoreCase)
                               && !string.IsNullOrEmpty(o.UtmCampaign) && !string.IsNullOrEmpty(utmCampaign) && o.UtmCampaign.Equals(utmCampaign, StringComparison.OrdinalIgnoreCase)
                               ).FirstOrDefault<CampaignMasters>();
                                if (campaign == null)
                                {
                                    campaign = Utility.PortalData.Campaigns.Where<CampaignMasters>(o => o.UtmSource.Equals(utmSource, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<CampaignMasters>();
                                }
                            }
                            if (campaign != null)
                            {

                                if (!string.IsNullOrEmpty(campaign.ClickedId) && queryString[campaign.ClickedId] != null)
                                {
                                    campaign.ClickedId = queryString[campaign.ClickedId].ToString().Trim();
                                }
                                else
                                {
                                    campaign.ClickedId = null;
                                }
                                search.AffiliateId = campaign.AffiliateId;
                                search.ClickedId = campaign.ClickedId;
                                int utmPageType = 0;
                                if (queryString["utm_page"] != null)
                                {
                                    int.TryParse(queryString["utm_page"].ToString(), out utmPageType);
                                }
                                campaign.Id = utmPageType;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("QUERY STRING:GetAffiliateSearchRoutes:{0}", queryString != null ? queryString.ToString() : ""));
                Logger.Error(string.Format("GetAffiliateSearchRoutes:InnerException|Message:{0}", ex != null ? ex.InnerException.ToString() + "|" + ex.Message + "|" + ex.StackTrace : ""));
            }
            return search;
        }


        /// <summary>
        /// Deserialize stream
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="reader">StreamReader</param>
        /// <returns></returns>
        public static T GetFileDeserialize<T>(StreamReader reader)
        {
            T response = default(T);
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(T));
                object obj = deserializer.Deserialize(reader);
                response = (T)obj;
                reader.Close();
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Business.Utility.GetFileDeserialize<T>:" + ex.ToString());
            }
            return response;
        }

        /// <summary>
        /// Get Enum Description
        /// </summary>
        /// <param name="enumValue">Enum</param>
        /// <returns>string</returns>
        public static string GetEnumDescription(Enum enumValue)
        {
            string enumDesc = string.Empty;

            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                object[] attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return enumDesc;
        }

        public static Airports GetAirportDetails(string code, OriginType originType = OriginType.Airport)
        {
            Airports airports = null;
            try
            {
                switch (originType)
                {
                    case OriginType.Airport:
                        airports = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                        break;
                    case OriginType.City:
                        airports = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                        break;
                    default:
                        airports = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                        if (airports == null)
                        {
                            airports = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Business.Utility.GetAirportDetails<T>:" + ex.ToString());
            }
            return airports;
        }
        public static string GetAirportCity(string airport)
        {
            string response = string.Empty;
            try
            {
                Airports airports = GetAirportDetails(airport);
                if (airports != null)
                {
                    response = string.Format("{0}, {1}", airports.City, airports.CountryCode);
                }
                else
                {
                    response = airport;
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Business.Utility.GetAirportInfo<T>:" + ex.ToString());
            }
            return response;
        }
        public static string GetCountryCode(string _code)
        {
            string response = string.Empty;
            Airports airports = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(_code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
            if (airports != null)
            {
                response = string.Format("{0}", airports.CountryCode);
            }
            else
            {
                airports = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(_code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                if (airports != null)
                {
                    response = string.Format("{0}", airports.CountryCode);
                }
            }
            return response;
        }

        public static string GetCountryName(string _code)
        {
            string response = string.Empty;
            Airports airports = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(_code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
            if (airports != null)
            {
                response = string.Format("{0}", airports.CountryName);
            }
            else
            {
                airports = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(_code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                if (airports != null)
                {
                    response = string.Format("{0}", airports.CountryName);
                }
            }
            return response;
        }

        public static string GetAirportName(string _code)
        {
            string response = string.Empty;
            Airports airports = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(_code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
            if (airports != null)
            {
                response = string.Format("{0}", airports.AirportName);
            }
            else
            {
                airports = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(_code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                if (airports != null)
                {
                    response = string.Format("{0}", airports.City);
                }
            }
            return response;
        }

        public static string GetCityName(string _code)
        {
            string response = string.Empty;
            Airports airports = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(_code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
            if (airports != null)
            {
                response = string.Format("{0}", airports.City);
            }
            else
            {
                airports = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(_code, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airports>();
                if (airports != null)
                {
                    response = string.Format("{0}", airports.City);
                }
            }
            return response;
        }
        private static T DeSerialize<T>(string filepath)
        {
            T obj = default(T);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                string lines = string.Empty;
                using (TextReader reader = File.OpenText(filepath))
                {
                    obj = (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Configration.ConfigManager.DeSerialize" + ex.ToString());
            }
            return obj;
        }

        public static string GetDuration(TimeSpan? _duration)
        {
            string response = string.Empty;
            try
            {
                if (_duration == null || _duration == TimeSpan.MinValue)
                {
                    response = "--";
                }
                else
                {
                    TimeSpan tempSpan = (_duration ?? TimeSpan.MinValue);
                    response = string.Format("{0}h {1}m", (int)tempSpan.TotalHours, tempSpan.Minutes);


                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("EasyPro.GetDuration" + ex.ToString());
            }
            return response;
        }

        public static TimeSpan GetTotalLayoverTime(List<Segments> segments)
        {
            TimeSpan timeSpan = TimeSpan.MinValue;
            try
            {

                if (segments != null && segments.Count > 1)
                {
                    for (int i = 0; i <= segments.Count - 1; i++)
                    {
                        if (i != 0)
                        {
                            if (timeSpan == TimeSpan.MinValue)
                            {
                                timeSpan = new DateTime(segments[i].Departure.Year, segments[i].Departure.Month, segments[i].Departure.Day, segments[i].DepartureTime.Hours, segments[i].DepartureTime.Minutes, 0) - new DateTime(segments[i - 1].Arrival.Year, segments[i - 1].Arrival.Month, segments[i - 1].Arrival.Day, segments[i - 1].ArrivalTime.Hours, segments[i - 1].ArrivalTime.Minutes, 0);
                            }
                            else
                            {
                                timeSpan = timeSpan + (new DateTime(segments[i].Departure.Year, segments[i].Departure.Month, segments[i].Departure.Day, segments[i].DepartureTime.Hours, segments[i].DepartureTime.Minutes, 0) - new DateTime(segments[i - 1].Arrival.Year, segments[i - 1].Arrival.Month, segments[i - 1].Arrival.Day, segments[i - 1].ArrivalTime.Hours, segments[i - 1].ArrivalTime.Minutes, 0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.GetLayoverTime" + ex.ToString());
            }
            return timeSpan;
        }

        public static int GetTotalStops(List<Segments> segments)
        {
            int stop = 0;
            try
            {

                if (segments != null && segments.Count > 1)
                {
                    stop = segments.Count - 1;
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.GetTotalStops" + ex.ToString());
            }
            return stop;
        }


        public static List<State> GetStates(string countryCode)
        {
            List<State> response = null;
            try
            {
                if (!string.IsNullOrEmpty(countryCode))
                {
                    switch (countryCode.ToUpper())
                    {
                        case "US":
                            response = Utility.Settings.USState;
                            break;
                        case "CA":
                            response = Utility.Settings.CanadaState;
                            break;
                        default:
                            response = Utility.Settings.USState;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Common.Utility.GetStates" + ex.ToString());
            }
            return response;
        }
        public static string GetTitleCase(string text)
        {
            return textInfo.ToTitleCase(text);
        }
        public static bool IsValidGuid(string id)
        {
            return id.Length == 32 ? true : false;
        }
        public static string GetGuid()
        {
            return Guid.NewGuid().ToString("N");
        }
        public static string GetCreditCardLastDigits(string creditCardNumber)
        {
            string response = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(creditCardNumber) && creditCardNumber.Length > 4)
                {
                    creditCardNumber = creditCardNumber.Replace(" ", "");
                    response = creditCardNumber.Substring(creditCardNumber.Length - 4, 4);
                }
                else
                {
                    response = creditCardNumber;
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Common.Utility.GetStates" + ex.ToString());
            }
            return response;
        }
        public static string GetCardType(int cardType)
        {
            string response = "";
            switch (cardType)
            {
                case 1:
                    response = "VI";
                    break;
                case 2:
                    response = "MC";
                    break;
                case 3:
                    response = "AX";
                    break;
                case 4:
                    response = "DC";
                    break;
                case 5:
                    response = "DS";
                    break;
                default:
                    response = "";
                    break;
            }
            return response;
        }
        public static string GetPhoneLastDigits(string phoneNumber)
        {
            string response = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length > 4)
                {
                    phoneNumber = phoneNumber.Replace(" ", "");
                    response = phoneNumber.Substring(phoneNumber.Length - 4, 4);
                }
                else
                {
                    response = phoneNumber;
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Common.Utility.GetPhoneLastDigits" + ex.ToString());
            }
            return response;
        }

        public static string GetAilineName(string airlineCode)
        {
            string response = airlineCode;
            try
            {
                if (!string.IsNullOrEmpty(airlineCode))
                {
                    Airlines airline = Utility.Airlines.Where<Airlines>(o => o.Code.Equals(airlineCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airlines>();
                    if (airline != null)
                    {
                        response = airline.Name;
                    }
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Common.Utility.GetAilineName" + ex.ToString());
            }
            return response;
        }

        public static void SetAirContextCache(string _key, AirContext _airContext)
        {
            try
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                DateTime expTime = DateTime.Now.AddMinutes(20);
                policy.AbsoluteExpiration = expTime;
                _airContext.CacheExpiryTime = expTime;
                Utility.GLobalCache.Set(_key.ToLower(), _airContext, policy);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Common.Utility.SetCache" + ex.ToString());
            }
        }
        public static void RemoveAirContextCache(string _key)
        {
            try
            {
                Utility.GLobalCache.Remove(_key);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Common.Utility.RemoveAirContextCache" + ex.ToString());
            }
        }
        public static AirContext GetAirContextCache(string _key)
        {
            AirContext airContext = null;
            try
            {
                airContext = (AirContext)Utility.GLobalCache.Get(_key);
                if (airContext != null && airContext.CacheExpiryTime.AddMinutes(-5) <= DateTime.Now)
                {
                    Task.Factory.StartNew(() => SetAirContextCache(_key.ToLower(), airContext));
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Common.Utility.GetCache" + ex.ToString());
            }
            return airContext;
        }

        public static void SetIncompleteBookingCache(string _key, IncompleteBookingContext _context)
        {
            try
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                DateTime expTime = DateTime.Now.AddMinutes(300);
                policy.AbsoluteExpiration = expTime;
                _context.CacheExpiryTime = expTime;
                Utility.GLobalCache.Set(_key.ToLower(), _context, policy);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Common.Utility.SetIncompleteBookingCache" + ex.ToString());
            }
        }
        public static IncompleteBookingContext GetIncompleteBookingCache(string _key)
        {
            IncompleteBookingContext airContext = null;
            try
            {
                airContext = (IncompleteBookingContext)Utility.GLobalCache.Get(_key);
                if (airContext != null && airContext.CacheExpiryTime.AddMinutes(-5) <= DateTime.Now)
                {
                    Task.Factory.StartNew(() => SetIncompleteBookingCache(_key.ToLower(), airContext));
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Common.Utility.GetIncompleteBookingCache" + ex.ToString());
            }
            return airContext;
        }

        public static float GetAllTotalAmount(BookingDetail Model)
        {
            float totalFare = 0;
            try
            {
                if (Model != null && Model.BagInsuranc != null)
                {
                    totalFare = totalFare + (float)Model.BagInsuranc.TotalPrice;
                }
                if (Model != null && Model.TravelerInsurance != null)
                {
                    totalFare = totalFare + (float)Model.TravelerInsurance.TotalPrice;
                }

                totalFare = totalFare + Model.Contract.TotalGDSFareV2;
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Utility.GetAllTotalAmount" + ex.ToString());
            }

            return totalFare;
        }
        #region Private
        private static bool LoadPortalConfigration()
        {
            bool isSuccess = false;
            try
            {
                Utility.Logger.Info("PortalSettings|BEGIN");
                string path = Path.Combine(HttpRuntime.AppDomainAppPath, string.Format("Configration\\Portal.config"));
                StreamReader reader = new StreamReader(path);
                Utility.PortalSettings = Utility.GetFileDeserialize<PortalSettings>(reader);
                isSuccess = true;
                Utility.Logger.Info("PortalSettings|END");
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Common.Utility.LoadPortalConfigration" + ex.ToString());
            }
            return isSuccess;
        }



        private static bool IsLoadConfig()
        {
            bool response = false;
            try
            {
                Utility.Logger.Debug("Common.Utility.IsLoadConfig:Begin");
                string path = Path.Combine(HttpRuntime.AppDomainAppPath, string.Format("Configration\\Settings.config"));
                StreamReader reader = new StreamReader(path);
                Utility.Settings = Utility.GetFileDeserialize<Settings>(reader);
                response = LoadPortalConfigration();
                Utility.Logger.Info("Common.Utility.IsLoadConfig:Load Data Successfully.");
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Common.Utility.IsLoadProtalSetting:" + ex.ToString());
            }
            return response;
        }
        #endregion





        public static float GetMinimumPrice(List<Contract> _contracts, ContractType contractType)
        {
            float minPrice = 0.0F;
            switch (contractType)
            {

                case ContractType.Actual:

                    var tempContract = _contracts.Where(o => o.ContractType == ContractType.Actual);
                    if (tempContract != null && tempContract.Count() > 0)
                    {
                        minPrice = tempContract.Min(o => o.AdultFare.TotalFarePPax);
                    }
                    break;
                case ContractType.NearBy:
                    var tempContractNear = _contracts.Where(o => o.IsPhoneOnly != true && o.ContractType == ContractType.NearByFlexi || o.ContractType == ContractType.NearBy);
                    if (tempContractNear != null && tempContractNear.Count() > 0)
                    {
                        minPrice = tempContractNear.Min(o => o.AdultFare.TotalFarePPax);
                    }
                    break;
                case ContractType.Flexi:
                    var tempContractFlex = _contracts.Where(o => o.IsPhoneOnly != true && o.ContractType == ContractType.NearByFlexi || o.ContractType == ContractType.Flexi);
                    if (tempContractFlex != null && tempContractFlex.Count() > 0)
                    {
                        minPrice = tempContractFlex.Min(o => o.AdultFare.TotalFarePPax);
                    }
                    break;
                case ContractType.PhoneOnly:
                    var tempContractPhoneOnly = _contracts.Where(o => o.IsPhoneOnly);
                    if (tempContractPhoneOnly != null && tempContractPhoneOnly.Count() > 0)
                    {
                        minPrice = tempContractPhoneOnly.Min(o => o.AdultFare.TotalFarePPax);
                    }
                    break;
            }
            return minPrice;
        }


        public static string GetTravellerPaxType(TravellerPaxType paxType)
        {
            string response = string.Empty;
            switch (paxType)
            {

                case TravellerPaxType.ADT:
                    response = "Adult";
                    break;
                case TravellerPaxType.CHD:
                    response = "Child";
                    break;
                case TravellerPaxType.INS:
                    response = "Seat Infant";
                    break;
                case TravellerPaxType.INL:
                    response = "Lap Infant";
                    break;
                default:
                    response = "Adult";
                    break;
            }
            return response;
        }

        public static void RemoveSoldoutContract(AirContext context)
        {
            throw new NotImplementedException();
        }
        public static DateTime GetZoneDateTime(DateTime sourceDatetime, BookingTimeZone destinationTimeZone)
        {

            return TimeZoneInfo.ConvertTime(sourceDatetime, TimeZoneInfo.FindSystemTimeZoneById(Utility.GetEnumDescription(destinationTimeZone)));
        }

        public static string GetClientIP(HttpContext _httpContext)
        {
            string customerIP = String.Empty;
            HttpRequest httpReq = _httpContext.Request;

            customerIP = httpReq.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(customerIP))
            {
                string[] addresses = customerIP.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }
            return httpReq.ServerVariables["REMOTE_ADDR"]; ;
        }
        public static bool GetDeviceType(string _userAgent)
        {
            //string ret = "";
            bool Agent = false;
            // Check if user agent is a smart TV - http://goo.gl/FocDk
            if (Regex.IsMatch(_userAgent, @"GoogleTV|SmartTV|Internet.TV|NetCast|NETTV|AppleTV|boxee|Kylo|Roku|DLNADOC|CE\-HTML", RegexOptions.IgnoreCase))
            {
                //ret = "tv";
                Agent = false;
            }
            // Check if user agent is a TV Based Gaming Console
            else if (Regex.IsMatch(_userAgent, "Xbox|PLAYSTATION.3|Wii", RegexOptions.IgnoreCase))
            {
                //ret = "tv";
                Agent = false;
            }
            // Check if user agent is a Tablet
            else if ((Regex.IsMatch(_userAgent, "iP(a|ro)d", RegexOptions.IgnoreCase) || (Regex.IsMatch(_userAgent, "tablet", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(_userAgent, "RX-34", RegexOptions.IgnoreCase)) || (Regex.IsMatch(_userAgent, "FOLIO", RegexOptions.IgnoreCase))))
            {
                //ret = "tablet";
                Agent = false;
            }
            // Check if user agent is an Android Tablet
            else if ((Regex.IsMatch(_userAgent, "Linux", RegexOptions.IgnoreCase)) && (Regex.IsMatch(_userAgent, "Android", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(_userAgent, "Fennec|mobi|HTC.Magic|HTCX06HT|Nexus.One|SC-02B|fone.945", RegexOptions.IgnoreCase)))
            {
                // ret = "tablet";
                Agent = false;
            }
            // Check if user agent is a Kindle or Kindle Fire
            else if ((Regex.IsMatch(_userAgent, "Kindle", RegexOptions.IgnoreCase)) || (Regex.IsMatch(_userAgent, "Mac.OS", RegexOptions.IgnoreCase)) && (Regex.IsMatch(_userAgent, "Silk", RegexOptions.IgnoreCase)))
            {
                //ret = "tablet";
                Agent = false;
            }
            // Check if user agent is a pre Android 3.0 Tablet
            else if ((Regex.IsMatch(_userAgent, @"GT-P10|SC-01C|SHW-M180S|SGH-T849|SCH-I800|SHW-M180L|SPH-P100|SGH-I987|zt180|HTC(.Flyer|\\_Flyer)|Sprint.ATP51|ViewPad7|pandigital(sprnova|nova)|Ideos.S7|Dell.Streak.7|Advent.Vega|A101IT|A70BHT|MID7015|Next2|nook", RegexOptions.IgnoreCase)) || (Regex.IsMatch(_userAgent, "MB511", RegexOptions.IgnoreCase)) && (Regex.IsMatch(_userAgent, "RUTEM", RegexOptions.IgnoreCase)))
            {
                //ret = "tablet";
                Agent = false;
            }
            // Check if user agent is unique Mobile User Agent
            else if ((Regex.IsMatch(_userAgent, "BOLT|Fennec|Iris|Maemo|Minimo|Mobi|mowser|NetFront|Novarra|Prism|RX-34|Skyfire|Tear|XV6875|XV6975|Google.Wireless.Transcoder", RegexOptions.IgnoreCase)))
            {
                //ret = "mobile";
                Agent = true;
            }
            // Check if user agent is an odd Opera User Agent - http://goo.gl/nK90K
            else if ((Regex.IsMatch(_userAgent, "Opera", RegexOptions.IgnoreCase)) && (Regex.IsMatch(_userAgent, "Windows.NT.5", RegexOptions.IgnoreCase)) && (Regex.IsMatch(_userAgent, @"HTC|Xda|Mini|Vario|SAMSUNG\-GT\-i8000|SAMSUNG\-SGH\-i9", RegexOptions.IgnoreCase)))
            {
                //ret = "mobile";
                Agent = true;
            }
            // Check if user agent is Windows Desktop
            else if ((Regex.IsMatch(_userAgent, "Windows.(NT|XP|ME|9)")) && (!Regex.IsMatch(_userAgent, "Phone", RegexOptions.IgnoreCase)) || (Regex.IsMatch(_userAgent, "Win(9|.9|NT)", RegexOptions.IgnoreCase)))
            {
                //ret = "desktop";
                Agent = false;
            }
            // Check if agent is Mac Desktop
            else if ((Regex.IsMatch(_userAgent, "Macintosh|PowerPC", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(_userAgent, "Silk", RegexOptions.IgnoreCase)))
            {
                //ret = "desktop";
                Agent = false;
            }
            // Check if user agent is a Linux Desktop
            else if ((Regex.IsMatch(_userAgent, "Linux", RegexOptions.IgnoreCase)) && (Regex.IsMatch(_userAgent, "X11", RegexOptions.IgnoreCase)))
            {
                //ret = "desktop";
                Agent = false;
            }
            // Check if user agent is a Solaris, SunOS, BSD Desktop
            else if ((Regex.IsMatch(_userAgent, "Solaris|SunOS|BSD", RegexOptions.IgnoreCase)))
            {
                //ret = "desktop";
                Agent = false;
            }
            // Check if user agent is a Desktop BOT/Crawler/Spider
            else if ((Regex.IsMatch(_userAgent, "Bot|Crawler|Spider|Yahoo|ia_archiver|Covario-IDS|findlinks|DataparkSearch|larbin|Mediapartners-Google|NG-Search|Snappy|Teoma|Jeeves|TinEye", RegexOptions.IgnoreCase)) && (!Regex.IsMatch(_userAgent, "Mobile", RegexOptions.IgnoreCase)))
            {
                //ret = "desktop";
                Agent = false;
            }
            // Otherwise assume it is a Mobile Device
            else
            {
                //ret = "mobile";
                Agent = true;
            }
            //return ret;
            return Agent;
        }
        public static FlightSearch GetDefaultSearch()
        {
            return new FlightSearch()
            {
                Adult = 1,
                Cabin = CabinType.Economy,
                TripType = TripType.ROUNDTRIP,
                OriginSearch = "",
                Destination = "",
                Departure = DateTime.MinValue,
                Return = DateTime.MinValue


            };
        }


        public static string GetParamEncryptKey(int tid, string email, string password)
        {
            return Encrypt(string.Format("{0}|{1}|{2}", tid, email, password).Replace(" ", "").ToLower(), Utility.Settings.EncryptionKey);
        }
        public static string Encrypt(string _clearText, string _encryptionKey)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(_clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    _clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return _clearText;
        }
        public static string Decrypt(string _clearText, string _encryptionKey)
        {
            byte[] cipherBytes = Convert.FromBase64String(_clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    _clearText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return _clearText;
        }
        /// <summary>
        /// Create Random Password
        /// </summary>
        /// <param name="passwordLength"></param>
        /// <returns></returns>
        public static string CreateRandomPassword(int passwordLength)
        {
            string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[passwordLength];
            Random rd = new Random();
            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }
        private static PortalStaticData GetPortalData(int portalId, string connectionString)
        {
            PortalStaticData response = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter sqlDataAdapter;
                    DataSet ds = new DataSet();
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "uspGetPortalStaticData";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@portalId", portalId);
                        sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(ds);
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            response = new PortalStaticData();

                            for (int i = 0; i <= ds.Tables.Count - 1; i++)
                            {
                                if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                {
                                    switch (i)
                                    {
                                        case 0://CampaignMaster
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.Campaigns = new List<CampaignMasters>();
                                                CampaignMasters campaign = null;
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    campaign = new CampaignMasters();
                                                    campaign.AffiliateId = Convert.ToInt32(row["AffiliateId"]);
                                                    campaign.PortalID = Convert.ToInt32(row["PortalID"]);
                                                    campaign.CampaignName = row["CampaignName"] == DBNull.Value ? null : Convert.ToString(row["CampaignName"]);
                                                    campaign.TollFreeNumber = row["TollFreeNumber"] == DBNull.Value ? null : Convert.ToString(row["TollFreeNumber"]);
                                                    campaign.UtmSource = row["UtmSource"] == DBNull.Value ? null : Convert.ToString(row["UtmSource"]);
                                                    campaign.UtmMedium = row["UtmMedium"] == DBNull.Value ? null : Convert.ToString(row["UtmMedium"]);
                                                    campaign.UtmCampaign = row["UtmCampaign"] == DBNull.Value ? null : Convert.ToString(row["UtmCampaign"]);
                                                    campaign.UtmTerm = row["UtmTerm"] == DBNull.Value ? null : Convert.ToString(row["UtmTerm"]);
                                                    campaign.UtmContent = row["UtmContent"] == DBNull.Value ? null : Convert.ToString(row["UtmContent"]);
                                                    campaign.ClickedId = row["ClickedId"] == DBNull.Value ? null : Convert.ToString(row["ClickedId"]);
                                                    campaign.UtmPublisher = row["UtmPublisher"] == DBNull.Value ? null : Convert.ToString(row["UtmPublisher"]);
                                                    campaign.UtmPublisherId = row["UtmPublisherId"] == DBNull.Value ? null : Convert.ToString(row["UtmPublisherId"]);
                                                    campaign.UtmChannelId = row["UtmChannelId"] == DBNull.Value ? null : Convert.ToString(row["UtmChannelId"]);
                                                    campaign.UtmKeyword = row["UtmKeyword"] == DBNull.Value ? null : Convert.ToString(row["UtmKeyword"]);
                                                    response.Campaigns.Add(campaign);
                                                }
                                            }
                                            break;
                                        case 1: //Airline Data
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.AirlinesData = new List<DealAirlines>();
                                                DealAirlines airline = null;
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    airline = new DealAirlines();
                                                    airline.Id = Convert.ToInt32(row["Id"]);
                                                    airline.PortalID = Convert.ToInt32(row["PortalID"]);
                                                    airline.Title = row["Title"] == DBNull.Value ? null : Convert.ToString(row["Title"]);
                                                    airline.Description = row["Description"] == DBNull.Value ? null : Convert.ToString(row["Description"]);
                                                    airline.Keywords = row["Keywords"] == DBNull.Value ? null : Convert.ToString(row["Keywords"]);
                                                    airline.AirlineName = row["AirlineName"] == DBNull.Value ? null : Convert.ToString(row["AirlineName"]);
                                                    airline.AirlineCode = row["AirlineCode"] == DBNull.Value ? null : Convert.ToString(row["AirlineCode"]);
                                                    airline.BannerTitle = row["BannerTitle"] == DBNull.Value ? null : Convert.ToString(row["BannerTitle"]);
                                                    airline.AltTag = row["AltTag"] == DBNull.Value ? null : Convert.ToString(row["AltTag"]);
                                                    airline.AboutAirline = row["AboutAirline"] == DBNull.Value ? null : Convert.ToString(row["AboutAirline"]);
                                                    airline.InFlightAmenities = row["InFlightAmenities"] == DBNull.Value ? null : Convert.ToString(row["InFlightAmenities"]);
                                                    airline.OnlineCheckin = row["OnlineCheckin"] == DBNull.Value ? null : Convert.ToString(row["OnlineCheckin"]);
                                                    airline.OtherAilineCode = row["OtherAilineCode"] == DBNull.Value ? null : Convert.ToString(row["OtherAilineCode"]);
                                                    airline.TipsToSave = row["TipsToSave"] == DBNull.Value ? null : Convert.ToString(row["TipsToSave"]);

                                                    airline.DealGuid = row["DealGuid"] == DBNull.Value ? null : Convert.ToString(row["DealGuid"]);
                                                    airline.DealFrom = row["DealFrom"] == DBNull.Value ? null : Convert.ToString(row["DealFrom"]);
                                                    airline.DealTo = row["DealTo"] == DBNull.Value ? null : Convert.ToString(row["DealTo"]);
                                                    airline.Extra = row["Extra"] == DBNull.Value ? null : Convert.ToString(row["Extra"]);
                                                    response.AirlinesData.Add(airline);
                                                }
                                            }
                                            break;
                                        case 2: //Destination
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.Destinations = new List<DealDestinations>();
                                                DealDestinations destination = null;
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    destination = new DealDestinations();
                                                    destination.Id = Convert.ToInt32(row["Id"]);
                                                    destination.PortalID = Convert.ToInt32(row["PortalID"]);
                                                    destination.Title = row["Title"] == DBNull.Value ? null : Convert.ToString(row["Title"]);
                                                    destination.Description = row["Description"] == DBNull.Value ? null : Convert.ToString(row["Description"]);
                                                    destination.Keywords = row["Keywords"] == DBNull.Value ? null : Convert.ToString(row["Keywords"]);
                                                    destination.DestinationName = row["DestinationName"] == DBNull.Value ? null : Convert.ToString(row["DestinationName"]);
                                                    destination.DestinationCode = row["DestinationCode"] == DBNull.Value ? null : Convert.ToString(row["DestinationCode"]);
                                                    destination.BannerTitle = row["BannerTitle"] == DBNull.Value ? null : Convert.ToString(row["BannerTitle"]);
                                                    destination.AltTag = row["AltTag"] == DBNull.Value ? null : Convert.ToString(row["AltTag"]);
                                                    destination.URL = row["URL"] == DBNull.Value ? null : Convert.ToString(row["URL"]);
                                                    destination.StateCountry = row["StateCountry"] == DBNull.Value ? null : Convert.ToString(row["StateCountry"]);
                                                    destination.IsDomestic = row["IsDomestic"] == DBNull.Value ? true : Convert.ToBoolean(row["IsDomestic"]);
                                                    destination.AboutDestination = row["AboutDestination"] == DBNull.Value ? null : Convert.ToString(row["AboutDestination"]);
                                                    destination.TopTouristAttractions = row["TopTouristAttractions"] == DBNull.Value ? null : Convert.ToString(row["TopTouristAttractions"]);
                                                    destination.BestSeason = row["BestSeason"] == DBNull.Value ? null : Convert.ToString(row["BestSeason"]);
                                                    destination.MajorAirport = row["MajorAirport"] == DBNull.Value ? null : Convert.ToString(row["MajorAirport"]);
                                                    destination.WhyChoose = row["WhyChoose"] == DBNull.Value ? null : Convert.ToString(row["WhyChoose"]);
                                                    destination.DealGuid = row["DealGuid"] == DBNull.Value ? null : Convert.ToString(row["DealGuid"]);
                                                    destination.DealFrom = row["DealFrom"] == DBNull.Value ? null : Convert.ToString(row["DealFrom"]);
                                                    destination.DealTo = row["DealTo"] == DBNull.Value ? null : Convert.ToString(row["DealTo"]);
                                                    destination.Extra = row["Extra"] == DBNull.Value ? null : Convert.ToString(row["Extra"]);

                                                    response.Destinations.Add(destination);
                                                }
                                            }
                                            break;
                                        case 3: //Theme
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.DealThemeHoliday = new List<DealThemeHoliday>();
                                                DealThemeHoliday dealThemeHoliday = null;
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    dealThemeHoliday = new DealThemeHoliday();
                                                    dealThemeHoliday.Id = Convert.ToInt32(row["Id"]);
                                                    dealThemeHoliday.PortalID = Convert.ToInt32(row["PortalID"]);
                                                    dealThemeHoliday.Title = row["Title"] == DBNull.Value ? null : Convert.ToString(row["Title"]);
                                                    dealThemeHoliday.Description = row["Description"] == DBNull.Value ? null : Convert.ToString(row["Description"]);
                                                    dealThemeHoliday.Keywords = row["Keywords"] == DBNull.Value ? null : Convert.ToString(row["Keywords"]);
                                                    dealThemeHoliday.AltTag = row["AltTag"] == DBNull.Value ? null : Convert.ToString(row["AltTag"]);
                                                    dealThemeHoliday.URL = row["URL"] == DBNull.Value ? null : Convert.ToString(row["URL"]);
                                                    dealThemeHoliday.Body = row["Body"] == DBNull.Value ? null : Convert.ToString(row["Body"]);
                                                    dealThemeHoliday.DealThemeType = Convert.ToInt32(row["DealThemeType"]);
                                                    dealThemeHoliday.Name = row["Name"] == DBNull.Value ? null : Convert.ToString(row["Name"]);
                                                    dealThemeHoliday.DealGuid = row["DealGuid"] == DBNull.Value ? null : Convert.ToString(row["DealGuid"]);
                                                    dealThemeHoliday.DealFrom = row["DealFrom"] == DBNull.Value ? null : Convert.ToString(row["DealFrom"]);
                                                    dealThemeHoliday.DealTo = row["DealTo"] == DBNull.Value ? null : Convert.ToString(row["DealTo"]);
                                                    response.DealThemeHoliday.Add(dealThemeHoliday);
                                                }
                                            }
                                            break;
                                        case 4: //FAQs
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.FAQs = new List<FAQs>();
                                                FAQs daqs = null;
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    daqs = new FAQs();
                                                    daqs.Id = Convert.ToInt32(row["Id"]);
                                                    daqs.PortalId = Convert.ToInt32(row["PortalId"]);
                                                    daqs.Question = row["Question"] == DBNull.Value ? null : Convert.ToString(row["Question"]);
                                                    daqs.Answer = row["Answer"] == DBNull.Value ? null : Convert.ToString(row["Answer"]);
                                                    daqs.AirlineCode = row["AirlineCode"] == DBNull.Value ? null : Convert.ToString(row["AirlineCode"]);
                                                    response.FAQs.Add(daqs);
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Common.Utility.GetPortalData" + ex.ToString());
            }
            return response;
        }

        public static string GetTFNNumber(HttpRequestBase _request)
        {
            string number = Utility.PortalSettings.PortalDetails.DefaultTollFreeNumber;
            CampaignMasters campaign = null;// CamapignInfo.GetCampaign(_request);
            if (campaign != null)
            {
                number = campaign.TollFreeNumber;
            }
            return number;
        }
        public static int GetAffiliateId(HttpRequestBase _request)
        {
            int number = Utility.PortalSettings.PortalId;
            CampaignMasters campaign = null;//CamapignInfo.GetCampaign(_request);
            if (campaign != null)
            {
                number = campaign.AffiliateId;
            }
            return number;
        }
        public static string GetClickedId(HttpRequestBase _request)
        {
            string response = string.Empty;
            CampaignMasters campaign = null;// CamapignInfo.GetCampaign(_request);
            if (campaign != null)
            {
                response = campaign.ClickedId;
            }
            return response;
        }

        public static string IsAnimateBannerListingPage(int affiliate)
        {
            string response = string.Empty;
            try
            {
                if (Utility.PortalSettings.TFNShowHideSetting.IsShowShakeStyle)
                {
                    if (!Utility.Settings.LstBannerAnimationDisable.Contains(affiliate))
                    {
                        response = "toog_animat";
                    }
                }

            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Common.Utility.IsAnimateBannerListingPage" + ex.ToString());
            }
            return response;
        }

        public static AirContext GetMediumMarketContext(string _id)
        {
            AirContext airContext = null;
            try
            {
                var contracts = Utility.MongoInstance.Single<Contracts>(_id);
                if (contracts != null)
                {
                    airContext = new AirContext()
                    {
                        Availability = contracts.Availability,
                        Search = contracts.Search
                    };
                }
            }
            catch (Exception ex)
            {

                Utility.Logger.Error("Common.Utility.GetMediumMarketContext" + ex.ToString());
            }
            return airContext;
        }

        public static BagInsuranc GetBagInsuranceType(int _productId, int _totalPax)
        {
            BagInsuranc response = null;
            try
            {
                //Configration.Product product = Utility.Settings.TravelBagAPI.Products.Product.Where<Configration.Product>(o => o.Id == _productId).FirstOrDefault<Configration.Product>();
                //if (product != null)
                //{
                //    response = new BagInsuranc()
                //    {
                //        BagInsuranceType = (BagInsuranceType)product.Id,
                //        PPaxPrice = (decimal)product.ProductPrice,
                //        TotalPrice = (decimal)(product.ProductPrice * Convert.ToSingle(_totalPax))
                //    };
                //}
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Common.Utility.GetBagInsuranceType:" + ex.ToString());
            }
            return response;
        }

        private static void WriteDataFile<T>(T dataObject, string _file)
        {
            try
            {
                string path = Path.Combine(HttpRuntime.AppDomainAppPath, string.Format("Configration\\" + _file + ".json"));
                File.WriteAllText(path, JsonConvert.SerializeObject(dataObject).ToString());
            }
            catch (Exception ex)
            {
                Utility.Logger.Info(string.Format("WriteDataFile |File:{0}.json Exception:{1}", _file, ex.ToString()));
            }
        }

        public static List<T> ReadDataFile<T>(string _file)
        {
            List<T> fileData = null;
            try
            {
                string fileLocation = Path.Combine(HttpRuntime.AppDomainAppPath, string.Format("Configration\\" + _file + ".json"));
                if (File.Exists(fileLocation))
                    fileData = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(fileLocation));
            }
            catch (Exception ex)
            {
                Utility.Logger.Info(string.Format("ReadDataFile:{0}.json | Exception:{1}", _file, ex.ToString()));
            }
            return fileData;
        }

        public static bool IsHideAffiliateTFN(int _aid)
        {
            bool isHide = false;
            try
            {
                if (_aid > 0 && !string.IsNullOrEmpty(Utility.Settings.HideAffiliateTFN) && Utility.Settings.HideAffiliateTFN.Contains(_aid.ToString()))
                {
                    isHide = true;
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("IsHideAffiliateTFN:{0}.json | Exception:{1}", _aid, ex.ToString()));
            }
            return isHide;
        }
        public static string GetTripType(TripType tripType)
        {
            string response = string.Empty;
            switch (tripType)
            {

                case TripType.ONEWAY:
                    response = "Oneway";
                    break;
                case TripType.MULTICITY:
                    response = "Multicity";
                    break;
                default:
                    response = "Round trip";
                    break;
            }
            return response;
        }

        public static string GetContractClass(Contract contract)
        {
            string response = string.Empty;
            try
            {
                StringBuilder str = new StringBuilder();
                foreach (Segments item in contract.TripDetails.OutBoundSegment)
                {
                    str.Append(item.Class + ",");
                }
                if (contract.TripDetails.InBoundSegment != null && contract.TripDetails.InBoundSegment.Count > 0)
                {
                    foreach (Segments item in contract.TripDetails.InBoundSegment)
                    {
                        str.Append(item.Class + ",");
                    }
                }
                response = str.ToString().Substring(0, str.ToString().Length - 1);
            }
            catch (Exception ex)
            {

                Utility.Logger.Error(string.Format("GetContractClass: | Exception:{0}", ex.ToString()));
            }
            return response;
        }
        public static string GetBasicEconomicAirline(Contract contract)
        {
            string response = string.Empty;
            try
            {
                Segments segments = segments = contract.TripDetails.OutBoundSegment.Where<Segments>(o => o.CabinType == CabinType.BasicEconomy && Utility.Settings.BasicEconomyAirlines.Contains(o.MarketingCarrier.Code)).FirstOrDefault<Segments>(); ;
                if (segments == null && contract.TripType == TripType.ROUNDTRIP)
                {
                    segments = contract.TripDetails.InBoundSegment.Where<Segments>(o => o.CabinType == CabinType.BasicEconomy && Utility.Settings.BasicEconomyAirlines.Contains(o.MarketingCarrier.Code)).FirstOrDefault<Segments>();
                }
                if (segments != null)
                {
                    response = segments.MarketingCarrier.Code;
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("GetBasicEconomicAirline: | Exception:{0}", ex.ToString()));
            }
            return response;
        }
        public static List<DealThemeHoliday> GetThemeHolidayDealsPages(DealThemeType dealThemeType)
        {
            return Utility.PortalData.DealThemeHoliday.Where<DealThemeHoliday>(o => o.DealThemeType == (int)dealThemeType).OrderBy(o => o.Name).ToList<DealThemeHoliday>();

        }

        public static bool IsSearchEngine(string _tfn)
        {
            bool response = false;
            try
            {
                if (!string.IsNullOrEmpty(_tfn))
                {
                    if (Utility.PortalSettings.SearchEngineAffiliate.Contains(_tfn))
                    {
                        response = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("IsSearchEngine: | Exception:{0}", ex.ToString()));
            }
            return response;
        }

        public static bool IsShowMetaSearchDesktop(string _affAffiliateId)
        {
            bool response = false;
            try
            {
                if (!string.IsNullOrEmpty(_affAffiliateId))
                {
                    if (Utility.PortalSettings.MetaSearchShow.DeaktopShow.Contains(_affAffiliateId))
                    {
                        response = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("MetaSearchShowDesktop: | Exception:{0}", ex.ToString()));
            }
            return response;
        }
        public static bool IsShowMetaSearchMobile(string _affAffiliateId)
        {
            bool response = false;
            try
            {
                if (!string.IsNullOrEmpty(_affAffiliateId))
                {
                    if (Utility.PortalSettings.MetaSearchShow.MobileShow.Contains(_affAffiliateId))
                    {
                        response = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("IsShowMetaSearchMobile: | Exception:{0}", ex.ToString()));
            }
            return response;
        }
        public static string NumberToWord(int num)
        {
            string words = "";
            try
            {
                if (num == 0)
                    return "Zero";

                if (num < 0)
                    return "Not supported";

                string[] strones = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                string[] strtens = { "Twenty", "Thirty", "Fourty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };


                int crore = 0, lakhs = 0, thousands = 0, hundreds = 0, tens = 0, single = 0;


                crore = num / 10000000; num = num - crore * 10000000;
                lakhs = num / 100000; num = num - lakhs * 100000;
                thousands = num / 1000; num = num - thousands * 1000;
                hundreds = num / 100; num = num - hundreds * 100;
                if (num > 19)
                {
                    tens = num / 10; num = num - tens * 10;
                }
                single = num;


                if (crore > 0)
                {
                    if (crore > 19)
                        words += NumberToWord(crore) + "Crore ";
                    else
                        words += strones[crore - 1] + " Crore ";
                }

                if (lakhs > 0)
                {
                    if (lakhs > 19)
                        words += NumberToWord(lakhs) + "Lakh ";
                    else
                        words += strones[lakhs - 1] + " Lakh ";
                }

                if (thousands > 0)
                {
                    if (thousands > 19)
                        words += NumberToWord(thousands) + "Thousand ";
                    else
                        words += strones[thousands - 1] + " Thousand ";
                }

                if (hundreds > 0)
                    words += strones[hundreds - 1] + " Hundred ";

                if (tens > 0)
                    words += strtens[tens - 2] + " ";

                if (single > 0)
                    words += strones[single - 1] + " ";
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("NumberToWord: | Exception:{0}", ex.ToString()));
            }
            words = !string.IsNullOrEmpty(words) ? words.Replace(" ", "") : words;
            return words;
        }
        public static string WriteFilePhysicalLocation(string _transactionId, string _content, string _location, string _extension = "txt")
        {
            string DOC = string.Empty;
            try
            {
                string filePath = Path.Combine(_location + DateTime.UtcNow.ToString("ddMMyyyy"));
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                DOC = string.Format("{0}.{1}", _transactionId, _extension);
                if (File.Exists(Path.Combine(filePath, DOC)))
                {
                    File.AppendAllText(Path.Combine(filePath, DOC), string.Format("{0}{1}", _content, System.Environment.NewLine));
                }
                else
                {
                    File.WriteAllText(Path.Combine(filePath, DOC), string.Format("{0}{1}", _content, System.Environment.NewLine));
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Utility.WriteFilePhysicalLocation|Exception:" + ex.ToString());
            }
            return DOC;
        }
        public static void SetEasyPayDetailsCache(string _key, EasyPayDetails _easyPayDetails)
        {
            try
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                DateTime expTime = DateTime.Now.AddMinutes(30);
                policy.AbsoluteExpiration = expTime;
                _easyPayDetails.CacheExpiryTime = expTime;
                Utility.GLobalCache.Set(_key.ToLower(), _easyPayDetails, policy);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Utility.SetEasyPayDetailsCache" + ex.ToString());
            }
        }
        public static void RemoveEasyPayDetailsCache(string _key)
        {
            try
            {
                Utility.GLobalCache.Remove(_key);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Utility.RemoveEasyPayDetailsCache" + ex.ToString());
            }
        }
        public static EasyPayDetails GetEasyPayDetailsCache(string _key)
        {
            EasyPayDetails easyPayDetails = null;
            try
            {
                easyPayDetails = (EasyPayDetails)Utility.GLobalCache.Get(_key);
                if (easyPayDetails != null && easyPayDetails.CacheExpiryTime.AddMinutes(-5) <= DateTime.Now)
                {
                    Task.Factory.StartNew(() => SetEasyPayDetailsCache(_key.ToLower(), easyPayDetails));
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Utility.GetEasyPayDetailsCache" + ex.ToString());
            }
            return easyPayDetails;
        }
        public static decimal GetFareTravelexRQ(TravellerPaxType paxType, Contract _contract)
        {
            decimal response = 0;

            switch (paxType)
            {
                case TravellerPaxType.ADT:
                    response = (decimal)(_contract.AdultFare.BaseFare + _contract.AdultFare.Tax + _contract.AdultFare.Markup + _contract.AdultFare.SupplierFee);
                    break;
                case TravellerPaxType.SEN:
                    response = (decimal)(_contract.SeniorFare.BaseFare + _contract.SeniorFare.Tax + _contract.SeniorFare.Markup + _contract.SeniorFare.SupplierFee);
                    break;
                case TravellerPaxType.CHD:
                    response = (decimal)(_contract.ChildFare.BaseFare + _contract.ChildFare.Tax + _contract.ChildFare.Markup + _contract.ChildFare.SupplierFee);
                    break;
                case TravellerPaxType.INS:
                    response = (decimal)(_contract.InfantOnSeatFare.BaseFare + _contract.InfantOnSeatFare.Tax + _contract.InfantOnSeatFare.Markup + _contract.InfantOnSeatFare.SupplierFee);
                    break;
                case TravellerPaxType.INL:
                    response = (decimal)(_contract.InfantOnLapFare.BaseFare + _contract.InfantOnLapFare.Tax + _contract.InfantOnLapFare.Markup + _contract.InfantOnLapFare.SupplierFee);
                    break;
            }
            return response;
        }
        public static bool IsRedirected(HttpRequestBase _request)
        {
            bool response = false;
            try
            {

                if (_request != null && _request.Headers != null
                    && (string.IsNullOrEmpty(_request.Headers["Referer"])
                    || (!string.IsNullOrEmpty(_request.Headers["Referer"]) && (_request.Headers["Referer"].ToString().Equals("https://wizfairtickets.com", StringComparison.OrdinalIgnoreCase) || _request.Headers["Referer"].ToString().Equals("http://wizfairtickets.com", StringComparison.OrdinalIgnoreCase)))
                    ))
                {
                    response = true;
                }

            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Utility.IsRedirected" + ex.ToString());
            }
            return response;
        }

        public static float GetExtendedCancelPricePerPax(Contract _contract)
        {
            float response = 0;
            try
            {
                if (_contract != null)
                {
                    float totalBaseTaxPrice = _contract.TotalBaseFare + _contract.TotalTax;
                    response = totalBaseTaxPrice > Utility.Settings.TravelExtendedCancellation.MinBookingAmount ? Utility.Settings.TravelExtendedCancellation.MaxAmount : Utility.Settings.TravelExtendedCancellation.MinAmount;
                }
                else
                {
                    response = Utility.Settings.TravelExtendedCancellation.MinAmount;
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Utility.GetExtendedCancelPricePerPax" + ex.ToString());
            }
            return response;
        }

        #region Coupon Business
        public static PromoCodeResponse GetPromocodeDetails(PromoCodeRequest _promoRequest, Contract _selectedContract)
        {
            PromoCodeResponse response = null;
            try
            {
                if (Utility.CouponData != null && Utility.CouponData.Count > 0)
                {
                    CouponData couponData = Utility.CouponData.Where(o => o.CouponCode.ToUpper().Equals(_promoRequest.CouponCode.ToUpper())).FirstOrDefault();
                    float totalMarkup = _selectedContract.TotalMarkup;
                    if (couponData != null)
                    {
                        float discount = 0;
                        float totalAmount = _selectedContract.TotalBaseFare + _selectedContract.TotalTax;
                        if (couponData.DiscountType == DiscountType.Amount)
                        {
                            if (totalMarkup > Convert.ToSingle(couponData.Amount))
                            {
                                discount = Convert.ToSingle(couponData.Amount);
                            }
                            else
                            {
                                discount = totalMarkup;
                            }
                        }
                        else if (couponData.DiscountType == DiscountType.Percentage)
                        {
                            float amount = (totalAmount * Convert.ToSingle(couponData.Percentage)) / 100;
                            if (totalMarkup > amount)
                            {
                                discount = amount;
                            }
                            else
                            {
                                discount = totalMarkup;
                            }
                        }
                        response = new PromoCodeResponse() { Result = discount, Status = new Statuspromo() { IsSuccess = true } };

                    }
                    else
                    {
                        response = new PromoCodeResponse() { Result = 0, Status = new Statuspromo() { IsSuccess = false, Error = new Error() { Description = "This coupon is not available." } } };
                    }
                }
                else
                {
                    response = new PromoCodeResponse() { Result = 0, Status = new Statuspromo() { IsSuccess = false, Error = new Error() { Description = "This coupon is not available." } } };
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Utility.GetPromocodeDetails|Exception:" + ex.ToString());
                response = new PromoCodeResponse() { Result = 0, Status = new Statuspromo() { IsSuccess = false, Error = new Error() { Description = "This coupon is not available." } } };
            }
            return response;

        }
        #endregion
        public static float GetActualMinFare(string _giud)
        {
            float response = 0;
            try
            {
                if (Utility.IsValidGuid(_giud))
                {
                    AirContext context = Utility.GetAirContextCache(_giud);
                    if (context != null && context.Availability != null && context.Availability.Contracts != null && context.Availability.Contracts.Count>0)
                    {
                        response = Utility.GetMinimumPrice(context.Availability.Contracts, ContractType.Actual);
                    }

                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Utility.GetActualMinFare|Exception:" + ex.ToString());
            }

            return response;
        }
        
    }
}
