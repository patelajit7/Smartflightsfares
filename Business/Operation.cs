using System;
using System.Collections.Generic;
using System.Data;
using Database;
using Common;
using Infrastructure;
using Infrastructure.HelpingModel.API;
using Infrastructure.HelpingModel.Operations;
using Infrastructure.HelpingModel;

namespace Business
{
    public class Operation
    {
        private static object syncPoolRoot = new Object();
        private static object syncPoolRoot1 = new Object();
        public static DocuSignsVM GetDocuSigns(int _id, int _filterType, int _docuSignVM)
        {
            DocuSignsVM response = null;
            try
            {
                response = OptProcedure.GetDocuSign(_id, _filterType, _docuSignVM, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("GetDocuSign UNABLE  SAVE IN DATABASE|EXCEPTION:" + ex.ToString());
            }
            return response;
        }
        public static bool DocuAccept(int _id, int _status, string _ip)
        {
            bool isAuth = false;
            try
            {
                isAuth = OptProcedure.DocuSignsAccepted(_id, _status, _ip, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("UNABLE TO UPDATE IN DATABASE|EXCEPTION:" + ex.ToString());
            }
            return isAuth;
        }
        public static void SaveNewsLetterDetails(string email, int _subscriptiontype)
        {
            try
            {
                BookingProcedures.SaveNewsLetter(email, _subscriptiontype, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("NEWSLETTER UNABLE  SAVE IN DATABASE|EXCEPTION:" + ex.ToString());
            }
        }
        public static void SaveContactSegmentDatails(ContactDetail contactDetails, Contract contract)
        {
            Dictionary<string, DataTable> contactSegmentTbl = null;
            try
            {
                contactSegmentTbl = new Dictionary<string, DataTable>();
                contactSegmentTbl.Add("tblContacts", ContactDetailsDataTbl(contactDetails));
                contactSegmentTbl.Add("tblSegments", FlightSegmentsDataTbl(contract));
                OptProcedure.SaveContactSegmentDatails(contactSegmentTbl, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("SaveContactData UNABLE TO UPDATE IN DATABASE|EXCEPTION:" + ex.ToString());
            }
        }
        public static void SaveOfferContactSegmentDatails(ContactDetail contactDetails, Contract contract)
        {
            Dictionary<string, DataTable> contactSegmentTbl = null;
            try
            {
                contactSegmentTbl = new Dictionary<string, DataTable>();
                contactSegmentTbl.Add("tblContacts", ContactDetailsDataTbl(contactDetails));
                contactSegmentTbl.Add("tblSegments", FlightSegmentsDataTbl(contract));
                OptProcedure.SaveOfferContactSegmentDatails(contactSegmentTbl, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("SaveContactData UNABLE TO UPDATE IN DATABASE|EXCEPTION:" + ex.ToString());
            }
        }
        private static DataTable FlightSegmentsDataTbl(Contract contract)
        {
            DataTable flightSegments = new DataTable();
            try
            {
                flightSegments.Columns.Add("SegmentOrder", typeof(int));
                flightSegments.Columns.Add("IsReturn", typeof(bool));
                flightSegments.Columns.Add("FlightNumber", typeof(string));
                flightSegments.Columns.Add("OptAirlineCode", typeof(string));
                flightSegments.Columns.Add("MktAirlineCode", typeof(string));
                flightSegments.Columns.Add("OriginCode", typeof(string));
                flightSegments.Columns.Add("DeptDateTime", typeof(DateTime));
                flightSegments.Columns.Add("DeptTerminal", typeof(string));
                flightSegments.Columns.Add("DestinationCode", typeof(string));
                flightSegments.Columns.Add("ArrivalDateTime", typeof(DateTime));
                flightSegments.Columns.Add("ArrivalTerminal", typeof(string));
                flightSegments.Columns.Add("EquipmentDetail", typeof(string));
                flightSegments.Columns.Add("SegmentClass", typeof(string));
                flightSegments.Columns.Add("Stops", typeof(int));
                flightSegments.Columns.Add("Cabin", typeof(int));
                flightSegments.Columns.Add("CompanyFranchiseDetails", typeof(string));
                flightSegments.Columns.Add("TechnicalStoppages", typeof(string));
                flightSegments.Columns.Add("AirlineLocator", typeof(string));
                flightSegments.Columns.Add("SegmentType", typeof(string));
                if (contract != null)
                {
                    PopulateFlightSegments(contract, ref flightSegments);
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.FlightsDataTbl|EXception:", ex.ToString());
            }
            return flightSegments;
        }
        private static DataTable ContactDetailsDataTbl(ContactDetail contact)
        {
            DataTable contactDetails = new DataTable();
            try
            {
                contactDetails.Columns.Add("Guid", typeof(string));
                contactDetails.Columns.Add("CountryCode", typeof(string));
                contactDetails.Columns.Add("AreaCode", typeof(string));
                contactDetails.Columns.Add("Phone", typeof(string));
                contactDetails.Columns.Add("Email", typeof(string));
                contactDetails.Columns.Add("Price", typeof(float));
                contactDetails.Columns.Add("Markup", typeof(float));
                contactDetails.Columns.Add("Status", typeof(int));
                contactDetails.Columns.Add("IP", typeof(string));
                contactDetails.Columns.Add("TripType", typeof(int));
                contactDetails.Columns.Add("DepartureDate", typeof(DateTime));
                contactDetails.Columns.Add("ReturnDate", typeof(DateTime));
                contactDetails.Columns.Add("From", typeof(string));
                contactDetails.Columns.Add("To", typeof(string));
                contactDetails.Columns.Add("PortalId", typeof(int));
                contactDetails.Columns.Add("AffiliateId", typeof(int));
                contactDetails.Columns.Add("IsMobile", typeof(int));
                PopulateContctDetails(contact, ref contactDetails);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.FlightsDataTbl|EXception:", ex.ToString());
            }
            return contactDetails;
        }
        private static void PopulateFlightSegments(Contract contract, ref DataTable flightSegments)
        {

            try
            {
                DataRow dataRow;
                int i = 0;
                foreach (Segments item in contract.TripDetails.OutBoundSegment)
                {
                    dataRow = flightSegments.NewRow();
                    dataRow["SegmentOrder"] = i;
                    dataRow["IsReturn"] = false;
                    dataRow["FlightNumber"] = item.FlightNumber;
                    dataRow["OptAirlineCode"] = item.OperatingCarrier != null ? item.OperatingCarrier.Code : item.MarketingCarrier.Code;
                    dataRow["MktAirlineCode"] = item.MarketingCarrier.Code;
                    dataRow["OriginCode"] = item.Origin;
                    dataRow["DeptDateTime"] = new DateTime(item.Departure.Year, item.Departure.Month, item.Departure.Day).Add(item.DepartureTime);
                    dataRow["DeptTerminal"] = item.OutTerminal;
                    dataRow["DestinationCode"] = item.Destination;
                    dataRow["ArrivalDateTime"] = new DateTime(item.Arrival.Year, item.Arrival.Month, item.Arrival.Day).Add(item.ArrivalTime);
                    dataRow["ArrivalTerminal"] = item.InTerminal;
                    dataRow["EquipmentDetail"] = item.EquipmentType;
                    dataRow["SegmentClass"] = item.Class;
                    dataRow["Stops"] = item.NoOfStops;
                    dataRow["Cabin"] = (int)item.CabinType;
                    dataRow["CompanyFranchiseDetails"] = item.CompanyFranchiseDetails;
                    dataRow["TechnicalStoppages"] = "";
                    dataRow["AirlineLocator"] = item.AirlineLocator;
                    dataRow["SegmentType"] = "";
                    flightSegments.Rows.Add(dataRow);
                    flightSegments.AcceptChanges();
                    i++;
                }
                if (contract.TripType == TripType.ROUNDTRIP && contract.TripDetails.InBoundSegment != null && contract.TripDetails.InBoundSegment.Count > 0)
                {
                    foreach (Segments item in contract.TripDetails.InBoundSegment)
                    {
                        dataRow = flightSegments.NewRow();
                        dataRow["SegmentOrder"] = i;
                        dataRow["IsReturn"] = true;
                        dataRow["FlightNumber"] = item.FlightNumber;
                        dataRow["OptAirlineCode"] = item.OperatingCarrier != null ? item.OperatingCarrier.Code : item.MarketingCarrier.Code;
                        dataRow["MktAirlineCode"] = item.MarketingCarrier.Code;
                        dataRow["OriginCode"] = item.Origin;
                        dataRow["DeptDateTime"] = new DateTime(item.Departure.Year, item.Departure.Month, item.Departure.Day).Add(item.DepartureTime);
                        dataRow["DeptTerminal"] = item.OutTerminal;
                        dataRow["DestinationCode"] = item.Destination;
                        dataRow["ArrivalDateTime"] = new DateTime(item.Arrival.Year, item.Arrival.Month, item.Arrival.Day).Add(item.ArrivalTime);
                        dataRow["ArrivalTerminal"] = item.InTerminal;
                        dataRow["EquipmentDetail"] = item.EquipmentType;
                        dataRow["SegmentClass"] = item.Class;
                        dataRow["Stops"] = item.NoOfStops;
                        dataRow["Cabin"] = (int)item.CabinType;
                        dataRow["CompanyFranchiseDetails"] = item.CompanyFranchiseDetails;
                        dataRow["TechnicalStoppages"] = "";
                        dataRow["AirlineLocator"] = item.AirlineLocator;
                        dataRow["SegmentType"] = "";
                        flightSegments.Rows.Add(dataRow);
                        flightSegments.AcceptChanges();
                        i++;
                    }
                }

            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.PopulateFlightSegments|EXception:", ex.ToString());
            }
        }
        private static void PopulateContctDetails(ContactDetail item, ref DataTable tblcontact)
        {
            try
            {
                DataRow dataRow;

                dataRow = tblcontact.NewRow();
                dataRow["Guid"] = item.Guid;
                dataRow["CountryCode"] = item.CountryCode;
                dataRow["AreaCode"] = item.AreaCode;
                dataRow["Phone"] = item.Phone;
                dataRow["Email"] = item.Email;
                dataRow["Price"] = item.Price;
                dataRow["Markup"] = item.Markup;
                dataRow["Status"] = item.Status;
                dataRow["IP"] = item.IP;
                dataRow["TripType"] = item.TripType;
                dataRow["DepartureDate"] = item.DepartureDate;
                dataRow["ReturnDate"] = item.ReturnDate == null ? (object)DBNull.Value : item.ReturnDate;
                dataRow["From"] = item.From;
                dataRow["To"] = item.To;
                dataRow["PortalId"] = item.PortalId;
                dataRow["AffiliateId"] = item.AffiliateId;
                dataRow["IsMobile"] = item.IsMobile;
                tblcontact.Rows.Add(dataRow);
                tblcontact.AcceptChanges();

            }
            catch (Exception ex)
            {
                Utility.Logger.Error("EasyPro.Business.PopulateContctDetails|EXception:", ex.ToString());
            }
        }

        public static void SentItineryDetails(RequestedItinerary request)
        {
            try
            {
                BookingProcedures.SentItineryDetails(request, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("SentItineryDetails UNABLE SAVE IN DATABASE|EXCEPTION:" + ex.ToString());
            }
        }

        public static void MetaClicks(MetaClicks request, string Guid)
        {
            try
            {
                lock (syncPoolRoot)
                {
                   int MetaclikId =  BookingProcedures.MetaClicks(request, Utility.ConnString);

                    AirContext context = Utility.GetAirContextCache(Guid);
                    if (context != null)
                    {
                        context.MetaClickId = MetaclikId;
                        Utility.SetAirContextCache(Guid, context);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("MetaClicks UNABLE SAVE IN DATABASE|EXCEPTION:" + ex.ToString());
            }
        }
        public static void ACallRequested(RequestedItinerary request)
        {
            try
            {
                BookingProcedures.RequestACallDetails(request, Utility.ConnString);
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("RequestACallDetails UNABLE SAVE IN DATABASE|EXCEPTION:" + ex.ToString());
            }
        }
        public static void UpdateBookingUserLocation(int _bookingId, string _location)
        {
            try
            {
                lock (syncPoolRoot1)
                {
                    BookingProcedures.UpdateBookingUserLocation(_bookingId, _location, Utility.ConnString);
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Booking User Location unable to save in database|EXCEPTION:" + ex.ToString());
            }
        }
        public static void UpdateBookingIsMeta(int bookingId, int metaClickId, bool isMobile)
        {
            try
            {
                lock (syncPoolRoot1)
                {
                    BookingProcedures.UpdateBookingIsMeta(bookingId, metaClickId, isMobile, Utility.ConnString);
                }
            }
            catch (Exception ex)
            {
                Utility.Logger.Error("Booking IsMeta to save in database|EXCEPTION:" + ex.ToString());
            }
        }
    }
}
