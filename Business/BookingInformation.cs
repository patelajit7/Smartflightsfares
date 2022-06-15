using Database;
using Common;
using Infrastructure;
using Infrastructure.HelpingModel;
using Infrastructure.HelpingModel.API;
using Infrastructure.HelpingModel.BookingEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class BookingInformation
    {
        public static BookingDetails GetBookingDetails(int bookingId)
        {
            BookingDetails response = null;
            try
            {
                response = BookingProcedures.GetBookingDetails(bookingId, "", Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("GetBookingDetails UNABLE  SAVE IN DATABASE|EXCEPTION:" + ex.ToString());
            }
            return response;
        }
        public static BookingDetails GetBookingDetailsGuid(string guid)
        {
            BookingDetails response = null;
            try
            {
                response = BookingProcedures.GetBookingDetails(0, guid, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("GetBookingDetails UNABLE  SAVE IN DATABASE|EXCEPTION:" + ex.ToString());
            }
            return response;
        }

        public static bool SaveFlightSearches(bool isSaveForcelly=true)
        {
            bool isSave = false;
            try
            {
                if ((isSaveForcelly && Utility.FlightSearches.Count>0) || (!isSaveForcelly && Utility.FlightSearches.Count > 3))
                {
                    List<FlightSearch> searches = Utility.FlightSearches;
                    Utility.FlightSearches = new List<FlightSearch>();
                    DataTable flighSearchTbl = PrepFlightSearchTbl(searches);
                    isSave = BookingProcedures.IsSaveFlightSearches(flighSearchTbl, Utility.ConnString);
                    if (!isSave)
                    {
                        Utility.FlightSearches.AddRange(searches);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Business.SaveFlightSearchDetails()|EXCEPTION:" + ex.ToString());
            }
            return isSave;
        }
        private static DataTable PrepFlightSearchTbl(List<FlightSearch> _flightSearchDetails)
        {
            DataTable flightSearch = new DataTable();
            try
            {
                flightSearch.Columns.Add("GuidId", typeof(string));
                flightSearch.Columns.Add("PortalId", typeof(int)).DefaultValue = Utility.PortalSettings.PortalId;
                flightSearch.Columns.Add("AffiliateId", typeof(int)).DefaultValue = Utility.PortalSettings.PortalId;
                flightSearch.Columns.Add("TripType", typeof(int)).DefaultValue = (int)TripType.ONEWAY;
                flightSearch.Columns.Add("Origin", typeof(string));
                flightSearch.Columns.Add("Destination", typeof(string));
                flightSearch.Columns.Add("Departure", typeof(DateTime));
                flightSearch.Columns.Add("Return", typeof(DateTime));
                flightSearch.Columns.Add("Adult", typeof(int));
                flightSearch.Columns.Add("Senior", typeof(int));
                flightSearch.Columns.Add("Child", typeof(int));
                flightSearch.Columns.Add("InfantOnSeat", typeof(int));
                flightSearch.Columns.Add("InfantOnLap", typeof(int));
                flightSearch.Columns.Add("Cabin", typeof(int)).DefaultValue = (int)CabinType.EconomyCoach;
                flightSearch.Columns.Add("PreferredCarrier", typeof(string));
                flightSearch.Columns.Add("IsDirectFlight", typeof(bool));
                flightSearch.Columns.Add("IP", typeof(string));
                flightSearch.Columns.Add("UtmSource", typeof(string));
                flightSearch.Columns.Add("UtmMedium", typeof(string));
                flightSearch.Columns.Add("UtmCampaign", typeof(string));
                flightSearch.Columns.Add("Created", typeof(DateTime));
                PopulateFlightSearch(_flightSearchDetails, ref flightSearch);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Business.FlightSearchTbl()|EXception:", ex.ToString());
            }

            return flightSearch;
        }
        private static void PopulateFlightSearch(List<FlightSearch> _flightSearchDetails, ref DataTable flightSearch)
        {
            try
            {
                if (_flightSearchDetails != null && _flightSearchDetails.Count > 0)
                {
                    DataRow dataRow;
                    foreach (FlightSearch item in _flightSearchDetails)
                    {
                        dataRow = flightSearch.NewRow();
                        dataRow["GuidId"] = item.SearchGuidId;
                        dataRow["PortalId"] = item.PortalId;
                        dataRow["AffiliateId"] = item.AffiliateId;
                        dataRow["TripType"] = (int)item.TripType;
                        dataRow["Origin"] = item.Origin;
                        dataRow["Destination"] = item.Destination;
                        dataRow["Departure"] = item.Departure;
                        dataRow["Return"] = item.TripType == TripType.ROUNDTRIP ? item.Return ?? item.Departure : item.Departure;
                        dataRow["Adult"] = item.Adult;
                        dataRow["Senior"] = item.Senior;
                        dataRow["Child"] = item.Child;
                        dataRow["InfantOnSeat"] = item.InfantOnSeat;
                        dataRow["InfantOnLap"] = item.InfantOnLap;
                        dataRow["Cabin"] = (int)item.Cabin;
                        dataRow["PreferredCarrier"] = string.IsNullOrEmpty(item.PreferredCarrier) ? string.Empty : item.PreferredCarrier;
                        dataRow["IsDirectFlight"] = item.IsDirectFlight;
                        dataRow["IP"] = !string.IsNullOrEmpty(item.IP) ? (item.IP.Length < 20 ? item.IP : item.IP.Substring(0, 15)) : string.Empty;
                        dataRow["UtmSource"] = item.UtmSource;
                        dataRow["UtmMedium"] = item.UtmMedium;
                        dataRow["UtmCampaign"] = item.UtmCampaign;
                        dataRow["Created"] = item.SearchDateTime;
                        flightSearch.Rows.Add(dataRow);
                        flightSearch.AcceptChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Business.PopulateFlightSearch()|EXception:", ex.ToString());
            }
        }
        public static void SaveCampaignTrackings(BookingCampaignTracking _tracking)
        {
            try
            {
                BookingProcedures.SaveCampaignTrackings(_tracking,Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Business.SaveCampaignTrackings()|EXception:", ex.ToString());
            }
        }
        public static BookingDetails GetDocuSignBookingDetails(int bookingId , int cardId)
        {
            BookingDetails response = null;
            try
            {
                response = BookingProcedures.GetDocuSignBookingDetails(bookingId, cardId, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("GetDocuSignBookingDetails UNABLE  SAVE IN DATABASE|EXCEPTION:" + ex.ToString());
            }
            return response;
        }
        public static int SaveEasyPay(EasyPayDetails _easypay)
        {
            int Result = 0;
            try
            {
                Result = BookingProcedures.SaveEasyPay(_easypay, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Business.SaveEasyPay()|EXception:{0}", ex.ToString()));
            }
            return Result;
        }
        public static void UpdateEasyPay(EasyPayDetails _easypay)
        {            
            try
            {
                BookingProcedures.UpdateEasyPay(_easypay, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Business.UpdateEasyPay()|EXception:{0}", ex.ToString()));
            }            
        }
        public static EasyPayDetails GetEasyPay(int Id)
        {
            EasyPayDetails easyPayDetails = null;
            try
            {
                easyPayDetails = BookingProcedures.GetEasyPay(Id, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Business.GetEasyPay()|EXception:{0}", ex.ToString()));
            }
            return easyPayDetails;
        }        
        public static void AddBookingFailureDatail(BookingFailureDetails _bookingFailureDetails)
        {
            try
            {
                BookingProcedures.AddBookingFailureDatail(_bookingFailureDetails, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Business.AddBookingFailureDatail()|EXception:{0}", ex.ToString()));
            }
        }
        public static void UpdateBookingFailureDatail(string _guid, string _action)
        {
            try
            {
                BookingProcedures.UpdateBookingFailureDatail(_guid, _action, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error(string.Format("Business.UpdateBookingFailureDatail()|EXception:{0}", ex.ToString()));
            }
        }
    }
}
