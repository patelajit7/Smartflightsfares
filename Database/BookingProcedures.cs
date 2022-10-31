using Infrastructure;
using Infrastructure.HelpingModel;
using Infrastructure.HelpingModel.API;
using Infrastructure.HelpingModel.BookingEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class BookingProcedures
    {
        public static BookingDetails GetBookingDetails(int bookingId, string guid, string connectionString)
        {
            BookingDetails response = null;
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
                        command.CommandText = "usp_GetBookingDetails";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@bookingId", bookingId);
                        command.Parameters.AddWithValue("@guid", guid);
                        sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(ds);
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            response = new BookingDetails();

                            for (int i = 0; i <= ds.Tables.Count - 1; i++)
                            {
                                if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                {
                                    switch (i)
                                    {
                                        case 0://Bookings
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.Transaction = new Transactions();
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    response.Transaction.Id = Convert.ToInt32(row["Id"]);
                                                    response.Transaction.PNR = row["PNR"] == DBNull.Value ? string.Empty : Convert.ToString(row["PNR"]);
                                                    response.Transaction.ReferenceNumber = row["ReferenceNumber"] == DBNull.Value ? string.Empty : Convert.ToString(row["ReferenceNumber"]);
                                                    response.Transaction.PortalId = Convert.ToInt32(row["PortalId"]);
                                                    response.Transaction.GDS = Convert.ToInt32(row["GDS"]);
                                                    response.Transaction.ProviderId = Convert.ToInt32(row["ProviderId"]);
                                                    response.Transaction.BookingType = Convert.ToInt32(row["BookingType"]);
                                                    response.Transaction.BookingSourceType = Convert.ToInt32(row["BookingSourceType"]);
                                                    response.Transaction.BookingStatus = Convert.ToInt32(row["BookingStatus"]);
                                                    response.Transaction.BookingSubStatus = Convert.ToInt32(row["BookingSubStatus"]);
                                                    response.Transaction.AgentId = Convert.ToInt32(row["AgentId"]);
                                                    response.Transaction.AgentLead = Convert.ToInt32(row["AgentLead"]);
                                                    response.Transaction.UserId = Convert.ToInt32(row["UserId"]);
                                                    response.Transaction.BookedOn = DateTime.SpecifyKind(Convert.ToDateTime(row["Created"]), DateTimeKind.Utc);
                                                    response.Transaction.CurrencyCode = row["CurrencyCode"] == DBNull.Value ? "USD" : Convert.ToString(row["CurrencyCode"]);
                                                    response.Transaction.CurrencyPrice = row["CurrencyConversion"] == DBNull.Value ? 1 : Convert.ToDecimal(row["CurrencyConversion"]);
                                                    response.Transaction.IsFailedBooking = row["IsFailedBooking"] == DBNull.Value ? false : Convert.ToBoolean(row["IsFailedBooking"]);
                                                    response.Transaction.BookingFailedErrorMessage = row["BookingFailedErrorMessage"] == DBNull.Value ? string.Empty : Convert.ToString(row["BookingFailedErrorMessage"]);
                                                    break;
                                                }
                                            }
                                            break;
                                        case 1: //Flights
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.Flight = new Flights();
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    response.Flight.TransactionId = Convert.ToInt32(row["BookingId"]);
                                                    response.Flight.Origin = row["OriginCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["OriginCode"]);
                                                    response.Flight.Destination = row["DestinationCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["DestinationCode"]);
                                                    response.Flight.Airline = row["ValAirlineCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["ValAirlineCode"]);
                                                    response.Flight.TripType = Convert.ToInt32(row["TripType"]);
                                                    response.Flight.DeptDate = Convert.ToDateTime(row["DeptDate"]);
                                                    response.Flight.ReturnDate = row["ReturnDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ReturnDate"]);
                                                    response.Flight.TotalPaxCount = Convert.ToInt32(row["PaxCount"]);
                                                    response.Flight.AdultCount = Convert.ToInt32(row["AdultCount"]);
                                                    response.Flight.SeniorCount = Convert.ToInt32(row["SeniorCount"]);
                                                    response.Flight.ChildCount = Convert.ToInt32(row["ChildCount"]);
                                                    response.Flight.InfantCount = Convert.ToInt32(row["InfantOnSeatCount"]);
                                                    response.Flight.InfantLapCount = Convert.ToInt32(row["InfantLapCount"]);
                                                    response.Flight.OutBoundFlightDuration = Convert.ToInt64(row["OutBoundFlightDuration"]);
                                                    response.Flight.InBoundFlightDuration = Convert.ToInt64(row["InBoundFlightDuration"]);
                                                    response.Flight.IsDomestic = Convert.ToBoolean(row["IsDomestic"]);
                                                    response.Flight.FareType = row["FareType"] == DBNull.Value ? "" : Convert.ToString(row["FareType"]);
                                                    response.Flight.ContractType = Convert.ToInt32(row["ContractType"]);
                                                    break;
                                                }
                                            }
                                            break;
                                        case 2: //FlightSegments
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.Segments = new List<FlightSegments>();
                                                FlightSegments segment = null;
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    segment = new FlightSegments();
                                                    segment.TransactionId = Convert.ToInt32(row["BookingId"]);
                                                    segment.SegmentOrder = Convert.ToInt32(row["SegmentOrder"]);
                                                    segment.IsReturn = Convert.ToBoolean(row["IsReturn"]);
                                                    segment.FlightNumber = row["FlightNumber"] == DBNull.Value ? string.Empty : Convert.ToString(row["FlightNumber"]);
                                                    segment.MarketingCode = row["MktAirlineCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["MktAirlineCode"]);
                                                    segment.OperatingCode = row["OptAirlineCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["OptAirlineCode"]);
                                                    segment.Origin = row["OriginCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["OriginCode"]);
                                                    segment.DeptDateTime = Convert.ToDateTime(row["DeptDateTime"]);
                                                    segment.DeptTerminal = row["DeptTerminal"] == DBNull.Value ? string.Empty : Convert.ToString(row["DeptTerminal"]);
                                                    segment.Destination = row["DestinationCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["DestinationCode"]);
                                                    segment.ArrivalDateTime = Convert.ToDateTime(row["ArrivalDateTime"]);
                                                    segment.DeptTerminal = row["ArrivalTerminal"] == DBNull.Value ? string.Empty : Convert.ToString(row["ArrivalTerminal"]);
                                                    segment.EquipmentDetail = row["EquipmentDetail"] == DBNull.Value ? string.Empty : Convert.ToString(row["EquipmentDetail"]);
                                                    segment.SegmentClass = row["SegmentClass"] == DBNull.Value ? string.Empty : Convert.ToString(row["SegmentClass"]);
                                                    segment.Stops = Convert.ToInt32(row["Stops"]);
                                                    segment.Cabin = Convert.ToInt32(row["Cabin"]);
                                                    segment.CompanyFranchiseDetails = row["CompanyFranchiseDetails"] == DBNull.Value ? string.Empty : Convert.ToString(row["CompanyFranchiseDetails"]);
                                                    segment.TechnicalStoppages = row["TechnicalStoppages"] == DBNull.Value ? string.Empty : Convert.ToString(row["TechnicalStoppages"]);
                                                    segment.AirlineLocator = row["AirlineLocator"] == DBNull.Value ? string.Empty : Convert.ToString(row["AirlineLocator"]);
                                                    response.Segments.Add(segment);
                                                }
                                            }
                                            break;
                                        case 3: //Flight Price Details
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.PriceDetail = new List<PriceDetails>();
                                                PriceDetails priceDetail = null;
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    priceDetail = new PriceDetails();
                                                    priceDetail.TransactionId = Convert.ToInt32(row["BookingId"]);
                                                    priceDetail.FareBaseCode = row["FareBaseCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["FareBaseCode"]);
                                                    priceDetail.PaxType = Convert.ToInt32(row["PaxType"]);
                                                    priceDetail.Currency = Convert.ToInt32(row["Currency"]);
                                                    priceDetail.PaxCount = Convert.ToInt32(row["PaxCount"]);
                                                    priceDetail.BaseFare = Convert.ToDecimal(row["BaseFare"]);
                                                    priceDetail.Tax = Convert.ToDecimal(row["Tax"]);
                                                    priceDetail.Markup = Convert.ToDecimal(row["Markup"]);
                                                    priceDetail.SupplierFee = Convert.ToDecimal(row["SupplierFee"]);
                                                    priceDetail.Discount = Convert.ToDecimal(row["Discount"]);
                                                    priceDetail.IsSellInsurance = Convert.ToBoolean(row["IsSellInsurance"]);
                                                    priceDetail.InsuranceAmount = Convert.ToDecimal(row["InsuranceAmount"]);
                                                    priceDetail.TotalAmount = Convert.ToDecimal(row["TotalAmount"]);
                                                    priceDetail.IsSellBaggageInsurance = Convert.ToBoolean(row["IsSellBaggageInsurance"]);
                                                    priceDetail.BaggageInsuranceAmount = Convert.ToDecimal(row["BaggageInsuranceAmount"]);
                                                    priceDetail.IsExtendedCancellation = Convert.ToBoolean(row["IsExtendedCancellation"]);
                                                    priceDetail.ExtendedCancellationAmount = Convert.ToDecimal(row["ExtendedCancellationAmount"]);
                                                    priceDetail.BookingFee = Convert.ToDecimal(row["BookingFee"]);
                                                    response.PriceDetail.Add(priceDetail);
                                                }
                                            }
                                            break;
                                        case 4: //BillingDetails
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.BillingDetails = new BillingDetails();
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    response.BillingDetails.TransactionId = Convert.ToInt32(row["BookingId"]);
                                                    response.BillingDetails.CCHolderName = row["CCHolderName"] == DBNull.Value ? string.Empty : Convert.ToString(row["CCHolderName"]);
                                                    response.BillingDetails.CardNumber = row["CardNumber"] == DBNull.Value ? string.Empty : Convert.ToString(row["CardNumber"]);
                                                    response.BillingDetails.CVVNumber = row["CVVNumber"] == DBNull.Value ? string.Empty : Convert.ToString(row["CVVNumber"]);


                                                    response.BillingDetails.ExpiryYear = Convert.ToInt32(row["ExpiryYear"]);
                                                    response.BillingDetails.ExpiryMonth = Convert.ToInt32(row["ExpiryMonth"]);
                                                    response.BillingDetails.CardType = Convert.ToInt32(row["CardType"]);
                                                    response.BillingDetails.Email = row["Email"] == DBNull.Value ? string.Empty : Convert.ToString(row["Email"]);
                                                    response.BillingDetails.Country = row["Country"] == DBNull.Value ? string.Empty : Convert.ToString(row["Country"]);
                                                    response.BillingDetails.State = row["State"] == DBNull.Value ? string.Empty : Convert.ToString(row["State"]);
                                                    response.BillingDetails.ZipCode = row["ZipCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["ZipCode"]);
                                                    response.BillingDetails.AddressLine1 = row["AddressLine1"] == DBNull.Value ? string.Empty : Convert.ToString(row["AddressLine1"]);
                                                    response.BillingDetails.AddressLine2 = row["AddressLine2"] == DBNull.Value ? string.Empty : Convert.ToString(row["AddressLine2"]);
                                                    response.BillingDetails.AddressLine3 = row["AddressLine3"] == DBNull.Value ? string.Empty : Convert.ToString(row["AddressLine3"]);
                                                    response.BillingDetails.City = row["City"] == DBNull.Value ? string.Empty : Convert.ToString(row["City"]);
                                                    response.BillingDetails.BillingPhone = row["BillingPhone"] == DBNull.Value ? string.Empty : Convert.ToString(row["BillingPhone"]);
                                                    response.BillingDetails.ContactPhone = row["ContactPhone"] == DBNull.Value ? string.Empty : Convert.ToString(row["ContactPhone"]);
                                                    response.BillingDetails.IsPrimaryCard = Convert.ToBoolean(row["IsPrimaryCard"]);
                                                    break;
                                                }
                                            }
                                            break;
                                        case 5: //Traveller Details
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.Travellers = new List<Travellers>();
                                                Travellers traveller = null;
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    traveller = new Travellers();
                                                    traveller.TransactionId = Convert.ToInt32(row["BookingId"]);
                                                    traveller.PaxOrderId = Convert.ToInt32(row["PaxOrder"]);
                                                    traveller.PaxType = Convert.ToInt32(row["PaxType"]);
                                                    traveller.Title = Convert.ToInt32(row["Title"]);
                                                    traveller.FirstName = row["FirstName"] == DBNull.Value ? string.Empty : Convert.ToString(row["FirstName"]);
                                                    traveller.MiddleName = row["MiddleName"] == DBNull.Value ? string.Empty : Convert.ToString(row["MiddleName"]);
                                                    traveller.LastName = row["LastName"] == DBNull.Value ? string.Empty : Convert.ToString(row["LastName"]);
                                                    traveller.Gender = Convert.ToInt32(row["Gender"]);
                                                    traveller.DOB = row["DOB"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["DOB"]);
                                                    traveller.TicketsNo = row["TicketNo"] == DBNull.Value ? string.Empty : Convert.ToString(row["TicketNo"]);
                                                    response.Travellers.Add(traveller);
                                                }
                                            }
                                            break;

                                        case 6: //Baggage Protuction
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.BaggageInsurances = new BaggageInsurances();

                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    response.BaggageInsurances.BookingId = Convert.ToInt32(row["BookingId"]);
                                                    response.BaggageInsurances.ServiceNumber = row["ServiceNumber"] == DBNull.Value ? string.Empty : Convert.ToString(row["ServiceNumber"]);
                                                    response.BaggageInsurances.ErrorCode = row["ErrorCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["ErrorCode"]);
                                                    response.BaggageInsurances.ErrorMessage = row["ErrorMessage"] == DBNull.Value ? string.Empty : Convert.ToString(row["ErrorMessage"]);
                                                    response.BaggageInsurances.ProductId = Convert.ToInt32(row["ProductId"]);
                                                    response.BaggageInsurances.ProductName = row["ProductName"] == DBNull.Value ? string.Empty : Convert.ToString(row["ProductName"]);
                                                    response.BaggageInsurances.Status = Convert.ToBoolean(row["Status"]);
                                                    response.BaggageInsurances.StatusCode = row["StatusCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["StatusCode"]);
                                                    response.BaggageInsurances.TotalPrice = Convert.ToDecimal(row["TotalPrice"]);


                                                }
                                            }
                                            break;
                                        case 7: //Travel Protuction
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.TravelInsurance = new TravelInsurance();
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    response.TravelInsurance.BookingId = Convert.ToInt32(row["BookingId"]);
                                                    response.TravelInsurance.PolicyNumber = row["PolicyNumber"] == DBNull.Value ? string.Empty : Convert.ToString(row["PolicyNumber"]);
                                                    response.TravelInsurance.RefNumber = row["RefNumber"] == DBNull.Value ? string.Empty : Convert.ToString(row["RefNumber"]);
                                                    response.TravelInsurance.GroupNumber = row["GroupNumber"] == DBNull.Value ? string.Empty : Convert.ToString(row["GroupNumber"]);
                                                    response.TravelInsurance.TotalPrice = Convert.ToDecimal(row["TotalPrice"]);
                                                    response.TravelInsurance.Warnings = row["Warnings"] == DBNull.Value ? string.Empty : Convert.ToString(row["Warnings"]);
                                                }
                                            }
                                            break;
                                        case 8: //Coupon Details
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.CouponDetails = new Coupon();
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    response.CouponDetails.CouponCode = row["CouponCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["CouponCode"]);
                                                    response.CouponDetails.CouponMessage = row["CouponMessage"] == DBNull.Value ? string.Empty : Convert.ToString(row["CouponMessage"]);
                                                    response.CouponDetails.Status = row["Status"] == DBNull.Value ? false : Convert.ToBoolean(row["Status"]);
                                                    response.CouponDetails.TotalAmount = row["TotalAmount"] == DBNull.Value ? 0 : Convert.ToSingle(row["TotalAmount"]);
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
                throw ex;
            }
            return response;
        }
        public static void SaveNewsLetter(string email, int _subscriptiontype, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "SubscribesInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@SubscriptionType", _subscriptiontype);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SentItineryDetails(RequestedItinerary request, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "RequestedItineraryInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@PortalId", request.PortalId);
                        command.Parameters.AddWithValue("@Email", request.Email);
                        command.Parameters.AddWithValue("@Origin", request.Origin);
                        command.Parameters.AddWithValue("@Destination", request.Destination);
                        command.Parameters.AddWithValue("@TripType", request.TripType);
                        command.Parameters.AddWithValue("@Departure", request.Departure);
                        command.Parameters.AddWithValue("@Return", request.Return != null ? request.Return : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IP", request.IP);
                        command.Parameters.AddWithValue("@SentSuccess", request.SentSuccess);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void RequestACallDetails(RequestedItinerary request, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "RequestedCallInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@PortalId", request.PortalId);
                        command.Parameters.AddWithValue("@Phone", request.Phone);
                        command.Parameters.AddWithValue("@Origin", request.Origin);
                        command.Parameters.AddWithValue("@Destination", request.Destination);
                        command.Parameters.AddWithValue("@TripType", request.TripType);
                        command.Parameters.AddWithValue("@Departure", request.Departure);
                        command.Parameters.AddWithValue("@Return", request.Return != null ? request.Return : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IP", request.IP);
                        command.Parameters.AddWithValue("@SentSuccess", request.SentSuccess);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool IsSaveFlightSearches(DataTable _flightSearchDetail, string _connectionString)
        {
            bool isSave = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "FlightSearchInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@tblFlightSearch", _flightSearchDetail);
                        command.ExecuteNonQuery();
                        isSave = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isSave;
        }
        public static void SaveCampaignTrackings(BookingCampaignTracking _tracking, string _connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "CampaignTrackingsInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@BookingId", _tracking.BookingId);
                        command.Parameters.AddWithValue("@AffiliateId", _tracking.AffiliateId);
                        command.Parameters.AddWithValue("@SearchGuid", _tracking.SearchGuid);
                        command.Parameters.AddWithValue("@UtmSource", string.IsNullOrEmpty(_tracking.UtmSource) ? string.Empty : _tracking.UtmSource);
                        command.Parameters.AddWithValue("@UtmMedium", string.IsNullOrEmpty(_tracking.UtmMedium) ? string.Empty : _tracking.UtmMedium);
                        command.Parameters.AddWithValue("@UtmCampaign", string.IsNullOrEmpty(_tracking.UtmCampaign) ? string.Empty : _tracking.UtmCampaign);
                        command.Parameters.AddWithValue("@ClickedId", string.IsNullOrEmpty(_tracking.ClickedId) ? string.Empty : _tracking.ClickedId);
                        command.Parameters.AddWithValue("@UtmTerm", string.IsNullOrEmpty(_tracking.UtmTerm) ? string.Empty : _tracking.UtmTerm);
                        command.Parameters.AddWithValue("@UtmContent", string.IsNullOrEmpty(_tracking.UtmContent) ? string.Empty : _tracking.UtmContent);
                        command.Parameters.AddWithValue("@UtmKeyword", string.IsNullOrEmpty(_tracking.UtmKeyword) ? string.Empty : _tracking.UtmKeyword);
                        command.Parameters.AddWithValue("@Origin", string.IsNullOrEmpty(_tracking.Origin) ? string.Empty : _tracking.Origin);
                        command.Parameters.AddWithValue("@Destination", string.IsNullOrEmpty(_tracking.Destination) ? string.Empty : _tracking.Destination);
                        command.Parameters.AddWithValue("@TripType", _tracking.TripType);
                        command.Parameters.AddWithValue("@IsMobile", _tracking.IsMobile);
                        command.Parameters.AddWithValue("@UtmPublisher", string.IsNullOrEmpty(_tracking.UtmPublisher) ? string.Empty : _tracking.UtmPublisher);
                        command.Parameters.AddWithValue("@UtmPublisherId", string.IsNullOrEmpty(_tracking.UtmPublisherId) ? string.Empty : _tracking.UtmPublisherId);
                        command.Parameters.AddWithValue("@UtmChannelId", string.IsNullOrEmpty(_tracking.UtmChannelId) ? string.Empty : _tracking.UtmChannelId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static BookingDetails GetDocuSignBookingDetails(int bookingId, int cardId, string connectionString)
        {
            BookingDetails response = null;
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
                        command.CommandText = "DocuSignBookingDetailsGet";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@bookingId", bookingId);
                        command.Parameters.AddWithValue("@billingCardId", cardId);
                        sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(ds);
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            response = new BookingDetails();

                            for (int i = 0; i <= ds.Tables.Count - 1; i++)
                            {
                                if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                {
                                    switch (i)
                                    {
                                        case 0://Bookings
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.Transaction = new Transactions();
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    response.Transaction.Id = Convert.ToInt32(row["Id"]);
                                                    response.Transaction.PNR = row["PNR"] == DBNull.Value ? string.Empty : Convert.ToString(row["PNR"]);
                                                    response.Transaction.PortalId = Convert.ToInt32(row["PortalId"]);
                                                    response.Transaction.GDS = Convert.ToInt32(row["GDS"]);
                                                    response.Transaction.ProviderId = Convert.ToInt32(row["ProviderId"]);
                                                    response.Transaction.BookingType = Convert.ToInt32(row["BookingType"]);
                                                    response.Transaction.BookingSourceType = Convert.ToInt32(row["BookingSourceType"]);
                                                    response.Transaction.BookingStatus = Convert.ToInt32(row["BookingStatus"]);
                                                    response.Transaction.BookingSubStatus = Convert.ToInt32(row["BookingSubStatus"]);
                                                    response.Transaction.AgentId = Convert.ToInt32(row["AgentId"]);
                                                    response.Transaction.AgentLead = Convert.ToInt32(row["AgentLead"]);
                                                    response.Transaction.UserId = Convert.ToInt32(row["UserId"]);
                                                    response.Transaction.BookedOn = DateTime.SpecifyKind(Convert.ToDateTime(row["Created"]), DateTimeKind.Utc);

                                                    break;
                                                }
                                            }
                                            break;
                                        case 1: //Flights
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.Flight = new Flights();
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    response.Flight.TransactionId = Convert.ToInt32(row["BookingId"]);
                                                    response.Flight.Origin = row["OriginCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["OriginCode"]);
                                                    response.Flight.Destination = row["DestinationCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["DestinationCode"]);
                                                    response.Flight.Airline = row["ValAirlineCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["ValAirlineCode"]);
                                                    response.Flight.TripType = Convert.ToInt32(row["TripType"]);
                                                    response.Flight.DeptDate = Convert.ToDateTime(row["DeptDate"]);
                                                    response.Flight.ReturnDate = row["ReturnDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ReturnDate"]);
                                                    response.Flight.TotalPaxCount = Convert.ToInt32(row["PaxCount"]);
                                                    response.Flight.AdultCount = Convert.ToInt32(row["AdultCount"]);
                                                    response.Flight.SeniorCount = Convert.ToInt32(row["SeniorCount"]);
                                                    response.Flight.ChildCount = Convert.ToInt32(row["ChildCount"]);
                                                    response.Flight.InfantCount = Convert.ToInt32(row["InfantOnSeatCount"]);
                                                    response.Flight.InfantLapCount = Convert.ToInt32(row["InfantLapCount"]);
                                                    response.Flight.OutBoundFlightDuration = Convert.ToInt64(row["OutBoundFlightDuration"]);
                                                    response.Flight.InBoundFlightDuration = Convert.ToInt64(row["InBoundFlightDuration"]);
                                                    response.Flight.IsDomestic = Convert.ToBoolean(row["IsDomestic"]);
                                                    response.Flight.FareType = row["FareType"] == DBNull.Value ? "" : Convert.ToString(row["FareType"]);
                                                    response.Flight.ContractType = Convert.ToInt32(row["ContractType"]);
                                                    break;
                                                }
                                            }
                                            break;
                                        case 2: //FlightSegments
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.Segments = new List<FlightSegments>();
                                                FlightSegments segment = null;
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    segment = new FlightSegments();
                                                    segment.TransactionId = Convert.ToInt32(row["BookingId"]);
                                                    segment.SegmentOrder = Convert.ToInt32(row["SegmentOrder"]);
                                                    segment.IsReturn = Convert.ToBoolean(row["IsReturn"]);
                                                    segment.FlightNumber = row["FlightNumber"] == DBNull.Value ? string.Empty : Convert.ToString(row["FlightNumber"]);
                                                    segment.MarketingCode = row["MktAirlineCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["MktAirlineCode"]);
                                                    segment.OperatingCode = row["OptAirlineCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["OptAirlineCode"]);
                                                    segment.Origin = row["OriginCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["OriginCode"]);
                                                    segment.DeptDateTime = Convert.ToDateTime(row["DeptDateTime"]);
                                                    segment.DeptTerminal = row["DeptTerminal"] == DBNull.Value ? string.Empty : Convert.ToString(row["DeptTerminal"]);
                                                    segment.Destination = row["DestinationCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["DestinationCode"]);
                                                    segment.ArrivalDateTime = Convert.ToDateTime(row["ArrivalDateTime"]);
                                                    segment.DeptTerminal = row["ArrivalTerminal"] == DBNull.Value ? string.Empty : Convert.ToString(row["ArrivalTerminal"]);
                                                    segment.EquipmentDetail = row["EquipmentDetail"] == DBNull.Value ? string.Empty : Convert.ToString(row["EquipmentDetail"]);
                                                    segment.SegmentClass = row["SegmentClass"] == DBNull.Value ? string.Empty : Convert.ToString(row["SegmentClass"]);
                                                    segment.Stops = Convert.ToInt32(row["Stops"]);
                                                    segment.Cabin = Convert.ToInt32(row["Cabin"]);
                                                    segment.CompanyFranchiseDetails = row["CompanyFranchiseDetails"] == DBNull.Value ? string.Empty : Convert.ToString(row["CompanyFranchiseDetails"]);
                                                    segment.TechnicalStoppages = row["TechnicalStoppages"] == DBNull.Value ? string.Empty : Convert.ToString(row["TechnicalStoppages"]);
                                                    segment.AirlineLocator = row["AirlineLocator"] == DBNull.Value ? string.Empty : Convert.ToString(row["AirlineLocator"]);
                                                    response.Segments.Add(segment);
                                                }
                                            }
                                            break;
                                        case 3: //Flight Price Details
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.PriceDetail = new List<PriceDetails>();
                                                PriceDetails priceDetail = null;
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    priceDetail = new PriceDetails();
                                                    priceDetail.TransactionId = Convert.ToInt32(row["BookingId"]);
                                                    priceDetail.FareBaseCode = row["FareBaseCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["FareBaseCode"]);
                                                    priceDetail.PaxType = Convert.ToInt32(row["PaxType"]);
                                                    priceDetail.Currency = Convert.ToInt32(row["Currency"]);
                                                    priceDetail.PaxCount = Convert.ToInt32(row["PaxCount"]);
                                                    priceDetail.BaseFare = Convert.ToDecimal(row["BaseFare"]);
                                                    priceDetail.Tax = Convert.ToDecimal(row["Tax"]);
                                                    priceDetail.Markup = Convert.ToDecimal(row["Markup"]);
                                                    priceDetail.SupplierFee = Convert.ToDecimal(row["SupplierFee"]);
                                                    priceDetail.Discount = Convert.ToDecimal(row["Discount"]);
                                                    priceDetail.IsSellInsurance = Convert.ToBoolean(row["IsSellInsurance"]);
                                                    priceDetail.InsuranceAmount = Convert.ToDecimal(row["InsuranceAmount"]);
                                                    priceDetail.TotalAmount = Convert.ToDecimal(row["TotalAmount"]);
                                                    response.PriceDetail.Add(priceDetail);
                                                }
                                            }
                                            break;
                                        case 4: //BillingDetails
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.BillingDetails = new BillingDetails();
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    response.BillingDetails.TransactionId = Convert.ToInt32(row["BookingId"]);
                                                    response.BillingDetails.CCHolderName = row["CCHolderName"] == DBNull.Value ? string.Empty : Convert.ToString(row["CCHolderName"]);
                                                    response.BillingDetails.CardNumber = row["CardNumber"] == DBNull.Value ? string.Empty : Convert.ToString(row["CardNumber"]);
                                                    response.BillingDetails.CVVNumber = row["CVVNumber"] == DBNull.Value ? string.Empty : Convert.ToString(row["CVVNumber"]);


                                                    response.BillingDetails.ExpiryYear = Convert.ToInt32(row["ExpiryYear"]);
                                                    response.BillingDetails.ExpiryMonth = Convert.ToInt32(row["ExpiryMonth"]);
                                                    response.BillingDetails.CardType = Convert.ToInt32(row["CardType"]);
                                                    response.BillingDetails.Email = row["Email"] == DBNull.Value ? string.Empty : Convert.ToString(row["Email"]);
                                                    response.BillingDetails.Country = row["Country"] == DBNull.Value ? string.Empty : Convert.ToString(row["Country"]);
                                                    response.BillingDetails.State = row["State"] == DBNull.Value ? string.Empty : Convert.ToString(row["State"]);
                                                    response.BillingDetails.ZipCode = row["ZipCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["ZipCode"]);
                                                    response.BillingDetails.AddressLine1 = row["AddressLine1"] == DBNull.Value ? string.Empty : Convert.ToString(row["AddressLine1"]);
                                                    response.BillingDetails.AddressLine2 = row["AddressLine2"] == DBNull.Value ? string.Empty : Convert.ToString(row["AddressLine2"]);
                                                    response.BillingDetails.AddressLine3 = row["AddressLine3"] == DBNull.Value ? string.Empty : Convert.ToString(row["AddressLine3"]);
                                                    response.BillingDetails.City = row["City"] == DBNull.Value ? string.Empty : Convert.ToString(row["City"]);
                                                    response.BillingDetails.BillingPhone = row["BillingPhone"] == DBNull.Value ? string.Empty : Convert.ToString(row["BillingPhone"]);
                                                    response.BillingDetails.ContactPhone = row["ContactPhone"] == DBNull.Value ? string.Empty : Convert.ToString(row["ContactPhone"]);
                                                    response.BillingDetails.IsPrimaryCard = Convert.ToBoolean(row["IsPrimaryCard"]);
                                                    break;
                                                }
                                            }
                                            break;
                                        case 5: //Traveller Details
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.Travellers = new List<Travellers>();
                                                Travellers traveller = null;
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    traveller = new Travellers();
                                                    traveller.TransactionId = Convert.ToInt32(row["BookingId"]);
                                                    traveller.PaxOrderId = Convert.ToInt32(row["PaxOrder"]);
                                                    traveller.PaxType = Convert.ToInt32(row["PaxType"]);
                                                    traveller.Title = Convert.ToInt32(row["Title"]);
                                                    traveller.FirstName = row["FirstName"] == DBNull.Value ? string.Empty : Convert.ToString(row["FirstName"]);
                                                    traveller.MiddleName = row["MiddleName"] == DBNull.Value ? string.Empty : Convert.ToString(row["MiddleName"]);
                                                    traveller.LastName = row["LastName"] == DBNull.Value ? string.Empty : Convert.ToString(row["LastName"]);
                                                    traveller.Gender = Convert.ToInt32(row["Gender"]);
                                                    traveller.DOB = Convert.ToDateTime(row["DOB"]);
                                                    traveller.TicketsNo = row["TicketNo"] == DBNull.Value ? string.Empty : Convert.ToString(row["TicketNo"]);
                                                    response.Travellers.Add(traveller);
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
                throw ex;
            }
            return response;
        }
        public static int SaveEasyPay(EasyPayDetails _easypay, string _connectionString)
        {
            int epId = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "EasyPayDetailsInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@BookingId", _easypay.BookingId);
                        command.Parameters.AddWithValue("@AgentId", _easypay.AgentId);
                        command.Parameters.AddWithValue("@PortalId", _easypay.PortalId);
                        command.Parameters.AddWithValue("@ClientIP", string.IsNullOrEmpty(_easypay.ClientIP) ? string.Empty : _easypay.ClientIP);
                        command.Parameters.AddWithValue("@Doc", string.IsNullOrEmpty(_easypay.Doc) ? string.Empty : _easypay.Doc);
                        command.Parameters.AddWithValue("@CIN", string.IsNullOrEmpty(_easypay.CIN) ? string.Empty : _easypay.CIN);
                        command.Parameters.AddWithValue("@UserID", string.IsNullOrEmpty(_easypay.UserID) ? string.Empty : _easypay.UserID);
                        command.Parameters.AddWithValue("@PayStatus", _easypay.PayStatus);
                        command.Parameters.AddWithValue("@SubMerchantId", string.IsNullOrEmpty(_easypay.SubMerchantId) ? string.Empty : _easypay.SubMerchantId);
                        command.Parameters.AddWithValue("@PayValue", _easypay.PayValue);
                        if (_easypay.Paydate != null && _easypay.Paydate != DateTime.MinValue)
                        {
                            command.Parameters.AddWithValue("@Paydate", _easypay.Paydate);
                        }
                        command.Parameters.AddWithValue("@PaymentType", string.IsNullOrEmpty(_easypay.PaymentType) ? string.Empty : _easypay.PaymentType);
                        command.Parameters.AddWithValue("@ValueFixed", _easypay.ValueFixed);
                        command.Parameters.AddWithValue("@ValueVar", _easypay.ValueVar);
                        command.Parameters.AddWithValue("@ValueTax", _easypay.ValueTax);
                        command.Parameters.AddWithValue("@ValueTransf", _easypay.ValueTransf);
                        command.Parameters.AddWithValue("@DateTransf", _easypay.DateTransf != null ? _easypay.DateTransf : null);
                        command.Parameters.AddWithValue("@isMailSent", _easypay.IsMailSent);
                        command.Parameters.AddWithValue("@isReceiptSent", _easypay.isReceiptSent);
                        command.Parameters.AddWithValue("@TransactionId", _easypay.TransactionId);
                        command.Parameters.Add("@Id", SqlDbType.Int);
                        command.Parameters["@Id"].Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();
                        epId = (int)command.Parameters["@Id"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return epId;
        }
        public static void UpdateEasyPay(EasyPayDetails _easypay, string _connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {

                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "EasyPayDetailsUpdate";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@Id", _easypay.Id);
                        command.Parameters.AddWithValue("@DOC", string.IsNullOrEmpty(_easypay.Doc) ? string.Empty : _easypay.Doc);
                        command.Parameters.AddWithValue("@PayStatus", _easypay.PayStatus);
                        command.Parameters.AddWithValue("@PayValue", _easypay.PayValue);
                        command.Parameters.AddWithValue("@PaymentType", string.IsNullOrEmpty(_easypay.PaymentType) ? string.Empty : _easypay.PaymentType);
                        command.Parameters.AddWithValue("@ValueFixed", _easypay.ValueFixed);
                        command.Parameters.AddWithValue("@ValueVar", _easypay.ValueVar);
                        command.Parameters.AddWithValue("@ValueTax", _easypay.ValueTax);
                        command.Parameters.AddWithValue("@isMailSent", _easypay.IsMailSent);
                        command.Parameters.AddWithValue("@isReceiptSent", _easypay.isReceiptSent);
                        command.Parameters.AddWithValue("@TransactionId", _easypay.TransactionId);
                        if (_easypay.DateTransf != null && _easypay.DateTransf != DateTime.MinValue)
                        {
                            command.Parameters.AddWithValue("@DateTransf", _easypay.DateTransf);
                        }
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static EasyPayDetails GetEasyPay(int Id, string connectionString)
        {
            EasyPayDetails response = null;
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
                        command.CommandText = "GetEasyPayDetails";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@Id", Id);
                        sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(ds);
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            response = new EasyPayDetails();

                            for (int i = 0; i <= ds.Tables.Count - 1; i++)
                            {
                                if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    response.Id = Convert.ToInt32(row["Id"]);

                                                    response.BookingId = Convert.ToInt32(row["BookingId"]);
                                                    response.ClientIP = Convert.ToString(row["ClientIP"]);
                                                    response.PortalId = Convert.ToInt32(row["PortalId"]);
                                                    response.AgentId = Convert.ToInt32(row["AgentId"]);
                                                    response.Doc = row["Doc"] == DBNull.Value ? string.Empty : Convert.ToString(row["Doc"]);
                                                    response.CIN = Convert.ToString(row["CIN"]);
                                                    response.UserID = Convert.ToString(row["UserID"]);
                                                    response.PayStatus = Convert.ToInt32(row["PayStatus"]);
                                                    response.SubMerchantId = row["SubMerchantId"] == DBNull.Value ? string.Empty : Convert.ToString(row["SubMerchantId"]);
                                                    response.PayValue = Convert.ToDecimal(row["PayValue"]);
                                                    response.Paydate = DateTime.SpecifyKind(Convert.ToDateTime(row["Paydate"]), DateTimeKind.Utc);
                                                    response.PaymentType = row["PaymentType"] == DBNull.Value ? string.Empty : Convert.ToString(row["PaymentType"]);
                                                    response.ValueFixed = Convert.ToDecimal(row["ValueFixed"]);
                                                    response.ValueVar = Convert.ToDecimal(row["ValueVar"]);
                                                    response.ValueTax = Convert.ToDecimal(row["ValueTax"]);
                                                    response.ValueTransf = Convert.ToDecimal(row["ValueTransf"]);
                                                    response.DateTransf = DateTime.SpecifyKind(Convert.ToDateTime(row["DateTransf"] != DBNull.Value ? row["DateTransf"] : DateTime.UtcNow), DateTimeKind.Utc);
                                                    response.IsMailSent = Convert.ToBoolean(row["isMailSent"]);
                                                    response.isReceiptSent = Convert.ToBoolean(row["isReceiptSent"] != DBNull.Value ? Convert.ToBoolean(row["isReceiptSent"]) : false);
                                                    response.TransactionId = Convert.ToInt64(row["TransactionId"]);
                                                    response.Created = DateTime.SpecifyKind(Convert.ToDateTime(row["Created"]), DateTimeKind.Utc);

                                                    break;
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
                throw ex;
            }
            return response;
        }
        public static void AddBookingFailureDatail(BookingFailureDetails _bookingFailureDetails, string _connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "BookingFailureDetailsInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@guid", _bookingFailureDetails.Guid);
                        command.Parameters.AddWithValue("@name", _bookingFailureDetails.Name);
                        command.Parameters.AddWithValue("@bookingstatus", _bookingFailureDetails.BookingStatus);
                        command.Parameters.AddWithValue("@oldprice", _bookingFailureDetails.OldPrice);
                        command.Parameters.AddWithValue("@newprice", _bookingFailureDetails.NewPrice);
                        command.Parameters.AddWithValue("@action", DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void UpdateBookingFailureDatail(string _guid, string _action, string _connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "BookingFailureDetailsUpdate";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@guid", _guid);
                        command.Parameters.AddWithValue("@action", _action);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LogsContractSoldout(Contract contract, FlightSearch flightSearch, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "LogsContractSoldoutsInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@Guid", contract.SearchGuid);
                        command.Parameters.AddWithValue("@ContractId", contract.ContractId);
                        command.Parameters.AddWithValue("@AffiliateId", contract.AffiliateId);
                        command.Parameters.AddWithValue("@ClientIP", flightSearch.IP);
                        command.Parameters.AddWithValue("@Origin", contract.Origin);
                        command.Parameters.AddWithValue("@Destination", contract.Destination);
                        command.Parameters.AddWithValue("@DepartureDate", contract.DepartureDate);
                        command.Parameters.AddWithValue("@TripType", (int)contract.TripType);
                        command.Parameters.AddWithValue("@Provider", (int)contract.Provider);
                        if (contract.TripType == Infrastructure.TripType.ROUNDTRIP)
                        {
                            command.Parameters.AddWithValue("@Return", contract.ArrivalDate);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Return", DBNull.Value);
                        }
                        command.Parameters.AddWithValue("@Adult", contract.Adult);
                        command.Parameters.AddWithValue("@Child", contract.Child);
                        command.Parameters.AddWithValue("@Infant", contract.InfantOnLap);
                        command.Parameters.AddWithValue("@Fare", contract.TotalGDSFareV2);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int SaveBookingDetails(Dictionary<string, DataTable> bookingDetail, string connectionString)
        {
            int tid = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "usp_SaveBookingDetails";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@tblBookings", bookingDetail["Bookings"]);
                        command.Parameters.AddWithValue("@tblFlights", bookingDetail["Flights"]);
                        command.Parameters.AddWithValue("@tblSegments", bookingDetail["Segments"]);
                        command.Parameters.AddWithValue("@tblTravellers", bookingDetail["Travellers"]);
                        command.Parameters.AddWithValue("@tblPriceDetails", bookingDetail["PriceDetails"]);
                        command.Parameters.AddWithValue("@tblBillingDetails", bookingDetail["BillingDetails"]);
                        command.Parameters.AddWithValue("@tblBookingExtras", bookingDetail["BookingExtras"]);
                        command.Parameters.AddWithValue("@tblMetaFixedPriceDetails", bookingDetail["MetaFixedPriceDetails"]);
                        tid = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tid;
        }

        public static List<CouponData> GetCouponDataDetails(int portalId, string connectionString)
        {
            List<CouponData> response = null;
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
                        command.CommandText = "GetCouponMasterData";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@PortalId", portalId);
                        sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(ds);
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                        {
                            response = new List<CouponData>();
                            CouponData coupon = null;
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                coupon = new CouponData();
                                coupon.Id = Convert.ToInt32(row["Id"]);
                                coupon.CouponCode = row["CouponCode"] == DBNull.Value ? string.Empty : Convert.ToString(row["CouponCode"]);
                                coupon.CouponLabel = row["CouponLabel"] == DBNull.Value ? string.Empty : Convert.ToString(row["CouponLabel"]);
                                coupon.DiscountType = (DiscountType)Convert.ToInt32(row["DiscountType"]);
                                coupon.Amount = row["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Amount"]);
                                coupon.Percentage = row["Percentage"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Percentage"]);
                                coupon.IsDefault = row["IsDefault"] == DBNull.Value ? false : Convert.ToBoolean(row["IsDefault"]);
                                response.Add(coupon);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public static GoogleReviews GetReviews(string connectionString)
        {
            GoogleReviews response = null;
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
                        command.CommandText = "ReviewsGet";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        sqlDataAdapter = new SqlDataAdapter(command);
                        sqlDataAdapter.Fill(ds);
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            response = new GoogleReviews();

                            for (int i = 0; i <= ds.Tables.Count - 1; i++)
                            {
                                if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                {
                                    switch (i)
                                    {
                                        case 0://Reviews
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {

                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    response.Id = Convert.ToInt32(row["Id"]);
                                                    response.Name = row["Name"] == DBNull.Value ? string.Empty : Convert.ToString(row["Name"]);
                                                    response.Address = row["Address"] == DBNull.Value ? string.Empty : Convert.ToString(row["Address"]);
                                                    response.Rating = Convert.ToDecimal(row["Rating"]);
                                                    response.Reviews = Convert.ToInt32(row["Reviews"]);
                                                    response.Created = DateTime.SpecifyKind(Convert.ToDateTime(row["Created"]), DateTimeKind.Utc);
                                                    break;
                                                }
                                            }
                                            break;
                                        case 1: //Sub Reviews
                                            if (ds.Tables[i] != null && ds.Tables[i].Rows.Count > 0)
                                            {
                                                response.GoogleSubReviews = new List<GoogleSubReviews>();
                                                foreach (DataRow row in ds.Tables[i].Rows)
                                                {
                                                    response.GoogleSubReviews.Add(new GoogleSubReviews()
                                                    {
                                                        Id = row["Id"] == DBNull.Value ? 0 : Convert.ToInt32(row["Id"]),
                                                        ReviewId = row["ReviewId"] == DBNull.Value ? 0 : Convert.ToInt32(row["ReviewId"]),
                                                        UserName = row["UserName"] == DBNull.Value ? string.Empty : Convert.ToString(row["UserName"]),
                                                        UserRating = row["UserRating"] == DBNull.Value ? 0 : Convert.ToDecimal(row["UserRating"]),
                                                        UserImage = row["UserImage"] == DBNull.Value ? string.Empty : Convert.ToString(row["UserImage"]),
                                                        UserLink = row["UserLink"] == DBNull.Value ? string.Empty : Convert.ToString(row["UserLink"]),
                                                        Date = row["Date"] == DBNull.Value ? string.Empty : Convert.ToString(row["Date"]),
                                                        ReviewText = row["ReviewText"] == DBNull.Value ? string.Empty : Convert.ToString(row["ReviewText"]),
                                                        ReviewSummary = row["ReviewSummary"] == DBNull.Value ? string.Empty : Convert.ToString(row["ReviewSummary"]),
                                                        Created = row["Created"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["Created"])
                                                    });

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
                throw ex;
            }
            return response;
        }

        public static void MetaClicks(MetaClicks request, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "MetaClicksInsert";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@PortalId", request.PortalId);
                        command.Parameters.AddWithValue("@Origin", request.Origin);
                        command.Parameters.AddWithValue("@Destination", request.Destination);
                        command.Parameters.AddWithValue("@TripType", request.TripType);
                        command.Parameters.AddWithValue("@Departure", request.Departure);
                        command.Parameters.AddWithValue("@Return", request.Return != null ? request.Return : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IP", request.IP);
                        command.Parameters.AddWithValue("@AffiliateId", request.AffiliateId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void UpdateBookingUserLocation(int bookingId, string location, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        connection.Open();
                        command.Connection = connection;
                        command.CommandText = "BookingsUserLocationUpdate";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 3600;
                        command.Parameters.AddWithValue("@BookingId", bookingId);
                        command.Parameters.AddWithValue("@UserLocation", location);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
